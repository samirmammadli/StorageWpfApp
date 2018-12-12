using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageWpfApp.Entities
{
    public class Consignment
    {
        public DateTime? Date { get; set; }

        public int Id { get; set; }

        public int Quantity { get; set; }

        public double PurchasePrice { get; set; }

        public double SellingPrice { get; set; }

        public long? ProductId { get; set; }

        public Product Product { get; set; }

        public int CurrentPieceQuantity { get; set; }

        public bool IsPieceAllowed { get; set; }

        public double? PiecePrice { get; set; }

        public IEnumerable<Invoice> Invoices { get; set; }


        [NotMapped]
        public string TotalPieceQuantity
        {
            get
            {
                if (PiecePrice == null || Product.PieceQuantity == null)
                    return "";

                return (Product.PieceQuantity * Quantity).ToString();
            }
        }

    }

}


//партия