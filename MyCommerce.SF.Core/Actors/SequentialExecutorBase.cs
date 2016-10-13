using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Entities;
using MyCommerce.SF.Core.Extensions;
using MyCommerce.SF.Core.Interfaces;

namespace MyCommerce.SF.Core.Actors
{
    public abstract class SequentialExecutorBase : MyCommerce.SF.Core.Actors.ActorBase, ISequentialExecutor, IProcessorCallback, IRemindable
    {
        public SequentialExecutorBase(ActorService actorService, ActorId actorId) : 
            base(actorService, actorId)
        {
        }

        public SequentialExecutorBase(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {
        }

        const string Command_Queue_Name = "CommandQueue";
        const string ReminderIsRegistered = "ReminderIsRegistered";
        const string WorkCommand_ReminderName = "WorkCommmand";
        const string CurrentExecutingCommand_KeyName = "CurrentExecutingCommand";

        protected virtual Uri ProcessorServiceUri { get; }

        protected virtual string ProcessorServiceName { get; }

        protected virtual string ProcessorApplicationName { get; }

        protected virtual bool StartDequeue { get { return true; } }

        protected virtual Task CheckCommandBeforeEnqueue(Command command) { return Task.FromResult(true); }

        public async Task<Command> GetCurrentProcessingCommandAsync()
        {
            this.WriteEnterMethod();
            var result = await this.StateManager.TryGetStateAsync<Command>(CurrentExecutingCommand_KeyName);
            return result.Value;
        }

        public Task<int> GetQueueLengthAsync()
        {
            this.WriteEnterMethod();
            return this.StateManager.GetQueueLengthAsync(Command_Queue_Name);
        }

        public virtual async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            this.WriteVerbose($"ReceiveReminderAsync({reminderName}, {dueTime}, {period})");

            if (!string.Equals(reminderName, WorkCommand_ReminderName))
            {
                return;
            }

            if (!this.StartDequeue)
            {
                this.WriteVerbose("StartDequeue set to false.");
                return;
            }

            var currentExecutingCommand = await this.StateManager.TryGetStateAsync<Command>(CurrentExecutingCommand_KeyName);

            if (currentExecutingCommand.HasValue)
            {
                this.WriteVerbose("A command is already running.");
                return;
            }

            var nextCommand = await this.StateManager.DequeueAsync<Command>(Command_Queue_Name);

            if (nextCommand == null)
            {
                this.WriteVerbose("Queue is empty. Deactivate reminder");
                await DeactivateWorkCommandReminderAsync() ;
                return;
            }

            IProcessor processorProxy;

            if (this.ProcessorServiceUri != null)
            {
                processorProxy = ActorProxy.Create<IProcessor>(this.Id, this.ProcessorServiceUri);
            }
            else
            {
                processorProxy = ActorProxy.Create<IProcessor>(this.Id, this.ProcessorApplicationName, this.ProcessorServiceName);
            }

            try
            {
                await processorProxy.ProcessAsync(nextCommand, this.ActorService.Context.ServiceName);
            }
            catch (Exception ex)
            {
                this.WriteException(ex, $"Error invoking Uri:{this.ProcessorServiceUri}, ApplicationName:{this.ProcessorApplicationName}, ServiceName:{this.ProcessorServiceName}");
                throw;
            }

            await this.StateManager.SetStateAsync(CurrentExecutingCommand_KeyName, nextCommand);
        }

        public async Task ProcessingCompleteCallBackAsync()
        {
            this.WriteEnterMethod();

            //Remove current executing command from the state
            await this.StateManager.TryRemoveStateAsync(CurrentExecutingCommand_KeyName);

            //Start next processing
            await this.RegisterReminderAsync(
                Guid.NewGuid().ToString(),
                null,
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(-1));
        }

        private async Task ActivateWorkCommandReminderAsync()
        {
            this.WriteEnterMethod();

            await this.ActivateWorkCommandRiminderSafeAsync(WorkCommand_ReminderName, null, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
        }

        private async Task DeactivateWorkCommandReminderAsync()
        {
            this.WriteEnterMethod();
            await this.DeactivateWorkCommandReminderSafeAsync(WorkCommand_ReminderName);
        }

        protected virtual Task<int> GetQueueLength()
        {
            this.WriteEnterMethod();
            return this.StateManager.GetQueueLengthAsync(Command_Queue_Name);
        }

        protected virtual Task PurgeQueue()
        {
            this.WriteEnterMethod();
            return this.StateManager.PurgeQueue(Command_Queue_Name);
        }

        public async Task ExecuteAsync(Command command)
        {
            this.WriteEnterMethod();
            this.WriteVerbose("Invoking CheckCommandBeforeEnqueue");
            await this.CheckCommandBeforeEnqueue(command);

            if (this.StartDequeue)
            {
                await ActivateWorkCommandReminderAsync();
            }

            await this.StateManager.EnqueueAsync(Command_Queue_Name, command);
        }
    }
}
