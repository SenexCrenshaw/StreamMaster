using System.Net;

namespace StreamMasterInfrastructure.Common;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage?> GetWithRedirectAsync(
        this HttpClient httpClient,
        string requestUri,
        //long byteRangeStart = 0,
        //long byteRangeLength = -1,
        int maxRetries = 5,
        CancellationToken cancellationToken = default)
    {
        int retryCount = 0;
        HttpResponseMessage? response = null;
        string redirectUrl = requestUri;

        while (retryCount <= maxRetries)
        {
            //if (byteRangeLength != -1)
            //{
            //    httpClient.DefaultRequestHeaders.Range = new RangeHeaderValue(byteRangeStart, byteRangeStart + byteRangeLength);
            //}
            //else
            //{
            //    httpClient.DefaultRequestHeaders.Range = null;
            //}
            try
            {
                response = await httpClient.GetAsync(redirectUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                if (response.Headers.Location != null)
                {
                    redirectUrl = response.Headers.Location.AbsoluteUri;
                    retryCount++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return response;
    }
}
