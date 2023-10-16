namespace StreamMasterDomain.Extensions;

public static class TokenExtensions
{
    public static async Task<bool> ApplyDelay(this CancellationToken token, int milliseconds = 200)
    {
        try
        {
            await Task.Delay(milliseconds, token);
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
