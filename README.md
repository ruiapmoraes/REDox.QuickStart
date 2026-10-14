# RE:Dox Hands-on: Token IR vs. System.Text.Json

**Autor:** Rui Moraes  
**Série:** Engenharia de Dados em Games e Aplicações Corporativas — Parte 2  
**Ambiente:** .NET 10, C# e x64

Este projeto é uma demonstração prática da biblioteca **CAPCOM.REDox / RE:Dox**
em comparação com a biblioteca nativa **System.Text.Json** do .NET.

This project is a hands-on demonstration of **CAPCOM.REDox / RE:Dox** compared
with the native .NET **System.Text.Json** library.

## Objetivo / Purpose

O exemplo mostra como processar um documento JSON de telemetria, extrair
valores, alterar propriedades e inserir itens em arrays sem reconstruir todo o
documento.

The sample processes a telemetry JSON document, extracts values, updates
properties, and inserts array items without rebuilding the entire document.

O foco é observar as diferenças entre:

- `JsonDocument`: DOM baseado em tokens, rápido e somente leitura.
- `JsonNode`: DOM mutável, flexível, mas baseado em objetos gerenciados.
- RE:Dox: DOM de tokens mutável, com `DObject`, `DArray` e `DValue`.

## RE:Dox e Token IR

O RE:Dox transforma dados estruturados em uma representação intermediária
baseada em tokens (*Token Intermediate Representation*). Essa abordagem mantém
as informações estruturais em uma forma compacta e permite leitura, edição e
serialização por meio do mesmo documento.

RE:Dox transforms structured data into a token-based intermediate
representation. This approach keeps structural information compact and allows
reading, editing, and serialization through the same document model.

```text
JSON / JSON5 / outros formatos
              │
              ▼
       RE:Dox Token Engine
              │
              ▼
     DObject / DArray / DValue
              │
              ▼
     Leitura, mutação e serialização
```

## Estrutura principal

```text
Redox_Tests/
├── src/
│   └── REDox.QuickStart/
│       ├── CapcomDox.cs
│       ├── NativeJson.cs
│       ├── SamplePayloads.cs
│       ├── Util.cs
│       └── REDox.QuickStart.csproj
├── tests/
│   └── REDox.QuickStart.Tests/
├── docs/
├── .github/
└── README.md
```

## Requisitos / Requirements

- .NET SDK 10.0 ou superior.
- Visual Studio Code.
- C# Dev Kit ou extensão C# para VS Code.
- Pacote NuGet `CAPCOM.REDox`.

## Instalação / Installation

Na raiz do projeto:

From the project root:

```powershell
dotnet add .\src\REDox.QuickStart\REDox.QuickStart.csproj package CAPCOM.REDox
dotnet restore
```

## Exemplo com RE:Dox

```csharp
using System;
using REDox.Json;

namespace REDox.QuickStart;

public static class CapcomDox
{
    public static void RunSample()
    {
        using var doc = JsonDocument.Parse(SamplePayloads.DeviceTelemetryJson);
        DObject root = doc.RootElement.AsObject();

        string deviceId = root["deviceId"].ToString();
        DObject telemetry = root["telemetry"].AsObject();

        double battery = Util.ReadDouble(
            telemetry["battery"],
            "telemetry.battery");

        int latency = Util.ReadInt32(
            telemetry["latencyMs"],
            "telemetry.latencyMs");

        Console.WriteLine($"Device ID: {deviceId}");
        Console.WriteLine($"Battery: {battery}% | Latency: {latency} ms");

        telemetry["latencyMs"] = 98;
        root["processedAt"] = DateTimeOffset.UtcNow.ToString("O");
        root["processedByEngine"] = "CAPCOM.REDox";

        DArray flags = root["flags"].AsArray();
        flags.Add("ENRICHED_BY_REDOX");

        string modifiedJson = doc.RootElement.ToJsonString();
        Console.WriteLine(modifiedJson);
    }
}
```

### Observação sobre `DValue`

Na versão `1.0.1` do pacote `CAPCOM.REDox`, `DValue` não expõe os métodos
`AsString()`, `AsDouble()` ou `AsInt32()`. Por isso, este projeto usa
`ToString()` combinado com `TryParse` e `CultureInfo.InvariantCulture`.

In version `1.0.1` of the `CAPCOM.REDox` package, `DValue` does not expose
`AsString()`, `AsDouble()`, or `AsInt32()`. Therefore, this project uses
`ToString()` together with `TryParse` and `CultureInfo.InvariantCulture`.

Essa conversão é uma solução compatível com a API publicada, mas pode gerar
uma string intermediária. Portanto, não deve ser descrita como “zero heap
allocation” sem uma medição específica.

This conversion is compatible with the published API, but it may create an
intermediate string. Therefore, it should not be described as “zero heap
allocation” without a specific measurement.

## Comparação com `System.Text.Json.JsonNode`

```csharp
using System.Text.Json.Nodes;

var node = JsonNode.Parse(
    SamplePayloads.DeviceTelemetryJson)!.AsObject();

string deviceId = node["deviceId"]!.GetValue<string>();
var telemetry = node["telemetry"]!.AsObject();
double battery = telemetry["battery"]!.GetValue<double>();

telemetry["latencyMs"] = 98;
node["processedByEngine"] = "System.Text.Json.JsonNode";

var flags = node["flags"]!.AsArray();
flags.Add("ENRICHED_BY_STJ");

string modifiedJson = node.ToJsonString();
```

`JsonNode` é conveniente para edição dinâmica, mas cada objeto, array e nó é
representado por objetos gerenciados. Em cenários de alta frequência, isso
pode aumentar a pressão sobre a Gen 0 do Garbage Collector.

`JsonNode` is convenient for dynamic editing, but each object, array, and node
is represented by managed objects. In high-frequency scenarios, this may
increase Gen 0 Garbage Collector pressure.

## Execução / Running

```powershell
dotnet build
dotnet run --project .\src\REDox.QuickStart\REDox.QuickStart.csproj
dotnet test
```

Execute os benchmarks em configuração `Release` quando quiser comparar
latência ou alocações:

Run benchmarks in `Release` configuration when comparing latency or allocations:

```powershell
dotnet run -c Release --project .\src\REDox.QuickStart\REDox.QuickStart.csproj
```

## Interpretação dos resultados

Os resultados dependem do hardware, da versão do .NET, do compilador, do
formato do payload e do ambiente de execução. Esta demonstração não deve ser
interpretada como uma conclusão universal de desempenho.

Results depend on hardware, .NET version, compiler, payload shape, and runtime
environment. This demonstration should not be interpreted as a universal
performance conclusion.

## Referências / References

- [RE:Dox — repositório oficial](https://github.com/CAPCOM-TD-OSS/REDox)
- [CAPCOM.REDox no NuGet](https://www.nuget.org/packages/CAPCOM.REDox)
- [System.Text.Json — Microsoft Learn](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview)
- [JsonDocument — Microsoft Learn](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument)
- [JsonNode — Microsoft Learn](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

## Licença / License

Este laboratório é uma demonstração técnica. Consulte as licenças do projeto
RE:Dox e das dependências utilizadas antes de redistribuir o código.
