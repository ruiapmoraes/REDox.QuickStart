using System;
using System.Globalization;
using REDox.Json;

namespace REDox.QuickStart;

/// <summary>
/// Validation of numeric types
/// </summary>
public static class Util
{
    public static double ReadDouble(DValue value, string propertyName)
    {
        string? rawValue = value.ToString();

        if (double.TryParse(
                rawValue,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result))
        {
            return result;
        }

        throw new FormatException(
            $"The property '{propertyName}' must contain a valid double value. " +
            $"Received: '{rawValue}'.");
    }

    public static int ReadInt32(DValue value, string propertyName)
    {
        string? rawValue = value.ToString();

        if (int.TryParse(
                rawValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result))
        {
            return result;
        }

        throw new FormatException(
            $"The property '{propertyName}' must contain a valid integer value. " +
            $"Received: '{rawValue}'.");
    }
}