using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Exceptions
{
    public sealed class ConfigurationTypeConversionException : Exception
    {
        public ConfigurationTypeConversionException(string key, string value, Type targetType) : base($"Configuration değeri dönüştürülemedi. Key: {key}, Değer: {value}, HedefTip: {targetType.Name}")
        {
        }
    }
}
