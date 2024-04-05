using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;
//public enum ModelAction
//{
//    Unknown = 0,
//    Created = 1,
//    Updated = 2,
//    Deleted = 3,
//    Sync = 4
//}


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record FieldData(string Entity, string Id, string Field, object Value) { }

