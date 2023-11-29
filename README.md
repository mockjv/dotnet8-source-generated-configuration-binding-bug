# Microsoft.Extensions.Configuration 8.x `EnableConfigurationBindingGenerator` Bug?

This repository tests and shows a behavioral difference in results of `IConfiguration.Get<TType>()` when `TType` is a
class containing either an `Enum` or a `TimeSpan` while `EnableConfigurationBindingGenerator` is set to true.

Basically, the effective "default values" specified in the class do not appear to be honored.

So for example, if you have the following configuration classes:

```csharp
public class MyConfiguration
{
    public string Value1 { get; set; } = "default1";
    public string Value2 { get; set; } = "default2";
    public TimeSpan Value3 { get; set; } = TimeSpan.FromHours(1);
    public Uri Value4 { get; set; } = new("https://default:5001");
    public MyEnum Value5 { get; set; } = MyEnum.enum4;
}

public enum MyEnum
{
    enum1,
    enum2,
    enum3,
    enum4
}
```

With the following `appsettings.json`:

```json
{
  "TopSection" : {
    "Section": {
      "StrVal1": "changed!"
    }
  }
}
```

Then you attempt to get those "settings" where only one value was actually changed:

```csharp
Console.WriteLine(JsonSerializer.Serialize(
    config.GetSection("TopSection:Section").Get<MyConfiguration>()
), new JsonSerializerOptions{WriteIndented = true});
```

And you'll observe the following results:

- EnableConfigurationBindingGenerator = false *(expected results)*
  ```json
  {
    "StrVal1": "changed!",
    "StrVal2": "default2",
    "TimeSpanVal1": "00:00:00",
    "UriVal1": "https://default:5001",
    "EnumVal1": 0
  }
  ```

- EnableConfigurationBindingGenerator = true
  ```json
  {
    "StrVal1": "changed!",
    "StrVal2": "default2",
    "TimeSpanVal1": "01:00:00",
    "UriVal1": "https://default:5001",
    "EnumVal1": 3
  }
  ```

## Setup

Your system needs _at least_ the .NET 8 SDK, but for the sake of proving the changes.

## Running the Example

To show the changed behavior, simply run the project as-is:

```shell
❯ dotnet run
Get<TType>()
{
  "StrVal1": "changed!",
  "StrVal2": "default2",
  "TimeSpanVal1": "00:00:00",
  "UriVal1": "https://default:5001",
  "EnumVal1": 0
}

Bind()
{
  "StrVal1": "changed!",
  "StrVal2": "default2",
  "TimeSpanVal1": "01:00:00",
  "UriVal1": "https://default:5001",
  "EnumVal1": 3
}
```

To show the previous behavior, change `EnableConfigurationBindingGenerator` to `false` and re-run the project:

```shell
❯ dotnet run
Get<TType>()
{
  "StrVal1": "changed!",
  "StrVal2": "default2",
  "TimeSpanVal1": "01:00:00",
  "UriVal1": "https://default:5001",
  "EnumVal1": 3
}

Bind()
{
  "StrVal1": "changed!",
  "StrVal2": "default2",
  "TimeSpanVal1": "01:00:00",
  "UriVal1": "https://default:5001",
  "EnumVal1": 3
}
```
