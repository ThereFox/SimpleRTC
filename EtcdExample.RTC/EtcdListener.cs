using System.Text;
using dotnet_etcd;
using dotnet_etcd.interfaces;
using Etcdserverpb;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Mvccpb;

namespace EtcdExample;

public class EtcdListener : IHostedService
{
    private readonly IEtcdClient _client;
    private readonly RTCSettingsStore _settingsStore;

    public EtcdListener(IEtcdClient client, RTCSettingsStore settingsStore)
    {
        _client = client;
        _settingsStore = settingsStore;
    }
    
    public async Task Start(long ttl)
    {
        await _client.LeaseGrantAsync(
            request: new LeaseGrantRequest()
            {
                ID = 1,
                TTL = ttl
            }
        );
    }

    private async Task subsrcibe(CancellationToken token)
    {
        await _client.WatchAsync(
            request: new WatchRequest()
            {
                CreateRequest = new WatchCreateRequest()
                {
                    Key = ByteString.CopyFrom("test", Encoding.ASCII),
                    ProgressNotify = true,
                    StartRevision = 1,
                    Fragment = true,
                    WatchId = 13,
                    PrevKv = true
                }
            },
            method: changes =>
            {
                foreach (var eventData in changes.Events)
                {
                    var key = Encoding.ASCII.GetString(eventData.Kv.Key.Span);
                    var value = Encoding.ASCII.GetString(eventData.Kv.Value.Span);
                    
                    if (eventData.Type == Event.Types.EventType.Delete)
                    {
                        _settingsStore.SetValue(key, null);
                    }

                    _settingsStore.SetValue(key, value);
                }
            },
            cancellationToken: token
        );
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await subsrcibe(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.Dispose();
        return Task.CompletedTask;
    }
}