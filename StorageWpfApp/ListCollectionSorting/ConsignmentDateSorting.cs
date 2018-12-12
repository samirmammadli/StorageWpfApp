using StorageWpfApp.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ListCollectionSorting
{
    public class ConsignmentDateSorting : IComparer
    {
        public int Compare(object x, object y)
        {
            var consX = x as Consignment;
            var consY = y as Consignment;
            if (consX.Date == consY.Date)
                return 0;
            return consX.Date > consY.Date ? 1 : -1;
        }
    }
}
