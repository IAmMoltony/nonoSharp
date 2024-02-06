using System.Collections.Generic;
using System.Linq;

namespace NonoSharp;

public class FPSCounter
{
    public long TotalFrames { get; private set; }
    public float TotalSeconds { get; private set; }
    public float AverageFPS { get; private set; }
    public float CurrentFPS { get; private set; }

    private const int MAX_SAMPLES = 100;
    private Queue<float> _sampleBuf;

    public FPSCounter()
    {
        _sampleBuf = new();
    }

    public void Update(float deltaTime)
    {
        CurrentFPS = 1.0f / deltaTime;
        _sampleBuf.Enqueue(CurrentFPS);

        if (_sampleBuf.Count > MAX_SAMPLES)
        {
            _sampleBuf.Dequeue();
            AverageFPS = _sampleBuf.Average(i => i);
        }
        else
            AverageFPS = CurrentFPS;

        TotalFrames++;
        TotalSeconds += deltaTime;
    }
}
