// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

namespace FundamentalFrequency
{
    public class YinOptions
    {
        public int SamplesPerSecond { get; }
        public float Threshold { get; }

        public YinOptions(int samplesPerSecond, float threshold)
        {
            SamplesPerSecond = samplesPerSecond;
            Threshold = threshold;
        }
    }
}