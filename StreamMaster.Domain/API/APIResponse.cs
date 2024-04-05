using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.Domain.API;

public static class APIResponse
{
    public static APIResponse<bool> True => new() { Message = "OK", Data = true };
    public static APIResponse<bool> False => new() { Message = "OK", Data = false };
    public static APIResponse<object> Ok => APIResponse<object>.Ok;
    public static APIResponse<object> NotFound => APIResponse<object>.NotFound;
}

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class APIResponse<T> : DefaultAPIResponse
{

    private int? _totalItemCount { get; set; }
    [XmlIgnore]
    public T Data { get; set; }
    public int TotalItemCount
    {
        get
        {
            if (_totalItemCount.HasValue)
            {
                return _totalItemCount.Value;
            }

            if (Data is null)
            {
                return 0;
            }

            // Try getting the 'Count' property for collections like List<T>
            System.Reflection.PropertyInfo? countProperty = Data.GetType().GetProperty("Count");
            if (countProperty != null && countProperty.PropertyType == typeof(int))
            {
                return (int)(countProperty.GetValue(Data) ?? 0);
            }

            System.Reflection.PropertyInfo? lengthProperty = Data.GetType().GetProperty("Length");
            return lengthProperty != null && lengthProperty.PropertyType == typeof(int) ? (int)(lengthProperty.GetValue(Data) ?? 0) : 0;
        }

        set => _totalItemCount = value;
    }

    public static APIResponse<T> Ok => new() { Message = "OK" };
    public static new APIResponse<T> NotFound => new()
    {
        IsError = true,
        ErrorMessage = "Not Found"
    };

    public static new APIResponse<T> Success(T data)
    {
        return new APIResponse<T> { Message = "OK", Data = data };
    }

    public int Count => TotalItemCount;

}
