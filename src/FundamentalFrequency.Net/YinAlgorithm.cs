// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

namespace FundamentalFrequency
{
    public interface IFundamentalFrequencyExtractor
    {

    }

    public class YinAlgorithm : IFundamentalFrequencyExtractor
    {
        private readonly YinOptions _options;

        public YinAlgorithm(YinOptions options)
        {
            _options = options;
        }

        public Result ExtractFundamentalFrequency(float[] signal)
        {
            float[] difference = DifferenceFunction(signal, signal.Length / 2);
            float[] cmndf = CumulativeMeanNormalizedDifferenceFunction(difference);
            float probability = 0;
            int tau = AbsoluteThreshold(cmndf, _options.Threshold, out probability);

            if (tau != -1)
            {
                return new Result(_options.SamplesPerSecond / (float)ParabolicInterpolation(cmndf, tau),
                    (float)probability);
            }

            return Result.Failure; // No pitch found
        }

        private float[] DifferenceFunction(float[] signal, int lag)
        {
            int N = signal.Length;
            float[] difference = new float[N - lag];

            for (int tau = 0; tau < lag; tau++)
            {
                difference[tau] = 0;
                for (int j = 0; j < N - lag; j++)
                {
                    float delta = signal[j] - signal[j + tau];
                    difference[tau] += delta * delta;
                }
            }

            return difference;
        }

        private float[] CumulativeMeanNormalizedDifferenceFunction(float[] difference)
        {
            int N = difference.Length;
            float[] cmndf = new float[N];
            cmndf[0] = 1;

            for (int tau = 1; tau < N; tau++)
            {
                float cumulativeSum = 0;
                for (int j = 1; j <= tau; j++)
                {
                    cumulativeSum += difference[j];
                }

                cmndf[tau] = difference[tau] * tau / cumulativeSum;
            }

            return cmndf;
        }

        private int AbsoluteThreshold(float[] cmndf, float threshold, out float probability)
        {
            for (int tau = 0; tau < cmndf.Length; tau++)
            {
                if (cmndf[tau] < threshold)
                {
                    while (tau + 1 < cmndf.Length && cmndf[tau + 1] < cmndf[tau])
                    {
                        tau++;
                    }

                    probability = 1.0f - cmndf[tau];
                    return tau;
                }
            }

            probability = 0.0f;
            return -1; // If no valid period is found
        }

        private float ParabolicInterpolation(float[] cmndf, int tau)
        {
            int x0 = tau > 0 ? tau - 1 : tau;
            int x2 = tau < cmndf.Length - 1 ? tau + 1 : tau;

            if (x0 == tau)
            {
                return cmndf[tau] <= cmndf[x2] ? tau : x2;
            }

            if (x2 == tau)
            {
                return cmndf[tau] <= cmndf[x0] ? tau : x0;
            }

            float a = cmndf[x0];
            float b = cmndf[tau];
            float c = cmndf[x2];

            return tau + 0.5f * (a - c) / (a - 2 * b + c);
        }
    }

}