using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace MyCommerce.SF.Core.Extensions
{
    public static class FineGrainStatusMapManager
    {
        private static object _criticalSection = new object();


        [DataContract]
        public class StatusMapValueStatusPair<TValue, TStatus> : Tuple<TValue, TStatus>
        {
            public StatusMapValueStatusPair(TValue item1, TStatus item2) : base(item1, item2)
            {
            }
        }

        [DataContract]
        internal class StatusMapGuidStatusPair<TStatus> : Tuple<Guid, TStatus>
        {
            public StatusMapGuidStatusPair(Guid item1, TStatus item2) : base(item1, item2)
            {
            }
        }

        internal class StatusMapDictionary<TKey, TStatus> : ConcurrentDictionary<TKey, StatusMapGuidStatusPair<TStatus>>
        {
        }


        public static Task<bool> TryAddToStatusMapAsync<TKey, TValue, TStatus>(this IActorStateManager state, string mapName, TKey key, TValue value, TStatus status)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var id = Guid.NewGuid();
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();

                result = dict.TryAdd(key, new StatusMapGuidStatusPair<TStatus>(id, status));
                if (result)
                {
                    var itemStateName = GetItemStateName(id.ToString());
                    state.SetStateAsync<TValue>(itemStateName, value).GetAwaiter().GetResult();

                    state.SetStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, dict).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(result);
        }

        public static Task<bool> TryAddOrUpdateStatusMapAsync<TKey, TValue, TStatus>(this IActorStateManager state, string mapName, TKey key, TValue value, TStatus status)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                StatusMapGuidStatusPair<TStatus> tupleIDStatus = default(StatusMapGuidStatusPair<TStatus>);

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out tupleIDStatus);

                if (result)
                {
                    dict[key] = new StatusMapGuidStatusPair<TStatus>(tupleIDStatus.Item1, status);

                    var itemStateName = GetItemStateName(tupleIDStatus.Item1.ToString());
                    state.SetStateAsync<TValue>(itemStateName, value).GetAwaiter().GetResult();

                    state.SetStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, dict).GetAwaiter().GetResult();
                }
                else
                {
                    result = state.TryAddToStatusMapAsync<TKey, TValue, TStatus>(mapName, key, value, status).GetAwaiter().GetResult();
                }
            }

            return Task.FromResult(result);
        }

        public static Task<bool> TryUpdateStatusInStatusMapAsync<TKey, TStatus>(this IActorStateManager state, string mapName, TKey key, TStatus status)
        {
            var result = false;
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                StatusMapGuidStatusPair<TStatus> tupleIDStatus = default(StatusMapGuidStatusPair<TStatus>);

                result = dict.Count != 0;
                if (result)
                    result = dict.TryGetValue(key, out tupleIDStatus);

                if (!result)
                    return Task.FromResult(false);

                dict[key] = new StatusMapGuidStatusPair<TStatus>(tupleIDStatus.Item1, status);
                state.SetStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, dict).GetAwaiter().GetResult();
            }

            return Task.FromResult(result);
        }

        public static Task<ConditionalValue<TValue>> TryRemoveFromMapAsync<TKey, TValue, TStatus>(this IActorStateManager state, string mapName, TKey key)
        {
            var value = new ConditionalValue<TValue>(false, default(TValue));
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    StatusMapGuidStatusPair<TStatus> tupleIDStatus = default(StatusMapGuidStatusPair<TStatus>);

                    if (dict.TryRemove(key, out tupleIDStatus))
                    {
                        var itemStateName = GetItemStateName(tupleIDStatus.Item1.ToString());
                        var result = state.TryGetStateAsync<TValue>(itemStateName).GetAwaiter().GetResult();

                        if (result.HasValue && state.TryRemoveStateAsync(itemStateName).GetAwaiter().GetResult())
                            state.SetStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, dict).GetAwaiter().GetResult();

                        value = new ConditionalValue<TValue>(result.HasValue, result.Value);
                    }
                }
            }

            return Task.FromResult(value);
        }

        public static Task<ConditionalValue<StatusMapValueStatusPair<TValue, TStatus>>> TryGetValueAndStatusFromStatusMapAsync<TKey, TValue, TStatus>(this IActorStateManager state, string mapName, TKey key)
        {
            var value = new ConditionalValue<StatusMapValueStatusPair<TValue, TStatus>>(false, new StatusMapValueStatusPair<TValue, TStatus>(default(TValue), default(TStatus)));
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    StatusMapGuidStatusPair<TStatus> tupleIDStatus = default(StatusMapGuidStatusPair<TStatus>);

                    if (!dict.TryGetValue(key, out tupleIDStatus))
                        return Task.FromResult(value);

                    var itemStateName = GetItemStateName(tupleIDStatus.Item1.ToString());
                    var result = state.TryGetStateAsync<TValue>(itemStateName).GetAwaiter().GetResult();

                    value = new ConditionalValue<StatusMapValueStatusPair<TValue, TStatus>>(result.HasValue, new StatusMapValueStatusPair<TValue, TStatus>(result.Value, tupleIDStatus.Item2));
                }
            }

            return Task.FromResult(value);
        }

        public static Task<TValue[]> TryGetValuesWithStatusFromStatusMapAsync<TKey, TValue, TStatus>(this IActorStateManager state, string mapName, TKey key, TStatus filterStatus)
        {
            var valuesList = new List<TValue>();
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                if (dict.Count != 0)
                {
                    var idList = dict.Where(it => it.Value != null && it.Value.Item2.Equals(filterStatus)).Select(it => it.Value.Item1);
                    foreach (var id in idList)
                    {
                        var itemStateName = GetItemStateName(id.ToString());
                        var result = state.TryGetStateAsync<TValue>(itemStateName).GetAwaiter().GetResult();
                        if (result.HasValue)
                            valuesList.Add(result.Value);
                    }
                }
            }

            return Task.FromResult(valuesList.ToArray());
        }

        public static Task<bool> PurgeStatusMap<TKey, TValue, TStatus>(this IActorStateManager state, string mapName)
        {
            var mapStateName = GetMapStateName(mapName);

            lock (_criticalSection)
            {
                var dict = state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>()).GetAwaiter().GetResult();
                if (dict.Count == 0)
                    return Task.FromResult(true);

                StatusMapGuidStatusPair<TStatus> tupleIDStatus = default(StatusMapGuidStatusPair<TStatus>);

                foreach (var key in dict.Keys)
                {
                    if (dict.TryRemove(key, out tupleIDStatus))
                    {
                        var itemStateName = GetItemStateName(tupleIDStatus.Item1.ToString());
                        state.TryRemoveStateAsync(itemStateName).GetAwaiter().GetResult();

                        state.SetStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, dict).GetAwaiter().GetResult();
                    }
                }

                return state.TryRemoveStateAsync(mapStateName);
            }
        }

        public static async Task<int> GetMapLengthAsync<TKey, TStatus>(this IActorStateManager state, string mapName)
        {
            var mapStateName = GetMapStateName(mapName);

            var dict = await state.GetOrAddStateAsync<StatusMapDictionary<TKey, TStatus>>(mapStateName, new StatusMapDictionary<TKey, TStatus>());
            return dict.Count;
        }


        private static string GetMapStateName(string mapName)
        {
            return $"{typeof(FineGrainStatusMapManager).Name}::Map({mapName})";
        }

        private static string GetItemStateName(string itemId)
        {
            return $"{typeof(FineGrainStatusMapManager).Name}:Item({itemId})";
        }
    }
}
