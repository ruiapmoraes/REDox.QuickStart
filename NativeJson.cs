using System;
using System.Text.Json.Nodes;

namespace REDox.QuickStart;

public static class NativeJson
{
    public static void RunSample()
    {
        Console.WriteLine("\n--- [System.Text.Json JsonNode: Native Mutation] ---");

        // 1. Parse into standard mutable tree (allocates class instances across nodes)
        var node = JsonNode.Parse(SamplePayloads.DeviceTelemetryJson)!.AsObject();

        string deviceId = node["deviceId"]!.GetValue<string>();
        var telemetry = node["telemetry"]!.AsObject();
        double battery = telemetry["battery"]!.GetValue<double>();

        // 2. Equivalent mutations
        telemetry["latencyMs"] = 98;
        node.Add("processedAt", DateTime.UtcNow.ToString("o"));
        node.Add("processedByEngine", "System.Text.Json.JsonNode");

        var flags = node["flags"]!.AsArray();
        flags.Add("ENRICHED_BY_STJ");

        string modifiedJson = node.ToJsonString();

        Console.WriteLine($"Device ID   : {deviceId}");
        Console.WriteLine($"Battery     : {battery}%");
        Console.WriteLine($"Total Flags : {flags.Count}");
        Console.WriteLine("\nMutated Payload via JsonNode:");
        Console.WriteLine(modifiedJson);
    }
}