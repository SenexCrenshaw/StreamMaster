using Reinforced.Typings.Attributes;

using System.Text.Json;

namespace StreamMaster.Domain.Models;


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FieldData
{
    public FieldData(string entity, object Parameter, object value)
    {
        string jsonString = JsonSerializer.Serialize(Parameter);
        Entity = entity;
        Id = jsonString;
        Value = value;
    }

    public FieldData(string entity, string id, string field, object? value)
    {
        Entity = entity;
        Id = id;
        Field = field;
        Value = value;
    }

    public string Entity { get; set; }
    public string Id { get; set; }
    public string? Field { get; set; }
    public object? Value { get; set; }
}

