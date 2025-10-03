using System.Diagnostics;

namespace Application;

public class LoopStopWatch
{
    private readonly Stopwatch _iterationStopwatch = new();
    private readonly Stopwatch _loopStopwatch = new();

    private readonly int _totalNumberOfIterations;
    private readonly double? _targetDurationMs;
    private int _currentIteration = 0;

    public LoopStopWatch(int numberOfIterations, TimeSpan? targetTotalTime = null)
    {
        _totalNumberOfIterations = numberOfIterations;
        _targetDurationMs = targetTotalTime?.TotalMilliseconds;
    }

    public void IterationHasStarted()
    {
        _iterationStopwatch.Restart();
    }

    public void IterationHasFinished()
    {
        _iterationStopwatch.Stop();
        _currentIteration++;
    }

    public void SleepToMatchTargetTime()
    {
        if (!_targetDurationMs.HasValue || _targetDurationMs <= 0)
        {
            throw new ArgumentException("No positive target time was provided");
        }

        if (_currentIteration < _totalNumberOfIterations - 1)
        {
            double remainingMsLoop = _targetDurationMs.Value - LoopElapsedSoFarMs();
            int remainingIterationsLoop = _totalNumberOfIterations - (_currentIteration + 1);
            double delayMs = remainingMsLoop / remainingIterationsLoop - _iterationStopwatch.ElapsedMilliseconds;
            if (delayMs > 0)
            {
                Thread.Sleep((int)delayMs);
            }
        }
    }

    public double IterationDurationMs()
    {
        return _iterationStopwatch.ElapsedMilliseconds;
    }

    public double LoopElapsedSoFarMs()
    {
        _loopStopwatch.Stop(); // Note: To read the elapsed time, the stopwatches needs to be stopped (and started)
        double elapsedSoFar = _loopStopwatch.ElapsedMilliseconds;
        _loopStopwatch.Start();
        return elapsedSoFar;
    }

    public double RemainingMinutes()
    {
        return RemaningMs() / 1000 / 60;
    }

    private double RemaningMs()
    {
        int remainingIterations = _totalNumberOfIterations - (_currentIteration + 1);
        double averageMsPerIteration = LoopElapsedSoFarMs() / (_currentIteration + 1);
        var remainingTimeMs = averageMsPerIteration * remainingIterations;
        return remainingTimeMs;
    }
}
