﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageWpfApp.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public double Debit { get; set; }

        public ICollection<Debt> Debts { get; set; }

        [NotMapped]
        public double DebitSum
        {
            get
            {
                if (Debts != null && Debts.Count() > 0)
                {
                    var payments = Debts.SelectMany(x => x.DebtPayments);

                    if (payments != null && payments.Any())
                       return Debts.Sum(x => x.Amount) - payments.Sum(x => x.Amount);
                    else
                        return Debts.Sum(x => x.Amount);
                }
                return 0;
            }
        }

    }
}
