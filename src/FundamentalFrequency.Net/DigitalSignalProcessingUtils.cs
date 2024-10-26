// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

using System.Buffers;
using static System.Formats.Asn1.AsnWriter;

namespace FundamentalFrequency;

public class DigitalSignalProcessingUtils
{
    public static void FFT(Span<float> input, Span<SingleComplex> output)
    {
        if (input.Length != output.Length)
        {
            throw new ArgumentException($"Buffer size mismatch between {nameof(input)} and {nameof(output)}",
                nameof(output));
        }

        var arrayPool = ArrayPool<SingleComplex>.Shared;
        var buffer = arrayPool.Rent(input.Length);

        for (int i = 0; i < input.Length; i++)
        {
            buffer[i] = new SingleComplex(input[i], 0);
        }

        FFT(buffer.AsSpan().Slice(0, input.Length), output);

        arrayPool.Return(buffer);
    }

    public static void FFT(Span<SingleComplex> input, Span<SingleComplex> output)
    {
        int n = input.Length;
        if (n <= 1)
            return;

        FFTRecursive(input, output);
    }

    private static void FFTRecursive(Span<SingleComplex> input, Span<SingleComplex> output)
    {
        int n = input.Length;
        if (n == 0)
            return;
        if (n == 1)
        {
            output[0] = input[0];
            return;
        }
            
        var arrayPool = ArrayPool<SingleComplex>.Shared;
        var buffer = arrayPool.Rent(input.Length);
        Span<SingleComplex> even = buffer.AsSpan().Slice(0, n / 2);
        Span<SingleComplex> odd = buffer.AsSpan().Slice(n / 2, n / 2);

        for (int i = 0; i < n / 2; i++)
        {
            even[i] = input[i * 2];
            odd[i] = input[i * 2 + 1];
        }

        var outputBuffer = arrayPool.Rent(input.Length);
        Span<SingleComplex> ffteven = outputBuffer.AsSpan().Slice(0, n / 2);
        Span<SingleComplex> fftodd = outputBuffer.AsSpan().Slice(n / 2, n / 2);

        FFTRecursive(even, ffteven);
        FFTRecursive(odd, fftodd);

        for (int k = 0; k < n / 2; k++)
        {
            SingleComplex t = SingleComplex.Exp(new SingleComplex(0.0f, - 2.0f * MathF.PI * k / n)) * fftodd[k];
            output[k] = ffteven[k] + t;
            output[k + n / 2] = ffteven[k] - t;
        }

        arrayPool.Return(buffer);
        arrayPool.Return(outputBuffer);
    }

    public static void IFFT(Span<SingleComplex> input, Span<float> output)
    {
        int n = input.Length;

        var arrayPool = ArrayPool<SingleComplex>.Shared;
        var buffer = arrayPool.Rent(input.Length);

        IFFT(input, buffer.AsSpan().Slice(0, input.Length));

        for (int i = 0; i < n; i++)
        {
            output[i] = buffer[i].Real;
        }
        arrayPool.Return(buffer);
    }

    public static void IFFT(Span<SingleComplex> input, Span<SingleComplex> output)
    {
        int n = input.Length;
        if (n < 1)
            return;

        IFFTRecursive(input, output);

        float scale = 1.0f / n;
        for (int i = 0; i < n; i++)
        {
            output[i] *= scale;
        }
    }

    private static void IFFTRecursive(Span<SingleComplex> input, Span<SingleComplex> output)
    {
        int n = input.Length;
        if (n == 0)
            return;
        if (n == 1)
        {
            output[0] = input[0];
            return;
        }

        var arrayPool = ArrayPool<SingleComplex>.Shared;
        var buffer = arrayPool.Rent(input.Length);
        Span<SingleComplex> even = buffer.AsSpan().Slice(0, n / 2);
        Span<SingleComplex> odd = buffer.AsSpan().Slice(n / 2, n / 2);
        for (int i = 0; i < n / 2; i++)
        {
            even[i] = input[i * 2];
            odd[i] = input[i * 2 + 1];
        }
        var outputBuffer = arrayPool.Rent(input.Length);
        Span<SingleComplex> ffteven = outputBuffer.AsSpan().Slice(0, n / 2);
        Span<SingleComplex> fftodd = outputBuffer.AsSpan().Slice(n / 2, n / 2);


        IFFTRecursive(even, ffteven);
        IFFTRecursive(odd, fftodd);

        for (int k = 0; k < n / 2; k++)
        {
            SingleComplex t = SingleComplex.Exp(new SingleComplex(0.0f, 2.0f * MathF.PI * k / n)) * fftodd[k];
            output[k] = ffteven[k] + t;
            output[k + n / 2] = ffteven[k] - t;
        }

        arrayPool.Return(buffer);
        arrayPool.Return(outputBuffer);
    }
}