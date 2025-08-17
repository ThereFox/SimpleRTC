namespace EtcdExample;

public class RTCAttribute : Attribute
{
    public RTCAttribute(string name, string defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }

    public string Name { get; init; }
    public string DefaultValue { get; init; }
}