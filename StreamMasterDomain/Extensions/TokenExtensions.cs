namespace StreamMasterDomain.Extensions;

public static class TokenExtensions
{
    //public static async Task<bool> ApplyDelay(this CancellationToken token, int delayDuration = 200)
    //{
    //    try
    //    {
    //        await token.DelayWithCancellation(delayDuration);
    //        return true;
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        throw;
    //    }
    //}

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
