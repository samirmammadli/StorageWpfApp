using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class SingleOrder
    {
        public int Id { get; set; }

        public int ConsignmentId { get; set; }

        public Consignment Consignment { get; set; }

        public double Discount { get; set; }
        
        public int Count { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

    }
}
