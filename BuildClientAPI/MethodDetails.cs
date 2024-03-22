
// Example structure for method details
public class MethodDetails
{
    public string Name { get; set; }
    public string ReturnType { get; set; }
    public string Parameters { get; set; } // Formatted as "type name, type name"
    public string ParameterNames { get; set; } // Formatted as "name, name"
    public bool JustHub { get; set; }
    // TypeScript specific properties
    public string TsParameters { get; set; } // TypeScript parameters formatted as "name: type, name: type"
    public string TsParameterTypes { get; set; } // TypeScript parameter types formatted as "type, type"
    public bool IsGet { get; internal set; }
    public bool JustController { get; internal set; }
    public bool IsTask { get; internal set; }
    public string TsReturnInterface { get; internal set; }
}