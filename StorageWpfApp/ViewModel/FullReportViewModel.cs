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

        private string _totalIncome;
        public string TotalIncome
        {
            get { return _totalIncome; }
            set => Set(ref _totalIncome, value);
        }

        private string _totalSoldProductsAmount;
        public string TotalSoldProductsAmount
        {
            get { return _totalSoldProductsAmount; }
            set => Set(ref _totalSoldProductsAmount, value);
        }

        private void Calculate()
        {
            var invoices = Invoices.Cast<Invoice>();
            var totalInvoicesSold = (invoices?.Sum(x => x.AmountToPay)).Value;

            var singleProductsSellingPrice = invoices.Where(i => i.SingleOrders != null).SelectMany(x => x.SingleOrders).Sum(x => x.Consignment.PurchasePrice * x.Count);
            var pieceProductsSellingPrice =  invoices.Where(i => i.PieceOrders != null).SelectMany(x => x.PieceOrders).Sum(x => (x.Consignment.PurchasePrice / x.Consignment.Product.PieceQuantity.Value) * x.Count);

            //MessageBox.Show(singleProductsSellingPrice.ToString());
            //MessageBox.Show(pieceProductsSellingPrice.ToString());


            TotalSoldProductsAmount = invoices.Sum(i => i.AmountToPay).ToString("#,###0.00") + " AZN";
            TotalIncome = (totalInvoicesSold - singleProductsSellingPrice - pieceProductsSellingPrice).ToString("#,###0.00") + " AZN";
        }


        public FullReportViewModel(ProjectContext db)
        {
            _db = db;
            Invoices  = new ListCollectionView( _db.Invoices.Local.ToObservableCollection());

            Calculate();
        }

    }
}
