namespace REDox.QuickStart;

public static class SamplePayloads
{
    public const string DeviceTelemetryJson = """
    {
      "deviceId": "TRACKER-009-ALPHA",
      "firmware": "2.4.1",
      "status": "ACTIVE",
      "telemetry": {
        "battery": 87.5,
        "temperature": 32.1, 
        "latencyMs": 142
      },
      "flags": ["ONLINE", "GPS_LOCK"]
    }
    """;
}