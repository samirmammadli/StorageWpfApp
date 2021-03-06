﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public double TotalPayed { get; set; }

        [NotMapped]
        public double AmountToPay
        {
            get => TotalAmount - TotalDiscount;
        }

        [NotMapped]
        public double DebtDisplay
        {
            get => AmountToPay - TotalPayed;
        }

        public Debt Debt { get; set; }

        public IEnumerable<SingleOrder> SingleOrders { get; set; }

        public IEnumerable<PieceOrder> PieceOrders { get; set; }
    }
}
