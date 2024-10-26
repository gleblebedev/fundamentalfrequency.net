namespace FundamentalFrequency.Tests;

[TestFixture]
public class FFTTests
{
    [Test]
    [TestCase(0.0f)]
    [TestCase(0.25f)]
    public void TestSinWave(float phaseShift)
    {
        var input = new float[128];
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = MathF.Sin((i / (float)input.Length + phaseShift) * MathF.PI * 2.0f);
        }

        var output = new SingleComplex[input.Length];
        DigitalSignalProcessingUtils.FFT(input, output);


        var inverseOutput = new float[input.Length];
        DigitalSignalProcessingUtils.IFFT(output, inverseOutput);
    }
}