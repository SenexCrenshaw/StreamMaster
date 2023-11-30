using Microsoft.Extensions.Logging;

using System.Net;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public static class SDHandler
{
    public static async Task<(HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? data)> ProcessResponse<T>(HttpResponseMessage response, ILogger logger, CancellationToken cancellationToken)
    {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        SDHttpResponseCode responseCode = SDHttpResponseCode.UNKNOWN_ERROR;

        try
        {
            T? result = JsonSerializer.Deserialize<T>(responseContent);
            if (result != null)
            {
                return (response.StatusCode, SDHttpResponseCode.OK, responseContent, result);
            }
        }
        catch (JsonException ex)
        {
            logger.LogWarning("Deserialization to type {Type} failed: {Message}", typeof(T).Name, ex.Message);
        }

        if (!responseContent.StartsWith("[{\"programID\"") && responseContent.Contains("serverID"))
        {
            try
            {
                SDTokenResponse? responseObj = JsonSerializer.Deserialize<SDTokenResponse>(responseContent);
                if (responseObj is not null)
                {
                    responseCode = (SDHttpResponseCode)responseObj.code;
                    if (responseCode != SDHttpResponseCode.OK || !response.IsSuccessStatusCode)
                    {
                        return (response.StatusCode, responseCode, responseContent, default(T?));
                    }
                }
            }
            catch (JsonException ex)
            {
                //logger.LogWarning("Deserialization to SDTokenResponse failed: {Message}", ex.Message);
            }
        }

        return (response.StatusCode, responseCode, responseContent, default(T?));
    }
}
