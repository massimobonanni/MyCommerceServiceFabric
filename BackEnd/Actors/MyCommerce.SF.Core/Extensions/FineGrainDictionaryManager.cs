using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace MyCommerce.SF.Core.Extensions
{
    public static class FineGrainDictionaryManager
    {
        private static object _criticalSection = new object();

        public static Task<bool> TryAddToDictionaryAsync<TKey, TValue>(this IActorStateManager state, string dictionaryName, TKey key, TValue value)
        {
            var result = false;
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);

            lock (_criticalSection)
            {
                var id = Guid.NewGuid();

                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();

                result = dict.TryAdd(key, id);
                if (result)
                {
                    var itemStateName = GetItemStateName(id.ToString());
                    state.SetStateAsync<TValue>(itemStateName, value).GetAwaiter().GetResult();

                    state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, dict).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(result);
        }

        public static Task<bool> TryAddOrUpdateDictionaryAsync<TKey, TValue>(this IActorStateManager state, string dictionaryName, TKey key, TValue value)
        {
            var result = false;
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                Guid id = Guid.Empty;

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out id);

                if (result)
                {
                    var itemStateName = GetItemStateName(id.ToString());
                    state.SetStateAsync<TValue>(itemStateName, value).GetAwaiter().GetResult();

                    state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, dict).GetAwaiter().GetResult();
                }
                else
                {
                    result = state.TryAddToDictionaryAsync<TKey, TValue>(dictionaryName, key, value).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(result);
        }

        public static Task<ConditionalValue<TValue>> TryRemoveFromDictionaryAsync<TKey, TValue>(this IActorStateManager state, string dictionaryName, TKey key)
        {
            ConditionalValue<TValue> value = new ConditionalValue<TValue>(false, default(TValue));
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    Guid id;

                    if (dict.TryRemove(key, out id))
                    {
                        var itemStateName = GetItemStateName(id.ToString());

                        var result = state.TryGetStateAsync<TValue>(itemStateName).GetAwaiter().GetResult();
                        if (result.HasValue && state.TryRemoveStateAsync(itemStateName).GetAwaiter().GetResult())
                        {
                            state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, dict).GetAwaiter().GetResult();
                            value = result;
                        }
                    }
                }
            }

            return Task.FromResult(value);
        }

        public static Task<ConditionalValue<TValue>> TryGetValueFromDictionaryAsync<TKey, TValue>(this IActorStateManager state, string dictionaryName, TKey key)
        {
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);
            ConditionalValue<TValue> value = new ConditionalValue<TValue>(false, default(TValue));

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    Guid id;

                    if (!dict.TryGetValue(key, out id))
                        return Task.FromResult(value);

                    var itemStateName = GetItemStateName(id.ToString());
                    value = state.TryGetStateAsync<TValue>(itemStateName).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(value);
        }

        public static Task<bool> PurgeDictionary<TKey>(this IActorStateManager state, string dictionaryName)
        {
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count == 0)
                    return Task.FromResult(true);

                Guid id;

                foreach (var k in dict.Keys)
                {
                    if (dict.TryRemove(k, out id))
                    {
                        var itemStateName = GetItemStateName(id.ToString());
                        state.TryRemoveStateAsync(itemStateName).GetAwaiter().GetResult();

                        state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, dict).GetAwaiter().GetResult();
                    }
                }

                return state.TryRemoveStateAsync(dictionaryStateName);
            }
        }

        public static async Task<int> GetDictionaryLengthAsync<TKey>(this IActorStateManager state, string dictionaryName)
        {
            var dictionaryStateName = GetDictionaryStateName(dictionaryName);

            var dict = await state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(dictionaryStateName, new ConcurrentDictionary<TKey, Guid>());
            return dict.Count;
        }


        private static string GetDictionaryStateName(string dictionaryName)
        {
            return $"{typeof(FineGrainDictionaryManager).Name}::Dictionary({dictionaryName})";
        }

        private static string GetItemStateName(string itemId)
        {
            return $"{typeof(FineGrainDictionaryManager).Name}:Item({itemId})";
        }
    }
}
