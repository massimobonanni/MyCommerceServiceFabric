using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace MyCommerce.SF.Core.Extensions
{
    public static class FineGrainMapManager
    {
        private static object _criticalSection = new object();

        public static Task<bool> TryAddToMapAsync<TKey, TValue1, TValue2>(this IActorStateManager state, string mapName, TKey key, TValue1 value1, TValue2 value2)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var id = Guid.NewGuid();

                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();

                result = dict.TryAdd(key, id);
                if (result)
                {
                    var item1StateName = GetItemStateName(id.ToString(), 1);
                    var item2StateName = GetItemStateName(id.ToString(), 2);

                    state.SetStateAsync<TValue1>(item1StateName, value1).GetAwaiter().GetResult();
                    state.SetStateAsync<TValue2>(item2StateName, value2).GetAwaiter().GetResult();

                    state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(result);
        }

        public static Task<bool> TryAddOrUpdateMapAsync<TKey, TValue1, TValue2>(this IActorStateManager state, string mapName, TKey key, TValue1 value1, TValue2 value2)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                Guid id = Guid.Empty;

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out id);

                if (result)
                {
                    var item1StateName = GetItemStateName(id.ToString(), 1);
                    var item2StateName = GetItemStateName(id.ToString(), 2);

                    state.SetStateAsync<TValue1>(item1StateName, value1).GetAwaiter().GetResult();
                    state.SetStateAsync<TValue2>(item2StateName, value2).GetAwaiter().GetResult();

                    state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
                }
                else
                {
                    result = state.TryAddToMapAsync<TKey, TValue1, TValue2>(mapName, key, value1, value2).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(true);
        }

        public static Task<bool> TryUpdateValue1InMapAsync<TKey, TValue1>(this IActorStateManager state, string mapName, TKey key, TValue1 value1)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                Guid id = Guid.Empty;

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out id);

                if (!result)
                    return Task.FromResult(false);

                var item1StateName = GetItemStateName(id.ToString(), 1);
                state.SetStateAsync<TValue1>(item1StateName, value1).GetAwaiter().GetResult();

                state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
            }

            return Task.FromResult(true);
        }

        public static Task<bool> TryUpdateValue2InMapAsync<TKey, TValue2>(this IActorStateManager state, string mapName, TKey key, TValue2 value2)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                Guid id = Guid.Empty;

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out id);

                if (!result)
                    return Task.FromResult(false);

                var item2StateName = GetItemStateName(id.ToString(), 2);
                state.SetStateAsync<TValue2>(item2StateName, value2).GetAwaiter().GetResult();

                state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
            }

            return Task.FromResult(true);
        }

        public static Task<ConditionalValue<Tuple<TValue1, TValue2>>> TryRemoveFromMapAsync<TKey, TValue1, TValue2>(this IActorStateManager state, string mapName, TKey key)
        {
            var value = new ConditionalValue<Tuple<TValue1, TValue2>>(false, default(Tuple<TValue1, TValue2>));
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    Guid id;

                    if (dict.TryRemove(key, out id))
                    {
                        TValue1 value1 = default(TValue1);
                        TValue2 value2 = default(TValue2);

                        var item1StateName = GetItemStateName(id.ToString(), 1);
                        var result1 = state.TryGetStateAsync<TValue1>(item1StateName).GetAwaiter().GetResult();
                        if (result1.HasValue && state.TryRemoveStateAsync(item1StateName).GetAwaiter().GetResult())
                        {
                            state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
                            value1 = result1.Value;
                        }

                        var item2StateName = GetItemStateName(id.ToString(), 2);
                        var result2 = state.TryGetStateAsync<TValue2>(item2StateName).GetAwaiter().GetResult();
                        if (result2.HasValue && state.TryRemoveStateAsync(item2StateName).GetAwaiter().GetResult())
                        {
                            state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
                            value2 = result2.Value;
                        }

                        value = new ConditionalValue<Tuple<TValue1, TValue2>>(true, new Tuple<TValue1, TValue2>(value1, value2));
                    }
                }
            }

            return Task.FromResult(value);
        }

        public static Task<ConditionalValue<Tuple<TValue1, TValue2>>> TryGetValuesFromMapAsync<TKey, TValue1, TValue2>(this IActorStateManager state, string mapName, TKey key)
        {
            var value = new ConditionalValue<Tuple<TValue1, TValue2>>(false, default(Tuple<TValue1, TValue2>));
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    Guid id;

                    if (!dict.TryGetValue(key, out id))
                        return Task.FromResult(value);

                    var item1StateName = GetItemStateName(id.ToString(), 1);
                    var result1 = state.TryGetStateAsync<TValue1>(item1StateName).GetAwaiter().GetResult();

                    var item2StateName = GetItemStateName(id.ToString(), 2);
                    var result2 = state.TryGetStateAsync<TValue2>(item2StateName).GetAwaiter().GetResult();

                    state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();

                    if (result1.HasValue && result2.HasValue)
                        value = new ConditionalValue<Tuple<TValue1, TValue2>>(true, new Tuple<TValue1, TValue2>(result1.Value, result2.Value));
                }
            }

            return Task.FromResult(value);
        }

        public static Task<bool> PurgeMap<TKey>(this IActorStateManager state, string mapName)
        {
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>()).GetAwaiter().GetResult();
                if (dict.Count == 0)
                    return Task.FromResult(true);

                Guid id;

                foreach (var k in dict.Keys)
                {
                    if (dict.TryRemove(k, out id))
                    {
                        var item1StateName = GetItemStateName(id.ToString(), 1);
                        var item2StateName = GetItemStateName(id.ToString(), 2);

                        state.TryRemoveStateAsync(item1StateName).GetAwaiter().GetResult();
                        state.TryRemoveStateAsync(item2StateName).GetAwaiter().GetResult();

                        state.SetStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, dict).GetAwaiter().GetResult();
                    }
                }

                return state.TryRemoveStateAsync(mapStateName);
            }
        }

        public static async Task<int> GetMapLengthAsync<TKey>(this IActorStateManager state, string mapName)
        {
            var mapStateName = GetMapStateName(mapName);

            var dict = await state.GetOrAddStateAsync<ConcurrentDictionary<TKey, Guid>>(mapStateName, new ConcurrentDictionary<TKey, Guid>());
            return dict.Count;
        }


        private static string GetMapStateName(string mapName)
        {
            return $"{typeof(FineGrainMapManager).Name}::Map({mapName})";
        }

        private static string GetItemStateName(string itemId, ushort idx)
        {
            return $"{typeof(FineGrainMapManager).Name}:Item{idx}({itemId})";
        }
    }
}
