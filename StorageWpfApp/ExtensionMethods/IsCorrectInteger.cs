using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods
{
    public static class IsCorrectInteger
    {
        public static bool IsCorrectInt(this string value)
        {
            int temp = 0;
            return (value == "" || int.TryParse(value, out temp)) && temp >=0 ;
        }
    }
}
