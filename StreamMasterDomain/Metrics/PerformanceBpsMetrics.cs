using System.Diagnostics;

namespace StreamMasterDomain.Metrics;

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

    public double GetBytesPerSecond()
    {
        long elapsedMilliseconds = Timer.ElapsedMilliseconds;
        if (elapsedMilliseconds - LastUpdateMilliseconds >= 1000)
        {
            double bps = TotalBytesProcessed / ((elapsedMilliseconds - LastUpdateMilliseconds) / 1000.0);
            LastUpdateMilliseconds = elapsedMilliseconds;
            TotalBytesProcessed = 0; // Reset total bytes for the next second
            return bps;
        }
        return -1; // Less than a second has passed, so don't calculate
    }
}
