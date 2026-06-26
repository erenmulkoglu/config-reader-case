using ConfigReader.Converters;
using FluentAssertions;

namespace ConfigReader.Tests;

public class ConfigurationValueConverterTests
{
    [Fact]
    public void Convert_Should_Return_String_Value()
    {
        var result = ConfigurationValueConverter.Convert<string>("SiteName", "soty.io");

        result.Should().Be("soty.io");
    }

    [Fact]
    public void Convert_Should_Return_Int_Value()
    {
        var result = ConfigurationValueConverter.Convert<int>("MaxItemCount", "50");

        result.Should().Be(50);
    }

    [Fact]
    public void Convert_Should_Return_Double_Value()
    {
        var result = ConfigurationValueConverter.Convert<double>("PriceRate", "12.5");

        result.Should().Be(12.5);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Convert_Should_Return_Bool_Value(string value, bool expected)
    {
        var result = ConfigurationValueConverter.Convert<bool>("IsBasketEnabled", value);

        result.Should().Be(expected);
    }
}