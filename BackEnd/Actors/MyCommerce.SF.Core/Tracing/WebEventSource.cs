using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyCommerce.SF.Core.Tracing
{
    [EventSource(Name = "MyCommerce-Services-Web")]
    internal sealed class WebEventSource : EventSourceBase
    {
        public static readonly WebEventSource Current = new WebEventSource();

        private const int WebMessageEventIdVerbose = 1;
        private const int WebMessageEventIdInformational = 2;
        private const int WebMessageEventIdWarning = 3;
        private const int WebMessageEventIdError = 4;
        private const int WebHostInitializationEventIdFailed = 5;
        private const int WebTypeRegisteredEventId = 6;
        private const int WebHostInitializationFailedEventId = 7;
        private const int WebRequestStartEventId = 8;
        private const int WebRequestStopEventId = 9;
        private const int WebRequestFailedEventId = 10;
        private const int WebMessageEventIdPerformance = 11;
        private const int WebMessageEventIdHealth = 12;

        static WebEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { }).Wait();
        }

        // Instance constructor is private to enforce singleton semantics
        private WebEventSource() : base() { }

        #region Keywords
        // Event keywords can be used to categorize events.
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords ServiceInitialization = (EventKeywords)1;
            public const EventKeywords Log = (EventKeywords)2;
            public const EventKeywords Performance = (EventKeywords)4;
            public const EventKeywords Health = (EventKeywords)8;
            public const EventKeywords Requests = (EventKeywords)16;
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
        public void Message(ServiceContext service, EventLevel level, string message, params object[] args)
        {
            this.Message(service, level, null, message, args);
        }

        [NonEvent]
        public void Message(ServiceContext service, EventLevel level, string correlationId, string message, params object[] args)
        {
            if (this.IsEnabled())
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
                        this.MessageError(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Warning:
                        this.MessageWarning(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Informational:
                        this.MessageInformational(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Verbose:
                        this.MessageVerbose(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                }
            }
        }

        [NonEvent]
        public void Message(ServiceContext service, Exception ex, string message, params object[] args)
        {
            this.Message(service, null, ex, message, args);
        }

        [NonEvent]
        public void Message(ServiceContext service, string correlationId, Exception ex, string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                MessageError(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            finalMessage,
                            ex.ToString(),
                            correlationId);
            }
        }

        [NonEvent]
        public void Performance(ServiceContext service, PerformanceActions actionType, string callerMemberName, string performanceId = null)
        {
            if (this.IsEnabled())
            {
                Performance(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            actionType.ToString(),
                            callerMemberName,
                            performanceId);
            }
        }

        [NonEvent]
        public void Health(ServiceContext service, string key, string value, string message = null)
        {
            if (this.IsEnabled())
            {
                Health(string.Empty, string.Empty,
                            service.CodePackageActivationContext.ApplicationTypeName,
                            service.CodePackageActivationContext.ApplicationName,
                            service.ServiceTypeName,
                            service.ServiceName.ToString(),
                            service.PartitionId,
                            service.ReplicaOrInstanceId,
                            service.NodeContext.NodeName,
                            key,
                            value,
                            message ?? string.Empty);
            }
        }

        // For very high-frequency events it might be advantageous to raise events using WriteEventCore API.
        // This results in more efficient parameter handling, but requires explicit allocation of EventData structure and unsafe code.
        // To enable this code path, define UNSAFE conditional compilation symbol and turn on unsafe code support in project properties.

        [Event(WebMessageEventIdVerbose, Level = EventLevel.Verbose, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(WebMessageEventIdVerbose,
                string.Empty,
                string.Empty,
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

        [Event(WebMessageEventIdInformational, Level = EventLevel.Informational, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(WebMessageEventIdInformational,
                string.Empty,
                string.Empty,
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

        [Event(WebMessageEventIdWarning, Level = EventLevel.Warning, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(WebMessageEventIdWarning,
                string.Empty,
                string.Empty,
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

        [Event(WebMessageEventIdError, Level = EventLevel.Error, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(WebMessageEventIdError,
                string.Empty,
                string.Empty,
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

        [Event(WebMessageEventIdPerformance, Level = EventLevel.Verbose, Message = "Performance CallerMemberName:{8}, ActionType:{7}", Keywords = Keywords.Performance)]
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
            this.PerformanceInternal(WebMessageEventIdPerformance,
                string.Empty,
                string.Empty,
                applicationTypeName,
                applicationName,
                serviceTypeName,
                serviceName,
                partitionId,
                replicaOrInstanceId,
                nodeName,
                actionType,
                callerMemberName,
                performanceId);
        }

        [Event(WebMessageEventIdHealth, Level = EventLevel.Verbose, Message = "Health Key:{7}, Value:{8}", Keywords = Keywords.Health)]
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
            this.HealthInternal(WebMessageEventIdHealth,
                string.Empty,
                string.Empty,
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

        [Event(WebTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceInitialization)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(WebTypeRegisteredEventId, hostProcessId, serviceType);
        }


        [Event(WebHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceInitialization)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(WebHostInitializationFailedEventId, exception);
        }

        // A pair of events sharing the same name prefix with a "Start"/"Stop" suffix implicitly marks boundaries of an event tracing activity.
        // These activities can be automatically picked up by debugging and profiling tools, which can compute their execution time, child activities,
        // and other statistics.

        [Event(WebRequestStartEventId, Level = EventLevel.Informational, Message = "Service request '{0}' started", Keywords = Keywords.Requests)]
        public void ServiceRequestStart(string requestTypeName)
        {
            WriteEvent(WebRequestStartEventId, requestTypeName);
        }


        [Event(WebRequestStopEventId, Level = EventLevel.Informational, Message = "Service request '{0}' finished", Keywords = Keywords.Requests)]
        public void ServiceRequestStop(string requestTypeName)
        {
            WriteEvent(WebRequestStopEventId, requestTypeName);
        }


        [Event(WebRequestFailedEventId, Level = EventLevel.Error, Message = "Service request '{0}' failed", Keywords = Keywords.Requests)]
        public void ServiceRequestFailed(string requestTypeName, string exception)
        {
            WriteEvent(WebRequestFailedEventId, exception);
        }
        #endregion

        #region Private methods

        private static long GetReplicaOrInstanceId(ServiceContext context)
        {
            StatelessServiceContext stateless = context as StatelessServiceContext;
            if (stateless != null)
            {
                return stateless.InstanceId;
            }

            StatefulServiceContext stateful = context as StatefulServiceContext;
            if (stateful != null)
            {
                return stateful.ReplicaId;
            }

            throw new NotSupportedException("Context type not supported.");
        }
        #endregion
    }

    public static class LoggingWebExtension
    {
        public static void WriteVerbose(this ServiceContext service, string correlationId, string message, params object[] args)
        {
            WebEventSource.Current.Message(service, EventLevel.Verbose, correlationId, message, args);
        }

        public static void WriteInformational(this ServiceContext service, string correlationId, string message, params object[] args)
        {
            WebEventSource.Current.Message(service, EventLevel.Informational, correlationId, message, args);
        }

        public static void WriteWarning(this ServiceContext service, string correlationId, string message, params object[] args)
        {
            WebEventSource.Current.Message(service, EventLevel.Warning, correlationId, message, args);
        }

        public static void WriteError(this ServiceContext service, string correlationId, string message, params object[] args)
        {
            WebEventSource.Current.Message(service, EventLevel.Error, correlationId, message, args);
        }

        public static void WriteException(this ServiceContext service, string correlationId, Exception exception, string message, params object[] args)
        {
            WebEventSource.Current.Message(service, correlationId, exception, message, args);
        }

        public static void WriteEnterMethod(this ServiceContext service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            WebEventSource.Current.Message(service, EventLevel.Verbose, correlationId, $"Enter {callerMemberName}");
        }

        public static void WriteExitMethod(this ServiceContext service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            WebEventSource.Current.Message(service, EventLevel.Verbose, correlationId, $"Exit {callerMemberName}");
        }

        public static void MethodStart(this ServiceContext service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            WebEventSource.Current.Performance(service, PerformanceActions.Start, callerMemberName, performanceId);
        }

        public static void MethodStop(this ServiceContext service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            WebEventSource.Current.Performance(service, PerformanceActions.Stop, callerMemberName, performanceId);
        }

        public static void SendHealth(this ServiceContext service, string key, string value, string message = null)
        {
            WebEventSource.Current.Health(service, key, value, message);
        }
    }
}
