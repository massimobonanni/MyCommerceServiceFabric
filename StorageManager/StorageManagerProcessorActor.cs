using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using MyCommerce.SF.Core.Interfaces;
using MyCommerce.SF.Core.Entities;
using System.Data.SqlClient;

namespace StorageManager
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = "StorageManagerProcessorActor")]
    internal class StorageManagerProcessorActor : MyCommerce.SF.Core.Actors.ProcessorBase, IProcessor
    {
        public StorageManagerProcessorActor(ActorService actorService, ActorId actorId) : 
            base(actorService, actorId)
        {
        }

        public StorageManagerProcessorActor(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {
        }

        private string connectionString;
        private string deadLetterQueueStoredProcedureName;

        private void ConfigChanged(object sender, System.Fabric.PackageModifiedEventArgs<System.Fabric.ConfigurationPackage> e)
        {
            this.WriteEnterMethod();
            this.ReadConfig();
        }

        private static object GetSqlFormattedValue(object value)
        {
            if (value == null)
            {
                return value;
            }

            if (value.GetType() == typeof(string)
                || value.GetType() == typeof(Guid)
                || value.GetType() == typeof(UInt16)
                || value.GetType() == typeof(UInt32)
                || value.GetType() == typeof(UInt64))
            {
                return string.Concat("\"", value.ToString(), "\"");
            }

            if (value.GetType() == typeof(DateTime))
            {
                return string.Concat("\"", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), "\"");
            }

            return value;
        }

        private object GetSqlSafeValue(object value)
        {
            if (value == null)
            {
                return value;
            }

            if (value.GetType() == typeof(UInt16)
                || value.GetType() == typeof(UInt32)
                || value.GetType() == typeof(UInt64))
            {
                return value.ToString();
            }

            return value;
        }

        private void ReadConfig()
        {
            this.WriteEnterMethod();

            this.connectionString = this.ReadSetting("ConnectionString");
            this.deadLetterQueueStoredProcedureName = this.ReadSetting("DeadLetterQueueStoredProcedureName");
            //this.delayBeforeRetry = TimeSpan.FromMilliseconds(Convert.ToInt32(this.ReadSetting("DelayBeforeRetryInMilliseconds")));
        }

        private static string ConvertCommandToSqlStatement(Command command)
        {
            // TODO da migliorare per gestire meglio il valore dei parametri in base al tipo
            var parameters = command.Parameters.Select((param) => $"@{param.Key} = {GetSqlFormattedValue(param.Value)}");
            var statementParameters = string.Join(", ", parameters);
            return $"exec {command.Name} {statementParameters}";
        }

        protected override async Task<bool> ExecuteCommandInternal(Command command)
        {
            string sqlStatement = ConvertCommandToSqlStatement(command);
            this.WriteVerbose($"ExecuteCommand: {sqlStatement}");

            try
            {
                using (SqlConnection con = new SqlConnection(this.connectionString))
                {
                    try
                    {
                        await con.OpenAsync();
                    }
                    catch (SqlException ex)
                    {
                        //IF Connection error then retry, otherwise put it in dead letter queue
                        //this.WriteVerbose($"ExecuteCommand:  SqlException on open connection. Retry in {this.delayBeforeRetry} milliseconds. {sex.ToString()}");
                        //Thread.Sleep(this.delayBeforeRetryInMilliseconds);
                        return false;
                    }

                    var sqlCommand = con.CreateCommand();
                    sqlCommand.CommandText = command.Name;
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    foreach (var param in command.Parameters)
                    {
                        var paramName = param.Key;
                        if (!paramName.StartsWith("@"))
                        {
                            paramName = string.Concat("@", param.Key);
                        }

                        object value = GetSqlSafeValue(param.Value);

                        sqlCommand.Parameters.AddWithValue(paramName, value);
                    }

                    try
                    {
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                    catch (SqlException sex)
                    {
                        this.WriteException(sex, "ExecuteCommand:  SqlException on execute query");
                        if (sex.Errors.Count == 0 || sex.Errors[0].Number != 8145)      // Invalid SP parameter
                            throw;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.WriteException(ex, "ExecuteCommand:  GenericError");
                //if (!command.Properties.ContainsKey("DeadLetterQueue"))
                //{
                //    await this.EnqueueCommandInDeadLetterQueue(command, ex);
                //    return false;
                //}
                //else
                //{
                //    this.WriteException(ex, "ExecuteCommand:  Data will be lost. {0}", sqlStatement);
                //    return true;
                //}
                return true;
            }
        }



        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            this.ReadConfig();
            this.ActorService.Context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += this.ConfigChanged;
            return base.OnActivateAsync();

        }

    }
}
