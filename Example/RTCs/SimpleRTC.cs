using EtcdExample;

namespace Example.RTCs;

public class SimpleRTC
{
    [RTC("test", "12")]
    public List<int> test { get; set; }
}