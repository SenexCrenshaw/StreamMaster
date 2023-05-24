namespace StreamMasterDomain.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BuilderIgnore : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IndexBy : Attribute
    {
        private readonly string value;

        public IndexBy(string value)
        {
            this.value = value;
        }

        public virtual string Value => value;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class JustUpdates : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RequireAll : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SortBy : Attribute
    {
    }
}
