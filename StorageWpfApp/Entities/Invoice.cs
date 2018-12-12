using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        public int? ClientId { get; set; }

        public Client Client { get; set; }

        public DateTime Date { get; set; }

        public double TotalDiscount { get; set; }
         
        public double TotalAmount { get; set; }

        public double TotalCash { get; set; }

        public int? DebtId { get; set; }

        public Debt Debt { get; set; }

        public IEnumerable<SingleOrder> SingleOrders { get; set; }

        public IEnumerable<PieceOrder> PieceOrders { get; set; }
    }
}
