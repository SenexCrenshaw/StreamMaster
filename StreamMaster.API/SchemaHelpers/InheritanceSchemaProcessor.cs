using NJsonSchema.Generation;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.API.SchemaHelpers;

public class InheritanceSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        Type? typeToCheck = context.ContextualType;  // Note the nullable annotation here with `Type?`
        if (typeToCheck == null)
        {
            return;
        }

        bool hasAttribute = typeToCheck.GetCustomAttributes(typeof(RequireAllAttribute), true).Length != 0;

        if (hasAttribute)
        {
            foreach (KeyValuePair<string, NJsonSchema.JsonSchemaProperty> property in context.Schema.ActualProperties)
            {
                property.Value.IsRequired = true;
            }
        }
    }
}
