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
            catch (OperationCanceledException)
            {
                // Handle task cancellation if needed, otherwise it will propagate.
                throw;
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
                Stopwatch sw = new();

                while (!token.IsCancellationRequested)
                {
                    sw.Restart();

                    Task<int> readTask = sourceStream.ReadAsync(buffer, token).AsTask();
                    Task completedTask;

                    if (_settings.CurrentValue.ReadTimeOutMs > 0)
                    {
                        Task timeoutTask = Task.Delay(_settings.CurrentValue.ReadTimeOutMs, token);
                        completedTask = await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(false);

                        if (completedTask == timeoutTask)
                        {
                            logger.LogDebug("Read operation timed out after {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                            throw new TimeoutException("Read operation timed out.");
                        }
                    }
                    else
                    {
                        completedTask = readTask;
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
                    logger.LogDebug("Read operation took {ElapsedMilliseconds} ms and read {BytesRead} bytes", sw.ElapsedMilliseconds, bytesRead);
                    metricsService.RecordMetrics(bytesRead, sw.Elapsed.TotalMilliseconds);
                }
            }
            catch (TimeoutException ex)
            {
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
