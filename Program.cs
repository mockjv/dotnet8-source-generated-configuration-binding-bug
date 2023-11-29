using System.Text.Json;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationManager().AddJsonFile("appsettings.json").Build();
var jsonOpts = new JsonSerializerOptions {WriteIndented = true};

// Get<TType>()
var getResult = config.GetSection("Section").Get<MyConfiguration>();
Console.WriteLine("Get<TType>()");
Console.WriteLine(JsonSerializer.Serialize(getResult, jsonOpts));

// Bind()
var bindResult = new MyConfiguration();
config.Bind("Section", bindResult);
Console.WriteLine($"{Environment.NewLine}Bind()");
Console.WriteLine(JsonSerializer.Serialize(bindResult, jsonOpts));

public class MyConfiguration
{
    public int IntVal1 { get; set; } = 8080;
    public string StrVal1 { get; set; } = "default1";
    public string StrVal2 { get; set; } = "default2";
    public TimeSpan TimeSpanVal1 { get; set; } = TimeSpan.FromHours(1);
    public Uri UriVal1 { get; set; } = new("https://default:5001");
    public MyEnum EnumVal1 { get; set; } = MyEnum.enum4;
}

public enum MyEnum
{
    enum1,
    enum2,
    enum3,
    enum4
}