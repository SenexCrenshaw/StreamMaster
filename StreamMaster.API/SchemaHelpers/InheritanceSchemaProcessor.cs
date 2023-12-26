using NJsonSchema.Generation;

using StreamMaster.Domain.Attributes;

namespace StreamMasterAPI.SchemaHelpers;

public class InheritanceSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        Type? typeToCheck = context.ContextualType;  // Note the nullable annotation here with `Type?`

        bool hasAttribute = typeToCheck.GetCustomAttributes(typeof(RequireAllAttribute), true).Any();

        if (hasAttribute)
        {
            foreach (KeyValuePair<string, NJsonSchema.JsonSchemaProperty> property in context.Schema.ActualProperties)
            {
                property.Value.IsRequired = true;
            }
        }
    }
}
