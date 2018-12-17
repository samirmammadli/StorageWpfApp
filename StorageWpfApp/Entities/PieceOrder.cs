using GalaSoft.MvvmLight;
using StorageWpfApp.ExtensionMethods;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageWpfApp.Entities
{
    public class PieceOrder : ObservableObject
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
                if (value >= 0 && value <= Consignment.PiecePrice.Value)
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
                    if (value > Consignment.CurrentPieceQuantity)
                        Set(ref _count, Consignment.CurrentPieceQuantity);
                    else
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
                return (Consignment.PiecePrice.Value - Discount) * Count;
            }
        }

    }
}
