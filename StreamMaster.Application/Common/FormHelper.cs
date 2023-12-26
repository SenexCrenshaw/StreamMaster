using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Common;

public sealed class FormHelper
{
    public static async Task<(bool success, Exception? ex)> SaveFormFileAsync(IFormFile data, string fileName)
    {
        try
        {
            //if (!File.Exists(fileName))
            //{
            //    FileUtil.CreateDirectory(fileName);
            //}
            using FileStream createStream = File.Create(fileName);
            await data.CopyToAsync(createStream).ConfigureAwait(false);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
        finally
        {
        }
    }
}
