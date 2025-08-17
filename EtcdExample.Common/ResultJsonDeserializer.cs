using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CSharpFunctionalExtensions;

namespace EtcdExample.Common;

public static class ResultJsonDeserializer
{
    public static Result<TValue> Deserialize<TValue>(string value)
    {
        try
        {
            var parseResult = JsonSerializer.Deserialize<TValue>(value);

            if (parseResult == null)
            {
                return Result.Failure<TValue>("connot parse");
            }

            return parseResult;

        }
        catch (Exception e)
        {
            return Result.Failure<TValue>("connot parse");
        }
    }
    public static Result<object> Deserialize(string value, Type targetType)
    {
        try
        {
            var parseResult = JsonSerializer.Deserialize(value, targetType);

            if (parseResult == null)
            {
                return Result.Failure<object>("connot parse");
            }

            return parseResult;

        }
        catch (Exception e)
        {
            return Result.Failure<object>("connot parse");
        }
    }
}