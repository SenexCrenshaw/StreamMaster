namespace StreamMasterDomain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexBy(string value) : Attribute
    {
        public virtual string Value => value;
    }
}
