using System.Text.Json;
using CSharpFunctionalExtensions;

namespace EtcdExample;

public class RTCStore : IRTCStore
{
    private RTCSettingsStore _store;

    public RTCStore(RTCSettingsStore store)
    {
        _store = store;
    }

    public Result<TSettings> Get<TSettings>()
    {
        var typeInfo = typeof(TSettings);

        var result = Get(typeInfo);

        return result.IsSuccess ? Result.Success((TSettings)result.Value) : result.ConvertFailure<TSettings>();
    }

    public Result<object> Get(Type typeInfo)
    {
        var canCreate = typeInfo.GetConstructors().Any(ex => ex.IsPublic && ex.IsConstructor && ex.GetParameters().Length == 0);

        if (canCreate == false)
        {
            return Result.Failure("is dont have empty ctor");
        }

        var settings = typeInfo.GetProperties()
            .Where(ex => ex.CanWrite && ex.GetCustomAttributes(typeof(RTCAttribute), true).Any())
            .ToList();

        if (settings.Any() == false)
        {
            return Result.Failure("nothing to get");
        }

        var instance = Activator.CreateInstance(typeInfo);
        
        settings.ForEach(ex =>
        {
            var attribute = (RTCAttribute)ex.GetCustomAttributes(typeof(RTCAttribute), true).Single();
            
            var settingsValue = _store.GetValue(attribute.Name, ex.PropertyType);
            
            ex.SetValue(instance, settingsValue.IsSuccess ? settingsValue.Value : JsonSerializer.Deserialize(attribute.DefaultValue, ex.PropertyType));
        });

        return instance;
    }
}