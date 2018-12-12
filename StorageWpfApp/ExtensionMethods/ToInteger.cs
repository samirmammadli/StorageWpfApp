using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods
{
    public static class ToInteger
    {
        public static int StringToInteger(this string str)
        {
            int.TryParse(str, out int temp);
            return temp;
        }
    }
}
