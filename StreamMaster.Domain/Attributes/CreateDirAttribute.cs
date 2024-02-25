namespace StreamMaster.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class CreateDirAttribute : Attribute
{
}
