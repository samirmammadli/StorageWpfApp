using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class DebtPayment
    {
        public int Id { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; }

        public int DebtId { get; set; }

        public Debt Debt { get; set; }
    }
}
