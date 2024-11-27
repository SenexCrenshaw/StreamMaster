using System.Xml.Serialization;

using StreamMaster.Domain.Attributes;

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
#pragma warning disable IDE1006 // Naming Styles
    private int? totalItemCount { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    [XmlIgnore]
    public T? Data { get; set; }
    public int TotalItemCount
    {
        get
        {
            if (totalItemCount.HasValue)
            {
                return totalItemCount.Value;
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

        set => totalItemCount = value;
    }

    public static new DataResponse<T> Ok => new() { Message = "OK" };
    public static new DataResponse<T> NotFound => new()
    {
        IsError = true,
        ErrorMessage = "Not Found"
    };

    public static new DataResponse<T> ErrorWithMessage(Exception exception, string message)
    {
        string ErrorMessage = $"{message} : {exception}";

        return ErrorWithMessage(ErrorMessage);
    }

    public static new DataResponse<T> ErrorWithMessage(string message)
    {
        DataResponse<T> error = new()
        {
            IsError = true,
            ErrorMessage = message
        };
        return error;
    }

    public static new DataResponse<T> Success(T data)
    {
        return new DataResponse<T> { Message = "OK", Data = data };
    }

    public int Count => TotalItemCount;
}
