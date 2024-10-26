// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace FundamentalFrequency
{
    public class YinBenchmark
    {
        private float[] _signal;
        YinAlgorithm _yin;

        public YinBenchmark()
        {
            _yin = new YinAlgorithm(new YinOptions(44100, 0.1f));
            _signal = new float[1024];
            Random rand = new Random();
            for (int i = 0; i < _signal.Length; i++)
            {
                _signal[i] = rand.NextSingle();
            }
        }

        [Benchmark]
        public void BenchmarkYinPitch()
        {
            _yin.ExtractFundamentalFrequency(_signal);
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