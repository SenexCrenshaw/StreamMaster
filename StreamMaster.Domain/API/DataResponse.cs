using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Attributes;

using System.Xml.Serialization;

namespace StreamMaster.Domain.API;

public static class DataResponse
{
    public static DataResponse<bool> True => new() { Message = "OK", Data = true };
    public static DataResponse<bool> False => new() { Message = "OK", Data = false };

}

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class DataResponse<T> : APIResponse
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

    public static new DataResponse<T> Ok => new() { Message = "OK" };
    public static new DataResponse<T> NotFound => new()
    {
        IsError = true,
        ErrorMessage = "Not Found"
    };

    public static new DataResponse<T> ErrorWithMessage(Exception exception, string message)
    {
        DataResponse<T> error = new()
        {
            IsError = true,
            ErrorMessage = $"{message} : {exception}"
        };
        return error;
    }

    public static new DataResponse<T> Success(T data)
    {
        return new DataResponse<T> { Message = "OK", Data = data };
    }

    public int Count => TotalItemCount;

}
