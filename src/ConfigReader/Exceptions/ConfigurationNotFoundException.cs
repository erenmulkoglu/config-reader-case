using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Exceptions
{
    public sealed class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string key) : base($"Configuration değeri boş olamaz. Key: {key}")
        {
        }
    }
    }