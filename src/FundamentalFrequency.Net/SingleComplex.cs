// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT
//
// Partially based on:
// Copyright (c) .NET Foundation
// https://github.com/dotnet/runtime/tree/main/src/libraries/System.Runtime.Numerics/src/System/Numerics/Complex.cs


using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FundamentalFrequency;

/// <summary>
/// Single precision (float) complex number.
/// </summary>
public readonly struct SingleComplex
{
    private readonly float _real;
    private readonly float _imaginary;

    public SingleComplex(float real, float imaginary)
    {
        _real = real;
        _imaginary = imaginary;
    }

    public float Real => _real;
    public float Imaginary => _imaginary;

    public static SingleComplex Exp(SingleComplex value)
    {
        float expReal = MathF.Exp(value._real);
        float cosImaginary = expReal * MathF.Cos(value._imaginary);
        float sinImaginary = expReal * MathF.Sin(value._imaginary);
        return new SingleComplex(cosImaginary, sinImaginary);
    }

    public static SingleComplex operator *(SingleComplex left, SingleComplex right)
    {
        // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
        float resultRealpart = (left._real * right._real) - (left._imaginary * right._imaginary);
        float resultImaginaryPart = (left._imaginary * right._real) + (left._real * right._imaginary);
        return new SingleComplex(resultRealpart, resultImaginaryPart);
    }

    public static SingleComplex operator *(SingleComplex left, float right)
    {
        return new SingleComplex(left._real * right, left._imaginary * right);
    }

    public static SingleComplex operator +(SingleComplex left, SingleComplex right)
    {
        return new SingleComplex(left._real + right._real, left._imaginary + right._imaginary);
    }

    public static SingleComplex operator +(SingleComplex left, float right)
    {
        return new SingleComplex(left._real + right, left._imaginary);
    }

    public static SingleComplex operator +(float left, SingleComplex right)
    {
        return new SingleComplex(left + right._real, right._imaginary);
    }

    public static SingleComplex operator -(SingleComplex left, SingleComplex right)
    {
        return new SingleComplex(left._real - right._real, left._imaginary - right._imaginary);
    }

    public static SingleComplex operator -(SingleComplex left, float right)
    {
        return new SingleComplex(left._real - right, left._imaginary);
    }

    public static SingleComplex operator -(float left, SingleComplex right)
    {
        return new SingleComplex(left - right._real, -right._imaginary);
    }

    public override int GetHashCode() => HashCode.Combine(_real, _imaginary);

    public override string ToString() => ToString(null, null);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) => ToString(format, null);

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? provider)
    {
        // $"<{m_real.ToString(format, provider)}; {m_imaginary.ToString(format, provider)}>";
        var handler = new DefaultInterpolatedStringHandler(4, 2, provider, stackalloc char[512]);
        handler.AppendLiteral("<");
        handler.AppendFormatted(_real, format);
        handler.AppendLiteral("; ");
        handler.AppendFormatted(_imaginary, format);
        handler.AppendLiteral(">");
        return handler.ToStringAndClear();
    }
}