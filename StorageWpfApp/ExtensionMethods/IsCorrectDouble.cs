using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods
{
    public static class IsCorrectDouble
    {
        public static bool IsDouble(this string value)
        {
            return (value.Length > 1 && value.IndexOf('.') == value.Length - 1) ||
                                        value == "" ||
                    double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.Float, CultureInfo.InvariantCulture, out double temp);
        }
    }
}


