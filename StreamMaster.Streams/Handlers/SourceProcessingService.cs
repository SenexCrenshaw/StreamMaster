using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Handlers;

public class SourceProcessingService(ILogger<IBroadcasterBase> logger, IOptionsMonitor<Setting> _settings) : ISourceProcessingService
{
    public Task ProcessSourceChannelReaderAsync(ChannelReader<byte[]> sourceChannelReader, Channel<byte[]> newChannel, IMetricsService metricsService, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            bool localInputStreamError = false;
            try
            {
                await foreach (byte[] item in sourceChannelReader.ReadAllAsync(token))
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    await newChannel.Writer.WriteAsync(item, token).ConfigureAwait(false);
                    sw.Stop();
                    metricsService.RecordMetrics(item.Length, sw.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception ex)
            {
                HandleReaderExceptions(ex, ref localInputStreamError);
                if (localInputStreamError)
                {
                    // Additional error handling if needed
                }
            }
            finally
            {
                newChannel.Writer.TryComplete();
            }
        }, token);
    }

    public Task ProcessSourceStreamAsync(Stream sourceStream, Channel<byte[]> newChannel, IMetricsService metricsService, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            bool localInputStreamError = false;
            try
            {
                byte[] buffer = new byte[16384];
                while (!token.IsCancellationRequested)
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    // Start the read task
                    Task<int> readTask = sourceStream.ReadAsync(buffer, token).AsTask();

                    Task completedTask;
                    if (_settings.CurrentValue.ReadTimeOutMs > 0)
                    {
                        // Create a timeout task if ReadTimeOutMs is greater than 0
                        Task timeoutTask = Task.Delay(_settings.CurrentValue.ReadTimeOutMs, token);

                        sw.Restart(); // Start the stopwatch to measure time for the read/timeout decision
                        completedTask = await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(false);
                        sw.Stop();

                        // Log the time taken for the read or timeout decision
                        logger.LogDebug("Read/timeout decision took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);

                        if (completedTask == timeoutTask)
                        {
                            // Handle the timeout case
                            logger.LogDebug("Read operation timed out after {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                            throw new TimeoutException("Read operation timed out.");
                        }
                    }
                    else
                    {
                        // No timeout, just await the read task
                        sw.Restart(); // Start the stopwatch for just the read operation
                        completedTask = readTask;
                        sw.Stop();

                        // Log the time taken for the read operation
                        logger.LogDebug("Read operation took {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    }

                    int bytesRead = await readTask.ConfigureAwait(false);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    await newChannel.Writer.WriteAsync(data, token).ConfigureAwait(false);

                    sw.Stop();
                    metricsService.RecordMetrics(bytesRead, sw.Elapsed.TotalMilliseconds);
                }
            }
            catch (TimeoutException ex)
            {
                // Handle the timeout exception specifically
                HandleReaderExceptions(ex, ref localInputStreamError);
            }
            catch (Exception ex)
            {
                HandleReaderExceptions(ex, ref localInputStreamError);
                if (localInputStreamError)
                {
                    // Additional error handling if needed
                }
            }
            finally
            {
                newChannel.Writer.TryComplete();
            }
        }, token);
    }

    private void HandleReaderExceptions(Exception ex, ref bool inputStreamError)
    {
        switch (ex)
        {
            case TaskCanceledException _:
            case OperationCanceledException _:
                logger.LogInformation("Source Reader stopped.");
                logger.LogDebug(ex, "Source Reader stopped.");
                break;
            case EndOfStreamException _:
                logger.LogInformation("Source Reader stopped.");
                logger.LogDebug(ex, "Source Reader End of stream reached.");
                inputStreamError = true;
                break;
            case HttpIOException _:
                logger.LogInformation("Source Reader stopped.");
                logger.LogDebug(ex, "Source Reader HTTP I/O exception occurred.");
                inputStreamError = true;
                break;
            default:
                logger.LogInformation("Source Reader stopped.");
                logger.LogDebug(ex, "Source Reader Unexpected error occurred.");
                break;
        }
    }
}
