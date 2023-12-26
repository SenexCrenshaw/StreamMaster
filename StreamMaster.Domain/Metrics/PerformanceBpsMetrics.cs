using System.Diagnostics;

namespace StreamMaster.Domain.Metrics;

public class PerformanceBpsMetrics
{
    public Guid Identifier { get; private set; }
    private Stopwatch Timer { get; }
    private long TotalBytesProcessed { get; set; }
    private long LastUpdateMilliseconds { get; set; }

    public PerformanceBpsMetrics(Guid identifier)
    {
        Identifier = identifier;
        Timer = new Stopwatch();
        Timer.Start();
        TotalBytesProcessed = 0;
        LastUpdateMilliseconds = 0;
    }

    public void StartOperation()
    {
        if (!Timer.IsRunning)
        {
            Timer.Start();
        }
    }

    public void StopOperation()
    {
        Timer.Stop();
    }

    public void RecordBytesProcessed(int bytesProcessed)
    {
        TotalBytesProcessed += bytesProcessed;

    }
    public double GetElapsedSeconds()
    {
        return Timer.Elapsed.TotalSeconds;
    }

    public (double bytesRead, double bps, long elapsedMilliseconds) GetBytesPerSecond()
    {
        long elapsedMilliseconds = Timer.ElapsedMilliseconds;
        long elapsed = elapsedMilliseconds - LastUpdateMilliseconds;
        if (elapsed >= 1000)
        {
            long old = TotalBytesProcessed;

            double bps = TotalBytesProcessed * 8 / (elapsed / 1000.0);
            LastUpdateMilliseconds = elapsedMilliseconds;
            TotalBytesProcessed = 0; // Reset total bytes for the next second
            return (old, bps, elapsed);
        }
        return (TotalBytesProcessed, -1, elapsed); // Less than a second has passed, so don't calculate
    }
}
