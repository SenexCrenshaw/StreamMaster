using System.Net;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public static class SDHandler
{
    public static async Task<(HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? data)> ProcessResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        SDHttpResponseCode responseCode = SDHttpResponseCode.UNKNOWN_ERROR;

        if (!string.IsNullOrEmpty(responseContent))
        {
            if (responseContent.Contains("serverID"))
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

            T? result = JsonSerializer.Deserialize<T>(responseContent);
            return (response.StatusCode, responseCode, responseContent, result);


        }
        return (response.StatusCode, responseCode, responseContent, default(T?));
    }
}
