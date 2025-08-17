using System.Reflection;
using dotnet_etcd;
using dotnet_etcd.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EtcdExample.DI;

public static class RTCRegister
{
    public static IServiceCollection AddRTC(this IServiceCollection services, string host, string serviceName)
    {
        services.AddSingleton<IEtcdClient>(
            ex => new EtcdClient(host, 2379, serviceName)
            );
        services.AddSingleton<RTCSettingsStore>();
        services.AddHostedService<EtcdListener>();

        services.AddScoped<IRTCStore, RTCStore>();
        
        return services;
    }

    public static IServiceCollection RegisterAllSettingsAsServiceFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var classes = assembly.GetTypes()
            .Where(ex =>
            {
                var canCreate = ex.GetConstructors()
                    .Any(ex => ex.IsPublic && ex.IsConstructor && ex.GetParameters().Length == 0);

                var propertyes = ex.GetProperties();

                return canCreate && propertyes.Any(ex =>
                    ex.CanWrite && ex.GetCustomAttributes(typeof(RTCAttribute), true).Any());
            }).ToList();

        foreach (var rtcSetting in classes)
        {
            services.AddScoped(rtcSetting, (ex) => ex.GetRequiredService<IRTCStore>().Get(rtcSetting).Value);
        }

        return services;
    }
    
}