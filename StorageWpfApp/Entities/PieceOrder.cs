namespace StorageWpfApp.Entities
{
    public class PieceOrder
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
