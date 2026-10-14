using System;
using System.Globalization;
using REDox.Json;

namespace REDox.QuickStart;

public static class CapcomDox
{
    public static void RunSample()
    {
        Console.WriteLine("--- [Capcom RE:Dox: In-Place Reading & Mutation] ---");

        // 1. Parse the document directly into the token engine
        using var doc = JsonDocument.Parse(SamplePayloads.DeviceTelemetryJson);
        DObject root = doc.RootElement.AsObject();

        // 2. Extract values from DValue
        string? deviceId = root["deviceId"].ToString();
        DObject telemetry = root["telemetry"].AsObject();
        double battery = Util.ReadDouble(telemetry["battery"],"telemetry.battery");
        int latency = Util.ReadInt32(telemetry["latencyMs"],"telemetry.latencyMs");

        Console.WriteLine($"Device ID   : {deviceId}");
        Console.WriteLine($"Battery     : {battery:F1}% | Original Latency: {latency} ms");

        // 3. Update scalar values and inject dynamic fields in place
        telemetry["latencyMs"] = 98;

        root["processedAt"] = DateTimeOffset.UtcNow.ToString("O");
        root["processedByEngine"] = "Capcom.REDox.v1";

        // 4. Insert a value into the existing array in place
        DArray flags = root["flags"].AsArray();
        flags.Add("ENRICHED_BY_REDOX");

        // 5. Serialize the mutated token structure
        string modifiedJson = doc.RootElement.ToJsonString();

        Console.WriteLine($"Total Flags : {flags.Count}");
        Console.WriteLine("\nMutated Payload via RE:Dox:");
        Console.WriteLine(modifiedJson);
    }

    private static double ParseDouble(string value, string propertyName)
    {
        if (double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result))
        {
            return result;
        }

        throw new FormatException(
            $"The property '{propertyName}' does not contain a valid double value: '{value}'.");
    }

    private static int ParseInt32(string value, string propertyName)
    {
        if (int.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result))
        {
            return result;
        }

        throw new FormatException(
            $"The property '{propertyName}' does not contain a valid Int32 value: '{value}'.");
    }
}