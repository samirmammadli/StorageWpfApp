﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class Debt
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        public double Amount { get; set; }

        public double Payed { get; set; }

        public IEnumerable<DebtPayment> DebtPayments { get; set; }
    }
}
