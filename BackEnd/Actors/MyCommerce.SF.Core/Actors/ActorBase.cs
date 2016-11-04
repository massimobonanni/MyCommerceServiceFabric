using System;
using System.Fabric.Description;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using MyCommerce.SF.Core.Interfaces;
using MyCommerce.SF.Core.Utilities;
using MyCommerce.SF.Core.Tracing;
using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;

namespace MyCommerce.SF.Core.Actors
{
    public abstract class ActorBase : Actor
    {

        private readonly bool _testMode = false;

        protected readonly IActorFactory ActorFactory;
        protected readonly IServiceFactory ServiceFactory;

        public ActorBase(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            _testMode = false;
            _stateManager = null;
            var reliableFactory = new ReliableFactory();

            ActorFactory = reliableFactory;
            ServiceFactory = reliableFactory;
        }

        public ActorBase(ActorService actorService, ActorId actorId, IActorStateManager stateManager,
            IActorFactory actorFactory, IServiceFactory serviceFactory) : base(actorService, actorId)
        {
            _testMode = true;
            _stateManager = stateManager;
            var reliableFactory = (actorFactory == null || serviceFactory == null) ? new ReliableFactory() : null;

            ActorFactory = actorFactory ?? reliableFactory;
            ServiceFactory = serviceFactory ?? reliableFactory;
        }

        private readonly IActorStateManager _stateManager;

        public new IActorStateManager StateManager
        {
            get
            {
                if (_stateManager != null) return _stateManager;
                return base.StateManager;
            }
        }

        protected virtual string ConfigurationSection
        {
            get
            {
                return this.GetType().Name + "Section";
            }
        }

        protected string GetReminderIsRegisteredStateName(string reminderName)
        {
            return $"ReminderState_{reminderName}";
        }

        protected string ReadSetting(string setting)
        {
            ConfigurationSettings settings = this.ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings;
            ConfigurationSection eventSourceSection = settings.Sections[this.ConfigurationSection];
            return eventSourceSection.Parameters[setting].Value;
        }

        #region Reminders
        protected async Task ActivateWorkCommandRiminderSafeAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            string reminderIsRegisteredStateName = GetReminderIsRegisteredStateName(reminderName);
            var reminderIsRegisterd = await this.StateManager.TryGetStateAsync<bool>(reminderIsRegisteredStateName);

            if (!reminderIsRegisterd.HasValue || reminderIsRegisterd.Value == false)
            {
                await this.StateManager.SetStateAsync(reminderIsRegisteredStateName, true);
                await this.RegisterReminderAsync(reminderName, null, dueTime, period);
            }
        }

        protected async Task DeactivateWorkCommandReminderSafeAsync(string reminderName)
        {
            string reminderIsRegisteredStateName = GetReminderIsRegisteredStateName(reminderName);
            var reminderIsRegisterd = await this.StateManager.TryGetStateAsync<bool>(reminderIsRegisteredStateName);

            if (reminderIsRegisterd.HasValue && reminderIsRegisterd.Value == true)
            {
                var reminder = this.GetReminder(reminderName);
                await this.StateManager.RemoveStateAsync(reminderIsRegisteredStateName);
                await this.UnregisterReminderAsync(reminder);
            }
        }

        protected new Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (!_testMode) return base.RegisterReminderAsync(reminderName, state, dueTime, period);
            return Task.FromResult<IActorReminder>(null);
        }

        protected new IActorTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
        {
            if (!_testMode) return base.RegisterTimer(asyncCallback, state, dueTime, period);
            return null;
        }

        protected new Task UnregisterReminderAsync(IActorReminder reminder)
        {
            if (!_testMode) return base.UnregisterReminderAsync(reminder);
            return Task.Delay(0);
        }

        protected new void UnregisterTimer(IActorTimer timer)
        {
            if (!_testMode) base.UnregisterTimer(timer);
        }
        #endregion

        #region Logging
        protected virtual void WriteEnterMethod([CallerMemberName] string callerMemberName = null)
        {
            ActorEventSource.Current.Method(this, null, $"Enter {callerMemberName}");
        }

        protected virtual void WriteExitMethod([CallerMemberName] string callerMemberName = null)
        {
            ActorEventSource.Current.Method(this, null, $"Exit {callerMemberName}");
        }

        protected virtual void WriteVerbose(string message, params object[] args)
        {
            ActorEventSource.Current.Message(this, EventLevel.Verbose, message, args);
        }

        protected virtual void WriteException(Exception exception, string message, params object[] args)
        {
            ActorEventSource.Current.Message(this, EventLevel.Error, $"{string.Format(message, args)}:{exception.ToString()}", args);
        }

        protected virtual void WriteError(string message, params object[] args)
        {
            ActorEventSource.Current.Message(this, EventLevel.Error, message, args);
        }
        #endregion
    }
}
