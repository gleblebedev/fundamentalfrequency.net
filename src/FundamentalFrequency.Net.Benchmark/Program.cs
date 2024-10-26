using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace FundamentalFrequency.Net
{
    public class YinBenchmark
    {
        private double[] signal;

        public YinBenchmark()
        {
            signal = new double[1024];
            Random rand = new Random();
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] = rand.NextDouble();
            }
        }

        [Benchmark]
        public void BenchmarkYinPitch()
        {
            YinAlgorithm.YinPitch(signal, 0.1, 44100, out var probability);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<YinBenchmark>();
        }
    }

}