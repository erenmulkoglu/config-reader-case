using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ConfigReader.Exceptions;

namespace ConfigReader.Converters
{


    /// Storage üzerinde string olarak tutulan konfigürasyon değerlerini istenen tipe dönüştürür
    /// Desteklenen tipler: string, int, double ve bool
    public static class ConfigurationValueConverter
    {


        /// Verilen string değeri generic tip parametresine göre dönüştürür
        /// Bool için "1/0" ve "true/false" değerlerini destekler
        public static T Convert<T>(string key, string value)
        {
            try
            {
                var targetType = typeof(T);

                if (targetType == typeof(string))
                    return (T)(object)value;

                if (targetType == typeof(int))
                    return (T)(object)int.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(double))
                    return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(bool))
                    return (T)(object)ConvertToBoolean(value);

                throw new NotSupportedException($"Desteklenmeyen configuration tipi: {targetType.Name}");
            }
            catch
            {
                throw new ConfigurationTypeConversionException(key, value, typeof(T));
            }
        }

        private static bool ConvertToBoolean(string value)
        {
            if (value == "1")
                return true;

            if (value == "0")
                return false;

            return bool.Parse(value);
        }
    }
}
