namespace StreamMasterDomain.Attributes
{
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
}
