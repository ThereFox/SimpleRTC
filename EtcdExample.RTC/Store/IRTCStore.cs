using CSharpFunctionalExtensions;

namespace EtcdExample;

public interface IRTCStore
{
    public Result<TSettings> Get<TSettings>();
    public Result<object> Get(Type type);
}