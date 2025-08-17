using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using EtcdExample.Common;

namespace EtcdExample;

public class RTCSettingsStore
{
    private readonly ConcurrentDictionary<string, string> _values;

    public RTCSettingsStore()
    {
        _values = new();
    }
    
    public Result<TValue> GetValue<TValue>(string key)
    {
        if (_values.ContainsKey(key) == false)
        {
            return Result.Failure<TValue>("dont contain key");
        }

        var valueByKey = _values[key];

        if (valueByKey is TValue result)
        {
            return result;
        }

        return ResultJsonDeserializer.Deserialize<TValue>(valueByKey);
    }

    public Result<object> GetValue(string key, Type targetType)
    {
        if (_values.ContainsKey(key) == false)
        {
            return Result.Failure<object>("dont contain key");
        }

        var valueByKey = _values[key];

        if (valueByKey.GetType() == targetType)
        {
            return valueByKey;
        }

        return ResultJsonDeserializer.Deserialize(valueByKey, targetType);
    }
    
    public void SetValue(string key, string value)
    {
        if (_values.ContainsKey(key) == false)
        {
            _values.TryAdd(key, value);
        }

        _values[key] = value;
    }
}