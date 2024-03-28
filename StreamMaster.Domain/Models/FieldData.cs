using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record FieldData(string Entity, string Id, string Field, object Value) { }

