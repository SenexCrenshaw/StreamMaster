using NJsonSchema.Generation;

using StreamMasterDomain.Attributes;

namespace StreamMasterAPI.SchemaHelpers;

public class InheritanceSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        IEnumerable<Attribute> test = context.ContextualType.Attributes.ToList().Where(a => a.GetType() == typeof(RequireAll));
        if (test.Any())
        {
            foreach (KeyValuePair<string, NJsonSchema.JsonSchemaProperty> property in context.Schema.Properties)
            {
                property.Value.IsRequired = true;
            }
        }
    }
}
