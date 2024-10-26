// MIT License
// 
// Copyright (c) 2023 Gleb Lebedev
// 
// This software is licensed under the MIT License.
// For more details, visit: https://opensource.org/licenses/MIT

namespace FundamentalFrequency;

public readonly struct Result
{
    /// <summary>
    /// Result indicating that extraction failed.
    /// </summary>
    public static readonly Result Failure = new Result();

    public Result(float? frequency, float? probability = null)
    {
        Frequency = frequency;
        Probability = probability;
    }

    /// <summary>
    /// Extracted frequency.
    /// </summary>
    public float? Frequency { get; }

    /// <summary>
    /// Probability of correct frequency extraction.
    /// </summary>
    public float? Probability { get; }
}