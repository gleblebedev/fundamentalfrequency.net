namespace Yin.Net.Tests;

[TestFixture]
public class YinTests
{
    [Test]
    [TestCase(220.0f, 0, 4f)]
    [TestCase(220.0f, 0.2f, 2)]
    [TestCase(330.0f, 0, 2.5f)]
    [TestCase(330.0f, 0.2f, 3.5f)]
    public void TestYin(float frequency, float offset, float fullPeriods)
    {
        int samplesPerSecond = 44100;
        float samplesPerPeriod = samplesPerSecond / frequency;
        float threshold = 0.15f;
        int bufferSize = (int)(samplesPerPeriod * fullPeriods);
        var signal = new double[bufferSize];
        for (int i = 0; i < bufferSize; i++)
        {
            signal[i] = MathF.Cos((MathF.PI * 2.0f)*(i / samplesPerPeriod + offset));
        }

        var result = YinAlgorithm.YinPitch(signal, threshold, samplesPerSecond, out var probability);

        Assert.AreEqual(frequency, result, frequency*threshold);
    }

    [Test]
    [TestCase(220.0f, 0.2f, 1.4f)]
    [TestCase(330.0f, 0.2f, 1.5f)]
    public void TestYinWithInsufficientData(float frequency, float offset, float fullPeriods)
    {
        int samplesPerSecond = 44100;
        float samplesPerPeriod = samplesPerSecond / frequency;
        float threshold = 0.15f;
        int bufferSize = (int)(samplesPerPeriod * fullPeriods);
        var signal = new double[bufferSize];
        for (int i = 0; i < bufferSize; i++)
        {
            signal[i] = MathF.Cos((MathF.PI * 2.0f) * (i / samplesPerPeriod + offset));
        }

        var result = YinAlgorithm.YinPitch(signal, threshold, samplesPerSecond, out var probability);

        Assert.AreEqual(-1, result, 1e-6f);
    }
}