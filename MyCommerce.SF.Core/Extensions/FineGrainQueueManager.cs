using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace MyCommerce.SF.Core.Extensions
{
    public static class FineGrainQueueManager
    {
        private static object _criticalSection = new object();

        public static Task EnqueueAsync<TElement>(this IActorStateManager state, string queueName, TElement element)
        {
            var elementId = Guid.NewGuid();
            var queueStateName = GetQueueStateName(queueName);

            lock (_criticalSection)
            {
                var queue = state.GetOrAddStateAsync<Queue<Guid>>(queueStateName, new Queue<Guid>()).GetAwaiter().GetResult();
                queue.Enqueue(elementId);

                state.SetStateAsync<TElement>(elementId.ToString(), element).GetAwaiter().GetResult();
                return state.SetStateAsync<Queue<Guid>>(queueName, queue);
            }
        }

        public static Task<TElement> DequeueAsync<TElement>(this IActorStateManager state, string queueName)
        {
            TElement result = default(TElement);
            var queueStateName = GetQueueStateName(queueName);

            lock (_criticalSection)
            {
                var queue = state.GetOrAddStateAsync<Queue<Guid>>(queueStateName, new Queue<Guid>()).GetAwaiter().GetResult();

                if (queue.Count == 0)
                    return Task.FromResult(result);

                var elementId = queue.Dequeue().ToString();

                var element = state.TryGetStateAsync<TElement>(elementId).GetAwaiter().GetResult();
                if (element.HasValue)
                {
                    state.TryRemoveStateAsync(elementId).GetAwaiter().GetResult();
                    result = element.Value;
                }

                state.SetStateAsync<Queue<Guid>>(queueName, queue).GetAwaiter().GetResult();
            }

            return Task.FromResult(result);
        }

        public static Task<bool> PurgeQueue(this IActorStateManager state, string queueName)
        {
            var queueStateName = GetQueueStateName(queueName);

            lock (_criticalSection)
            {
                var queue = state.GetOrAddStateAsync<Queue<Guid>>(queueStateName, new Queue<Guid>()).GetAwaiter().GetResult();

                while (queue.Count > 0)
                {
                    var elementId = queue.Dequeue().ToString();
                    state.TryRemoveStateAsync(elementId).GetAwaiter().GetResult();
                }

                return state.TryRemoveStateAsync(queueName);
            }
        }

        public static async Task<int> GetQueueLengthAsync(this IActorStateManager state, string queueName)
        {
            var queueStateName = GetQueueStateName(queueName);

            var queue = await state.GetOrAddStateAsync<Queue<Guid>>(queueStateName, new Queue<Guid>());
            return queue.Count;
        }

        private static string GetQueueStateName(string queueName)
        {
            return $"{typeof(FineGrainQueueManager).Name}.Queue({queueName})";
        }
    }
}
