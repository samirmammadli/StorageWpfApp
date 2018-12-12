using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class SingleOrder : ObservableObject
    {
        public int Id { get; set; }

        public int ConsignmentId { get; set; }

        public Consignment Consignment { get; set; }

        private double _discount;
        public double Discount

        {
            get => _discount;
            set
            {
                if (value >= 0 && value <= Consignment.SellingPrice)
                {
                    Set(ref _discount, value);

                    RaisePropertyChanged(nameof(Sum));
                }
            }
        }
        
        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                if (value > 0)
                {
                    Set(ref _count, value);

                    RaisePropertyChanged(nameof(Sum));
                }
            }
        }

        public int InvoiceId { get; set; }

        public Invoice Invoice { get; set; }

        [NotMapped]
        public double Sum
        {
            get
            {
                return (Consignment.SellingPrice - Discount) * Count;
            }
        }

    }
}
