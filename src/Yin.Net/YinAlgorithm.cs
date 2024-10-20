namespace Yin.Net;

public class YinAlgorithm
{
    public static double[] DifferenceFunction(double[] signal, int lag)
    {
        int N = signal.Length;
        double[] difference = new double[N - lag];

        for (int tau = 0; tau < lag; tau++)
        {
            difference[tau] = 0;
            for (int j = 0; j < N - lag; j++)
            {
                double delta = signal[j] - signal[j + tau];
                difference[tau] += delta * delta;
            }
        }

        return difference;
    }

    public static double[] CumulativeMeanNormalizedDifferenceFunction(double[] difference)
    {
        int N = difference.Length;
        double[] cmndf = new double[N];
        cmndf[0] = 1;

        for (int tau = 1; tau < N; tau++)
        {
            double cumulativeSum = 0;
            for (int j = 1; j <= tau; j++)
            {
                cumulativeSum += difference[j];
            }

            cmndf[tau] = difference[tau] * tau / cumulativeSum;
        }

        return cmndf;
    }

    public static int AbsoluteThreshold(double[] cmndf, double threshold, out double probability)
    {
        for (int tau = 0; tau < cmndf.Length; tau++)
        {
            if (cmndf[tau] < threshold)
            {
                while (tau + 1 < cmndf.Length && cmndf[tau + 1] < cmndf[tau])
                {
                    tau++;
                }

                probability = 1.0 - cmndf[tau];
                return tau;
            }
        }

        probability = 0.0;
        return -1; // If no valid period is found
    }

    public static double ParabolicInterpolation(double[] cmndf, int tau)
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

        double a = cmndf[x0];
        double b = cmndf[tau];
        double c = cmndf[x2];

        return tau + 0.5 * (a - c) / (a - 2 * b + c);
    }

    public static double YinPitch(double[] signal, double threshold, int samplingRate, out double probability)
    {
        double[] difference = DifferenceFunction(signal, signal.Length/2);
        double[] cmndf = CumulativeMeanNormalizedDifferenceFunction(difference);
        probability = 0;
        int tau = AbsoluteThreshold(cmndf, threshold, out probability);

        if (tau != -1)
        {
            return samplingRate / ParabolicInterpolation(cmndf, tau);
        }

        return -1; // No pitch found
    }
}