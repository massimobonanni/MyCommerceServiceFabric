using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Entities;
using MyCommerce.SF.Core.Interfaces;

namespace MyCommerce.SF.Core.Actors
{
    public abstract class ProcessorBase : MyCommerce.SF.Core.Actors.ActorBase, IProcessor, IRemindable
    {
        public ProcessorBase(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public ProcessorBase(ActorService actorService, ActorId actorId,
            IActorStateManager stateManager, IActorFactory actorFactory, IServiceFactory serviceFactory) :
            base(actorService, actorId, stateManager, actorFactory, serviceFactory)
        {
        }

        const string CurrentCommand_KeyName = "CurrentCommand";
        const string CallbackServiceUri_KeyName = "CallbackServiceUri";
        const string WorkCommand_ReminderName = "WorkCommmand";

        protected override async Task OnDeactivateAsync()
        {
            this.WriteEnterMethod();
            await this.DeactivateWorkCommandReminderAsync();
            await base.OnDeactivateAsync();
        }

        public async Task ProcessAsync(Command command, Uri callbackServiceUri)
        {
            this.WriteEnterMethod();
            await this.ActivateWorkCommandReminderAsync();
            await this.StateManager.AddOrUpdateStateAsync(CurrentCommand_KeyName, command, (key, old) => command);
            await this.StateManager.AddOrUpdateStateAsync(CallbackServiceUri_KeyName, callbackServiceUri, (key, old) => callbackServiceUri);
        }

        private async Task WorkCommand(object state)
        {
            this.WriteEnterMethod();

            var command = await this.StateManager.TryGetStateAsync<Command>(CurrentCommand_KeyName);
            var callbackServiceUri = await this.StateManager.TryGetStateAsync<Uri>(CallbackServiceUri_KeyName);

            if (!command.HasValue)
            {
                this.WriteVerbose("There are not commands in queue to process. Deactivate timer");
                //Se non ho niente da lavorare rimuovo il timer che verrà
                //riattivato dall'invocazione del metodo ExecuteAsync
                await this.DeactivateWorkCommandReminderAsync();
                return;
            }

            if (await ExecuteCommandInternal(command.Value))
            {
                this.WriteVerbose("ExecuteCommandInternal return true. Remove command from state");
                await this.StateManager.RemoveStateAsync(CurrentCommand_KeyName);
                await this.StateManager.RemoveStateAsync(CallbackServiceUri_KeyName);

                if (callbackServiceUri.HasValue)
                {
                    var proxy = ActorProxy.Create<IProcessorCallback>(this.Id, callbackServiceUri.Value);
                    await proxy.ProcessingCompleteCallBackAsync();
                }
            }
        }

        protected abstract Task<bool> ExecuteCommandInternal(Command command);

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

        protected async Task UpdateCommand(Command command)
        {
            this.WriteEnterMethod();
            await this.StateManager.SetStateAsync(CurrentCommand_KeyName, command);
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (string.Equals(reminderName, WorkCommand_ReminderName))
            {
                await this.WorkCommand(null);
            }
        }

    }
}
