using System.Diagnostics;

namespace StreamMaster.Streams.Domain.Metrics;

public class PerformanceBpsMetrics
{
    private Stopwatch Timer { get; }
    private long TotalBytesProcessed { get; set; }

    public PerformanceBpsMetrics()
    {
        Timer = new Stopwatch();
        Timer.Start();
        TotalBytesProcessed = 0;
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

    public double GetBitsPerSecond()
    {
        long elapsedMilliseconds = Timer.ElapsedMilliseconds;

        if (elapsedMilliseconds >= 1000)
        {
            var bps = TotalBytesProcessed * 8 / (elapsedMilliseconds / 1000.0);
            return bps;
        }
        return -1;
    }
}
