namespace StreamMaster.Domain.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SMAPIAttribute(bool JustHub = false, bool JustController = false, bool IsTask = false) : Attribute
{
    public bool IsTask { get; set; } = IsTask;
    public bool JustHub { get; set; } = JustHub;
    public bool JustController { get; set; } = JustController;
}
