using System;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace MyCommerce.SF.Core.Tracing
{
    [EventSource(Name = "MyCommerce-Services-Actors")]
    public sealed class ActorEventSource : EventSourceBase
    {
        public static readonly ActorEventSource Current = new ActorEventSource();

        private const int ActorMessageEventIdVerbose = 1;
        private const int ActorMessageEventIdInformational = 2;
        private const int ActorMessageEventIdWarning = 3;
        private const int ActorMessageEventIdError = 4;
        private const int ActorTypeRegisteredEventId = 5;
        private const int ActorHostInitializationEventIdFailed = 6;
        private const int ActorMessageEventIdPerformance = 11;
        private const int ActorMessageEventIdHealth = 12;

        static ActorEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { }).Wait();
        }

        // Instance constructor is private to enforce singleton semantics
        private ActorEventSource() : base() { }

        #region Keywords
        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords HostInitialization = (EventKeywords)0x1L;
            public const EventKeywords Log = (EventKeywords)2;
            public const EventKeywords Performance = (EventKeywords)4;
            public const EventKeywords Health = (EventKeywords)8;
        }
        #endregion

        #region Events
        // Define an instance method for each event you want to record and apply an [Event] attribute to it.
        // The method name is the name of the event.
        // Pass any parameters you want to record with the event (only primitive integer types, DateTime, Guid & string are allowed).
        // Each event method implementation should check whether the event source is enabled, and if it is, call WriteEvent() method to raise the event.
        // The number and types of arguments passed to every event method must exactly match what is passed to WriteEvent().
        // Put [NonEvent] attribute on all methods that do not define an event.
        // For more information see https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventsource.aspx

        [NonEvent]
        public void Message(Actor actor, EventLevel level, string message, params object[] args)
        {
            this.Message(actor, level, null, message, args);
        }

        [NonEvent]
        public void Message(Actor actor, EventLevel level, string correlationId, string message, params object[] args)
        {
            if (this.IsEnabled() && actor != null
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                string finalMessage = message;

                if (args != null)
                {
                    finalMessage = string.Format(message, args);
                }

                switch (level)
                {
                    case EventLevel.LogAlways:
                        break;
                    case EventLevel.Critical:
                    case EventLevel.Error:
                        this.MessageError(actor.GetType().ToString(),
                            actor.Id.ToString(),
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                            actor.ActorService.Context.ServiceTypeName,
                            actor.ActorService.Context.ServiceName.ToString(),
                            actor.ActorService.Context.PartitionId,
                            actor.ActorService.Context.ReplicaId,
                            actor.ActorService.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Warning:
                        this.MessageWarning(actor.GetType().ToString(),
                            actor.Id.ToString(),
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                            actor.ActorService.Context.ServiceTypeName,
                            actor.ActorService.Context.ServiceName.ToString(),
                            actor.ActorService.Context.PartitionId,
                            actor.ActorService.Context.ReplicaId,
                            actor.ActorService.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Informational:
                        this.MessageInformational(actor.GetType().ToString(),
                            actor.Id.ToString(),
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                            actor.ActorService.Context.ServiceTypeName,
                            actor.ActorService.Context.ServiceName.ToString(),
                            actor.ActorService.Context.PartitionId,
                            actor.ActorService.Context.ReplicaId,
                            actor.ActorService.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Verbose:
                        this.MessageVerbose(actor.GetType().ToString(),
                            actor.Id.ToString(),
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                            actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                            actor.ActorService.Context.ServiceTypeName,
                            actor.ActorService.Context.ServiceName.ToString(),
                            actor.ActorService.Context.PartitionId,
                            actor.ActorService.Context.ReplicaId,
                            actor.ActorService.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                }
            }
        }

        [NonEvent]
        public void Message(Actor actor, Exception ex, string message, params object[] args)
        {
            this.Message(actor, null, ex, message, args);
        }

        [NonEvent]
        public void Message(Actor actor, string correlationId, Exception ex, string message, params object[] args)
        {
            if (this.IsEnabled() && actor != null
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                string finalMessage = message;

                if (args != null)
                {
                    finalMessage = string.Format(message, args);
                }

                MessageError(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    finalMessage,
                    ex.ToString(),
                    correlationId);
            }
        }

        [NonEvent]
        public void Method(Actor actor, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            Message(actor, EventLevel.Verbose, correlationId, "Method {0}::{1}", actor, callerMemberName);
        }

        [NonEvent]
        public void Performance(Actor actor, PerformanceActions actionType, string callerMemberName, string performanceId = null)
        {
            if (this.IsEnabled() && actor != null
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                Performance(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    actionType.ToString(),
                    callerMemberName,
                    performanceId);
            }
        }

        [NonEvent]
        public void Health(Actor actor, string key, string value, string message = null)
        {
            if (this.IsEnabled() && actor != null
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                Health(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    key,
                    value,
                    message ?? string.Empty);
            }
        }


        // For very high-frequency events it might be advantageous to raise events using WriteEventCore API.
        // This results in more efficient parameter handling, but requires explicit allocation of EventData structure and unsafe code.
        // To enable this code path, define UNSAFE conditional compilation symbol and turn on unsafe code support in project properties.

        [Event(ActorMessageEventIdVerbose, Level = EventLevel.Verbose, Message = "{9}", Keywords = Keywords.Log)]
        private
#if UNSAFE
            unsafe
#endif
            void MessageVerbose(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string message,
                string exception,
                string correlationId)
        {
            this.MessageInternal(ActorMessageEventIdVerbose,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                message,
                exception,
                correlationId
            );
        }

        [Event(ActorMessageEventIdInformational, Level = EventLevel.Informational, Message = "{9}", Keywords = Keywords.Log)]
        private
#if UNSAFE
            unsafe
#endif
            void MessageInformational(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string message,
                string exception,
                string correlationId)
        {
            this.MessageInternal(ActorMessageEventIdInformational,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                message,
                exception,
                correlationId
            );
        }

        [Event(ActorMessageEventIdWarning, Level = EventLevel.Warning, Message = "{9}", Keywords = Keywords.Log)]
        private
#if UNSAFE
            unsafe
#endif
            void MessageWarning(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string message,
                string exception,
                string correlationId)
        {
            this.MessageInternal(ActorMessageEventIdWarning,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                message,
                exception,
                correlationId
            );
        }

        [Event(ActorMessageEventIdError, Level = EventLevel.Error, Message = "{9}", Keywords = Keywords.Log)]
        private
#if UNSAFE
            unsafe
#endif
            void MessageError(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string message,
                string exception,
                string correlationId)
        {
            this.MessageInternal(ActorMessageEventIdError,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                message,
                exception,
                correlationId
            );
        }

        [Event(ActorMessageEventIdPerformance, Level = EventLevel.Informational, Message = "Performance CallerMemberName:{10}, ActionType:{9}", Keywords = Keywords.Performance)]
        private
#if UNSAFE
            unsafe
#endif
            void Performance(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string actionType,
                string callerMemberName,
                string performanceId)
        {
            this.PerformanceInternal(ActorMessageEventIdPerformance,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                actionType,
                callerMemberName,
                performanceId
            );
        }

        [Event(ActorMessageEventIdHealth, Level = EventLevel.LogAlways, Message = "Health Key:{9}, Value:{10}", Keywords = Keywords.Health)]
        private
#if UNSAFE
            unsafe
#endif
            void Health(
                string actorType,
                string actorId,
                string applicationTypeName,
                string applicationName,
                string serviceTypeName,
                string serviceName,
                Guid partitionId,
                long replicaOrInstanceId,
                string nodeName,
                string key,
                string value,
                string message)
        {
            this.HealthInternal(ActorMessageEventIdHealth,
                actorType,
                actorId,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                key,
                value,
                message
            );
        }

        [Event(ActorTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Actor host process {0} registered service type {1}", Keywords = Keywords.HostInitialization)]
        public void ActorTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(ActorTypeRegisteredEventId, hostProcessId, serviceType);
        }


        [Event(ActorHostInitializationEventIdFailed, Level = EventLevel.Error, Message = "Actor host initialization failed", Keywords = Keywords.HostInitialization)]
        public void ActorHostInitializationFailed(string exception)
        {
            WriteEvent(ActorHostInitializationEventIdFailed, exception);
        }
        #endregion
    }

    public static class LoggingActorExtension
    {
        public static void TraceVerbose(this Actor actor, string correlationId, string message, params object[] args)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Verbose, correlationId, message, args);
        }

        public static void TraceInformational(this Actor actor, string correlationId, string message, params object[] args)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Informational, correlationId, message, args);
        }

        public static void TraceWarning(this Actor actor, string correlationId, string message, params object[] args)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Warning, correlationId, message, args);
        }

        public static void TraceError(this Actor actor, string correlationId, string message, params object[] args)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Error, correlationId, message, args);
        }

        public static void TraceException(this Actor actor, string correlationId, Exception exception, string message, params object[] args)
        {
            ActorEventSource.Current.Message(actor, correlationId, exception, message, args);
        }

        public static void TraceMethod(this Actor actor, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Verbose, correlationId, "Method {0}::{1}", actor, callerMemberName);
        }

        public static void TraceMethodEnter(this Actor actor, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}", actor, callerMemberName);
        }

        public static void TraceMethodEnterWithParams(this Actor actor, string correlationId, string paramsDescription, [CallerMemberName] string callerMemberName = "")
        {
            ActorEventSource.Current.Message(actor, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}({2})", actor, callerMemberName, paramsDescription);
        }

        public static void TraceMethodExit(this Actor actor, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ActorEventSource.Current.Message(actor, EventLevel.Verbose, correlationId, "(exit) <-- {0}::{1}", actor, callerMemberName);
        }

        public static void MethodStart(this Actor actor, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ActorEventSource.Current.Performance(actor, PerformanceActions.Start, callerMemberName, performanceId);
        }

        public static void MethodStop(this Actor actor, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ActorEventSource.Current.Performance(actor, PerformanceActions.Stop, callerMemberName, performanceId);
        }

        public static void SendHealth(this Actor actor, string key, string value, string message = null)
        {
            ActorEventSource.Current.Health(actor, key, value, message);
        }
    }
}

