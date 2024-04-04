
// Example structure for method details

public class MethodDetails
{
    public string Name { get; set; }
    public string ReturnType { get; set; }
    public string Parameter { get; set; }
    public string ParameterNames { get; set; }
    public bool JustHub { get; set; }

    public string TsParameter { get; set; }
    public bool IsGetPaged { get; internal set; }
    public bool JustController { get; internal set; }
    public bool IsTask { get; internal set; }
    public string TsReturnType { get; internal set; }
    public bool IsList { get; internal set; }
    public List<string> SMAPIImport { get; internal set; }
    public string TSName { get; internal set; }
    public string ReturnEntityType { get; internal set; }
    public string SingalRFunction { get; internal set; }
    public string NamespaceName { get; internal set; }
    public bool IsGet { get; internal set; }
    public bool IsReturnNull { get; internal set; }
}