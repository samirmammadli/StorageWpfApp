using GalaSoft.MvvmLight;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace StorageWpfApp.ViewModel
{
    public class FullReportViewModel : ViewModelBase
    {
        private ProjectContext _db { get; }

        private ListCollectionView _invoices;
        public ListCollectionView Invoices
        {
            get { return _invoices; }
            set { _invoices = value; }
        }

        private double _totalIncome;
        public double TotalIncome
        {
            get { return _totalIncome; }
            set => Set(ref _totalIncome, value);
        }

        private void Calculate()
        {
            var invoices = Invoices.Cast<Invoice>();
            var totalInvoicesSold = (invoices?.Sum(x => x.AmountToPay)).Value;

            var singleProductsSellingPrice = invoices.Where(i => i.SingleOrders != null).SelectMany(x => x.SingleOrders).Sum(x => x.Consignment.PurchasePrice * x.Count);
            var pieceProductsSellingPrice =  invoices.Where(i => i.PieceOrders != null).SelectMany(x => x.PieceOrders).Sum(x => (x.Consignment.PurchasePrice / x.Consignment.Product.PieceQuantity.Value) * x.Count);

            //MessageBox.Show(singleProductsSellingPrice.ToString());
            //MessageBox.Show(pieceProductsSellingPrice.ToString());

            TotalIncome = totalInvoicesSold - singleProductsSellingPrice - pieceProductsSellingPrice;
        }


        public FullReportViewModel(ProjectContext db)
        {
            _db = db;
            Invoices  = new ListCollectionView( _db.Invoices.Local.ToObservableCollection());

            Calculate();
        }

    }
}
