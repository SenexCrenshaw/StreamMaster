using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMMessage(string Severity, string Summary, string? Detail) { }

