using GalaSoft.MvvmLight;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ViewModel
{
    class ComposeInvoiceViewModel : ViewModelBase
    {
        private ProjectContext _db;

        public List<Product> Products { get; set; }

        private ObservableCollection<PieceOrder> _pieceOrders;
        public ObservableCollection<PieceOrder> PieceOrders
        {
            get => _pieceOrders;
            set => Set(ref _pieceOrders, value);
        }

        private ObservableCollection<SingleOrder> singleOrders;
        public ObservableCollection<SingleOrder> SingleOrders
        {
            get => singleOrders;
            set => Set(ref singleOrders, value);
        }

        public Invoice Invoice { get; set; }


        public ComposeInvoiceViewModel(ProjectContext db)
        {
            _db = db;
            Invoice = new Invoice();
            Products = _db.Products.Local.ToList();
        }
    }
}
