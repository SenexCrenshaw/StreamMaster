using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Buffers;

public sealed partial class ClientReadStream
{
    private readonly CancellationTokenSource _readCancel = new();
    private readonly ILogger<ReadsLogger> _readLogger;

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        if (IsCancelled)
        {
            return 0;
        }

        Stopwatch stopWatch = Stopwatch.StartNew();

        int bytesRead = 0;


        _readLogger.LogDebug($"-------------------{VideoStreamName}-----------------------------");
        try
        {
            CancellationTokenSource timedToken = new(TimeSpan.FromSeconds(30));
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_readCancel.Token, timedToken.Token, cancellationToken);

            while (!Channel.Reader.CanPeek)
            {
                await Task.Delay(10, cancellationToken);
            }
            byte[] read = await Channel.Reader.ReadAsync(linkedCts.Token);
            bytesRead = read.Length;
            _readLogger.LogDebug("End bytesRead: {bytesRead}", bytesRead);
            if (bytesRead == 0)
            {
                return 0;
            }
            read[..bytesRead].CopyTo(buffer);

            metrics.RecordBytesProcessed(bytesRead);

            if (timedToken.IsCancellationRequested)
            {
                logger.LogWarning("ReadAsync timedToken cancelled for ClientId: {ClientId}", ClientId);
                return bytesRead;
            }
        }
        catch (ChannelClosedException ex)
        {
            logger.LogInformation(ex, "ReadAsync closed for ClientId: {ClientId}", ClientId);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogInformation(ex, "ReadAsync cancelled ended for ClientId: {ClientId}", ClientId);
            logger.LogInformation("ReadAsync {_readCancel.Token}", _readCancel.Token.IsCancellationRequested);
            logger.LogInformation("ReadAsync {cancellationToken}", cancellationToken.IsCancellationRequested);
            bytesRead = 1;
        }
        finally
        {
            stopWatch.Stop();

            SetMetrics(bytesRead);

            if (bytesRead == 0)
            {
                logger.LogDebug("Read 0 bytes for ClientId: {ClientId}", ClientId);
            }
        }

        //_clientStatisticsManager.AddBytesRead(ClientId, bytesRead);
        return bytesRead;
    }


}