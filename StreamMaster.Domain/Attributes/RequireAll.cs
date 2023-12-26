namespace StreamMaster.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
    public sealed class RequireAllAttribute : Attribute
    {
    }
}
