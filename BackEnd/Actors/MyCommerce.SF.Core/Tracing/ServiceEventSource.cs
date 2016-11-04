using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Runtime.CompilerServices;
using Microsoft.ServiceFabric.Services.Runtime;

namespace MyCommerce.SF.Core.Tracing
{
    [EventSource(Name = "MyCommerce-Services-Services")]
    public sealed class ServiceEventSource : EventSourceBase
    {
        public static ServiceEventSource Current = new ServiceEventSource();

        private const int ServiceMessageEventIdVerbose = 1;
        private const int ServiceMessageEventIdInformational = 2;
        private const int ServiceMessageEventIdWarning = 3;
        private const int ServiceMessageEventIdError = 4;
        private const int ServiceHostInitializationEventIdFailed = 5;
        private const int ServiceTypeRegisteredEventId = 6;
        private const int ServiceHostInitializationFailedEventId = 7;
        private const int ServiceRequestStartEventId = 8;
        private const int ServiceRequestStopEventId = 9;
        private const int ServiceRequestFailedEventId = 10;
        private const int ServiceMessageEventIdPerformance = 11;
        private const int ServiceMessageEventIdHealth = 12;

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
        public void Message(StatelessService service, EventLevel level, string message, params object[] args)
        {
            this.Message(service, level, null, message, args);
        }

        [NonEvent]
        public void Message(StatefulService service, EventLevel level, string message, params object[] args)
        {
            this.Message(service, level, null, message, args);
        }

        [NonEvent]
        public void Message(StatelessService service, EventLevel level, string correlationId, string message, params object[] args)
        {
            if (this.IsEnabled() && service != null)
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
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.InstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Warning:
                        this.MessageWarning(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.InstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Informational:
                        this.MessageInformational(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.InstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Verbose:
                        this.MessageVerbose(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.InstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                }
            }
        }

        [NonEvent]
        public void Message(StatefulService service, EventLevel level, string correlationId, string message, params object[] args)
        {
            if (this.IsEnabled() && service != null)
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
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Warning:
                        this.MessageWarning(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Informational:
                        this.MessageInformational(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                    case EventLevel.Verbose:
                        this.MessageVerbose(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, string.Empty, correlationId);
                        break;
                }
            }
        }

        [NonEvent]
        public void Message(StatelessService service, Exception ex, string message, params object[] args)
        {
            this.Message(service, null, ex, message, args);
        }

        [NonEvent]
        public void Method(StatelessService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            this.Message(service, EventLevel.Verbose, correlationId, $"Method {callerMemberName}");
        }

        [NonEvent]
        public void Message(StatefulService service, Exception ex, string message, params object[] args)
        {
            this.Message(service, null, ex, message, args);
        }

        public void Message(ServiceContext serviceContext, string correlationId, string message, params object[] args)
        {
            if (this.IsEnabled() && serviceContext != null)
            {
                string finalMessage = string.Format(message, args);
                this.MessageVerbose(string.Empty, string.Empty,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.ToString(),
                    serviceContext.PartitionId,
                    serviceContext.ReplicaOrInstanceId,
                    serviceContext.NodeContext.NodeName,
                    finalMessage, string.Empty, correlationId);
            }
        }

        [NonEvent]
        public void Message(StatelessService service, string correlationId, Exception ex, string message, params object[] args)
        {
            if (this.IsEnabled() && service != null)
            {
                string finalMessage = string.Format(message, args);
                MessageError(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage, 
                            ex.ToString(), 
                            correlationId);
            }
        }

        [NonEvent]
        public void Message(StatefulService service, string correlationId, Exception ex, string message, params object[] args)
        {
            if (this.IsEnabled() && service != null)
            {
                string finalMessage = string.Format(message, args);
                MessageError(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            finalMessage,
                            ex.ToString(),
                            correlationId);
            }
        }

        [NonEvent]
        public void Method(StatefulService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            this.Message(service, EventLevel.Verbose, correlationId, $"Method {callerMemberName}");
        }

        [NonEvent]
        public void Performance(StatelessService service, PerformanceActions actionType, string callerMemberName, string performanceId = null)
        {
            if (this.IsEnabled())
            {
                Performance(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            actionType.ToString(),
                            callerMemberName,
                            performanceId);
            }
        }

        [NonEvent]
        public void Performance(StatefulService service, PerformanceActions actionType, string callerMemberName, string performanceId = null)
        {
            if (this.IsEnabled() && service != null)
            {
                Performance(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            actionType.ToString(),
                            callerMemberName,
                            performanceId);
            }
        }

        [NonEvent]
        public void Health(StatelessService service, string key, string value, string message = null)
        {
            if (this.IsEnabled() && service != null)
            {
                Health(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            key,
                            value,
                            message ?? string.Empty);
            }
        }

        [NonEvent]
        public void Health(StatefulService service, string key, string value, string message = null)
        {
            if (this.IsEnabled() && service != null)
            {
                Health(string.Empty, string.Empty,
                            service.Context.CodePackageActivationContext.ApplicationTypeName,
                            service.Context.CodePackageActivationContext.ApplicationName,
                            service.Context.ServiceTypeName,
                            service.Context.ServiceName.ToString(),
                            service.Context.PartitionId,
                            service.Context.ReplicaOrInstanceId,
                            service.Context.NodeContext.NodeName,
                            key,
                            value,
                            message ?? string.Empty);
            }
        }

        // For very high-frequency events it might be advantageous to raise events using WriteEventCore API.
        // This results in more efficient parameter handling, but requires explicit allocation of EventData structure and unsafe code.
        // To enable this code path, define UNSAFE conditional compilation symbol and turn on unsafe code support in project properties.

        [Event(ServiceMessageEventIdVerbose, Level = EventLevel.Verbose, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(ServiceMessageEventIdVerbose,
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

        [Event(ServiceMessageEventIdInformational, Level = EventLevel.Informational, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(ServiceMessageEventIdInformational,
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

        [Event(ServiceMessageEventIdWarning, Level = EventLevel.Warning, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(ServiceMessageEventIdWarning,
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

        [Event(ServiceMessageEventIdError, Level = EventLevel.Error, Message = "{7}", Keywords = Keywords.Log)]
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
            this.MessageInternal(ServiceMessageEventIdError,
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

        [Event(ServiceMessageEventIdPerformance, Level = EventLevel.Verbose, Message = "Performance CallerMemberName:{8}, ActionType:{7}", Keywords = Keywords.Performance)]
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
            this.PerformanceInternal(ServiceMessageEventIdPerformance,
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

        [Event(ServiceMessageEventIdHealth, Level = EventLevel.Verbose, Message = "Health Key:{7}, Value:{8}", Keywords = Keywords.Health)]
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
            this.HealthInternal(ServiceMessageEventIdHealth,
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

        [Event(ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceInitialization)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(ServiceTypeRegisteredEventId, hostProcessId, serviceType);
        }

        [Event(ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceInitialization)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(ServiceHostInitializationFailedEventId, exception);
        }

        // A pair of events sharing the same name prefix with a "Start"/"Stop" suffix implicitly marks boundaries of an event tracing activity.
        // These activities can be automatically picked up by debugging and profiling tools, which can compute their execution time, child activities,
        // and other statistics.
        [Event(ServiceRequestStartEventId, Level = EventLevel.Informational, Message = "Service request '{0}' started", Keywords = Keywords.Requests)]
        public void ServiceRequestStart(string requestTypeName)
        {
            WriteEvent(ServiceRequestStartEventId, requestTypeName);
        }

        [Event(ServiceRequestStopEventId, Level = EventLevel.Informational, Message = "Service request '{0}' finished", Keywords = Keywords.Requests)]
        public void ServiceRequestStop(string requestTypeName)
        {
            WriteEvent(ServiceRequestStopEventId, requestTypeName);
        }

        [Event(ServiceRequestFailedEventId, Level = EventLevel.Error, Message = "Service request '{0}' failed", Keywords = Keywords.Requests)]
        public void ServiceRequestFailed(string requestTypeName, string exception)
        {
            WriteEvent(ServiceRequestFailedEventId, exception);
        }
        #endregion
    }

    public static class LoggingServiceStatelessExtension
    {
        public static void TraceVerbose(this StatelessService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, message, args);
        }

        public static void TraceInformational(this StatelessService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Informational, correlationId, message, args);
        }

        public static void TraceInformational(this StatelessService service, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Informational, service.Context.ReplicaOrInstanceId.ToString(), message, args);
        }

        public static void TraceWarning(this StatelessService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Warning, correlationId, message, args);
        }

        public static void TraceError(this StatelessService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Error, correlationId, message, args);
        }

        public static void TraceException(this StatelessService service, string correlationId, Exception exception, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, correlationId, exception, message, args);
        }

        public static void TraceMethod(this StatelessService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "Method {0}::{1}", service, callerMemberName);
        }

        public static void TraceMethodEnter(this StatelessService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}", service, callerMemberName);
        }

        public static void TraceMethodEnterWithParams(this StatelessService service, string correlationId, string paramsDescription, [CallerMemberName] string callerMemberName = "")
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}({2})", service, callerMemberName, paramsDescription);
        }

        public static void TraceMethodExit(this StatelessService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(exit) <-- {0}::{1}", service, callerMemberName);
        }

        public static void MethodStart(this StatelessService service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Performance(service, PerformanceActions.Start, callerMemberName, performanceId);
        }

        public static void MethodStop(this StatelessService service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Performance(service, PerformanceActions.Stop, callerMemberName, performanceId);
        }

        public static void SendHealth(this StatelessService service, string key, string value, string message = null)
        {
            ServiceEventSource.Current.Health(service, key, value, message);
        }
    }

    public static class LoggingServiceStatefulExtension
    {
        public static void TraceVerbose(this StatefulService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, message, args);
        }

        public static void TraceInformational(this StatefulService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Informational, correlationId, message, args);
        }

        public static void TraceWarning(this StatefulService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Warning, correlationId, message, args);
        }

        public static void TraceError(this StatefulService service, string correlationId, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Error, correlationId, message, args);
        }

        public static void TraceException(this StatefulService service, string correlationId, Exception exception, string message, params object[] args)
        {
            ServiceEventSource.Current.Message(service, correlationId, exception, message, args);
        }

        public static void TraceMethod(this StatefulService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "Method {0}::{1}", service, callerMemberName);
        }

        public static void TraceMethodEnter(this StatefulService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}", service, callerMemberName);
        }

        public static void TraceMethodEnterWithParams(this StatefulService service, string correlationId, string paramsDescription, [CallerMemberName] string callerMemberName = "")
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(enter) --> {0}::{1}({2})", service, callerMemberName, paramsDescription);
        }

        public static void TraceMethodExit(this StatefulService service, string correlationId, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Message(service, EventLevel.Verbose, correlationId, "(exit) <-- {0}::{1}", service, callerMemberName);
        }

        public static void MethodStart(this StatefulService service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Performance(service, PerformanceActions.Start, callerMemberName, performanceId);
        }

        public static void MethodStop(this StatefulService service, string performanceId = null, [CallerMemberName]string callerMemberName = null)
        {
            ServiceEventSource.Current.Performance(service, PerformanceActions.Stop, callerMemberName, performanceId);
        }

        public static void SendHealth(this StatefulService service, string key, string value, string message = null)
        {
            ServiceEventSource.Current.Health(service, key, value, message);
        }
    }
}