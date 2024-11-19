
// Example structure for method details

public class MethodDetails
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public string ParameterNames { get; set; } = string.Empty;
    public bool JustHub { get; set; }

    public string TsParameter { get; set; } = string.Empty;
    public bool IsGetPaged { get; internal set; }
    public bool JustController { get; internal set; }
    public bool IsTask { get; internal set; }
    public string TsReturnType { get; internal set; } = string.Empty;
    public bool IsList { get; internal set; }
    public List<string> SMAPIImport { get; internal set; } = [];
    public string TSName { get; internal set; } = string.Empty;
    public string ReturnEntityType { get; internal set; } = string.Empty;
    public string SingalRFunction { get; internal set; } = string.Empty;
    public string NamespaceName { get; internal set; } = string.Empty;
    public bool IsGet { get; internal set; }
    public bool IsGetCached => !IsGetPaged && IsGet && !string.IsNullOrEmpty(TsParameter);
    public bool IsReturnNull { get; internal set; }
    public bool Pertsist { get; internal set; }
}