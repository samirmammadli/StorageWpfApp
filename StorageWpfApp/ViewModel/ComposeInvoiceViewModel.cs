using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageWpfApp.ViewModel
{
    class ComposeInvoiceViewModel : ViewModelBase
    {
        private ProjectContext _db;

        private Client _client;
        public Client Client
        {
            get => _client;
            set => Set(ref _client, value);
        }


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

        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get => _invoiceDate;
            set => Set(ref _invoiceDate, value);
        }

        private string _count = "";
        public string Count
        {
            get => _count;
            set
            {
                if (value.IsCorrectInt())
                {
                    Set(ref _count, value);
                }
            }
        }

        private RelayCommand<Window> _addSingleCons;
        public RelayCommand<Window> AddSingleCons
        {
            get => _addSingleCons ?? (_addSingleCons = new RelayCommand<Window>(
                wnd =>
                {
                    var context = new ConsignmentsViewModel(_db, ConsignmentSelectionType.Signle);
                    var view = new ConsignmentsView
                    {
                        Owner = wnd,
                        DataContext = context
                    };
                    var result = view.ShowDialog();
                    if (result.HasValue && result.Value)
                        AddSingleOrder(context.SelectedConsignment);
                }

            ));
        }

        private RelayCommand<Window> _addPieceCons;
        public RelayCommand<Window> AddPieceCons
        {
            get => _addPieceCons ?? (_addPieceCons = new RelayCommand<Window>(
                wnd =>
                {
                    var context = new ConsignmentsViewModel(_db, ConsignmentSelectionType.Piece);
                    var view = new ConsignmentsView
                    {
                        Owner = wnd,
                        DataContext = context
                    };
                    var result = view.ShowDialog();
                    if (result.HasValue && result.Value)
                        AddPieceOrder(context.SelectedConsignment);
                }

            ));
        }

        private void AddSingleOrder(Consignment cons)
        {
            SingleOrders.Add(new SingleOrder
            {
                Consignment = cons,
                Invoice = Invoice,
                Count = 1,
                Discount = 0
            });
        }

        private void AddPieceOrder(Consignment cons)
        {
            PieceOrders.Add(new PieceOrder
            {
                Consignment = cons,
                Invoice = Invoice,
                Count = 1,
                Discount = 0
            });
        }

        public ComposeInvoiceViewModel(ProjectContext db)
        {
            InvoiceDate = DateTime.Now;
            _db = db;
            Invoice = new Invoice();
            SingleOrders = new ObservableCollection<SingleOrder>();
            PieceOrders = new ObservableCollection<PieceOrder>();

            SingleOrders.Add(new SingleOrder { Consignment = _db.Consignments.FirstOrDefault(), Count = 10, Discount = 0 });
        }
    }
}
