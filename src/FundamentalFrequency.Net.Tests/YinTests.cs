// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

namespace FundamentalFrequency.Tests
{
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
            var yinOptions = new YinOptions(samplesPerSecond, 0.15f);

            var algorithm = new YinAlgorithm(yinOptions);

            float samplesPerPeriod = samplesPerSecond / frequency;
            int bufferSize = (int)(samplesPerPeriod * fullPeriods);
            var signal = new float[bufferSize];
            for (int i = 0; i < bufferSize; i++)
            {
                signal[i] = MathF.Cos((MathF.PI * 2.0f) * (i / samplesPerPeriod + offset));
            }

            var result = algorithm.ExtractFundamentalFrequency(signal);

            Assert.That(result.Frequency, Is.EqualTo(frequency).Within(frequency * yinOptions.Threshold));
        }

        [Test]
        [TestCase(220.0f, 0, 4f)]
        [TestCase(330.0f, 0, 2.5f)]
        public void TestYinWithNoise(float frequency, float offset, float fullPeriods)
        {
            int samplesPerSecond = 44100;
            var yinOptions = new YinOptions(samplesPerSecond, 0.15f);
            var algorithm = new YinAlgorithm(yinOptions);

            float samplesPerPeriod = samplesPerSecond / frequency;
            int bufferSize = (int)(samplesPerPeriod * fullPeriods);
            var signal = new float[bufferSize];
            var rnd = new Random(0);
            for (int i = 0; i < bufferSize; i++)
            {
                signal[i] = MathF.Cos((MathF.PI * 2.0f) * (i / samplesPerPeriod + offset)) + (rnd.NextSingle() - 0.5f) * 0.1f;
            }

            var result = algorithm.ExtractFundamentalFrequency(signal);

            Assert.That(result.Frequency, Is.EqualTo(frequency).Within(frequency * yinOptions.Threshold));
        }

        [Test]
        [TestCase(220.0f, 0.2f, 1.4f)]
        [TestCase(330.0f, 0.2f, 1.5f)]
        public void TestYinWithInsufficientData(float frequency, float offset, float fullPeriods)
        {
            int samplesPerSecond = 44100;
            var yinOptions = new YinOptions(samplesPerSecond, 0.15f);
            var algorithm = new YinAlgorithm(yinOptions);
            float samplesPerPeriod = samplesPerSecond / frequency;
            int bufferSize = (int)(samplesPerPeriod * fullPeriods);
            var signal = new float[bufferSize];
            for (int i = 0; i < bufferSize; i++)
            {
                signal[i] = MathF.Cos((MathF.PI * 2.0f) * (i / samplesPerPeriod + offset));
            }

            var result = algorithm.ExtractFundamentalFrequency(signal);

            Assert.Null(result.Frequency);
        }
    }

}