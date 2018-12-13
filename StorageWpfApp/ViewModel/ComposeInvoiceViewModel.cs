using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        private ObservableCollection<SingleOrder> _singleOrders;
        public ObservableCollection<SingleOrder> SingleOrders
        {
            get => _singleOrders;
            set => Set(ref _singleOrders, value);
        }

        public Invoice Invoice { get; set; }

        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get => _invoiceDate;
            set => Set(ref _invoiceDate, value);
        }

        private double _totalPriceWithDiscount;
        public double TotalPriceWithDiscount
        {
            get => _totalPriceWithDiscount;
            set => Set(ref _totalPriceWithDiscount, value);
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
                        if (SingleOrders.FirstOrDefault(x => x.Consignment == context.SelectedConsignment) == null)
                            AddSingleOrder(context.SelectedConsignment);
                        else
                            MessageBox.Show("Товар уже добавлен в список!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        if (PieceOrders.FirstOrDefault(x => x.Consignment == context.SelectedConsignment) == null)
                            AddPieceOrder(context.SelectedConsignment);
                        else
                            MessageBox.Show("Товар уже добавлен в список!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            ));
        }

        private RelayCommand<SingleOrder> _removeFromSingle;
        public RelayCommand<SingleOrder>  RemoveFromSingle
        {
            get => _removeFromSingle ?? (_removeFromSingle = new RelayCommand<SingleOrder>(
                order =>
                {
                    if (order != null)
                        SingleOrders.Remove(order);
                }

            ));
        }

        private RelayCommand<PieceOrder> _removeFromPiece;
        public RelayCommand<PieceOrder> RemoveFromPiece
        {
            get => _removeFromPiece ?? (_removeFromPiece = new RelayCommand<PieceOrder>(
                order =>
                {
                    if (order != null)
                        PieceOrders.Remove(order);
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

        private void SubscribeToSumPropertyChangedSingle(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateTotalSum();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newItems = e.NewItems.Cast<SingleOrder>();
                if (newItems != null)
                {
                    foreach (var item in newItems)
                    {
                        item.PropertyChanged += CalculateTotalSumEventHandler;
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var deletedItems = e.OldItems.Cast<SingleOrder>();
                if (deletedItems != null)
                {
                    foreach (var item in deletedItems)
                    {
                        item.PropertyChanged -= CalculateTotalSumEventHandler;
                    }
                }
            }
        }

        private void SubscribeToSumPropertyChangedPiece(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateTotalSum();
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newItems = e.NewItems.Cast<PieceOrder>();
                if (newItems != null)
                {
                    foreach (var item in newItems)
                    {
                        item.PropertyChanged += CalculateTotalSumEventHandler;
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var deletedItems = e.OldItems.Cast<PieceOrder>();
                if (deletedItems != null)
                {
                    foreach (var item in deletedItems)
                    {
                        item.PropertyChanged -= CalculateTotalSumEventHandler;
                    }
                }
            }
        }

        private void CalculateTotalSumEventHandler(object sender, PropertyChangedEventArgs e)
        {
            CalculateTotalSum();
        }

        private void CalculateTotalSum()
        {
            TotalPriceWithDiscount = SingleOrders.Sum(x => x.Sum) + PieceOrders.Sum(x => x.Sum);
        }

        public ComposeInvoiceViewModel(ProjectContext db)
        {
            InvoiceDate = DateTime.Now;
            _db = db;
            Invoice = new Invoice();
            SingleOrders = new ObservableCollection<SingleOrder>();
            PieceOrders = new ObservableCollection<PieceOrder>();

            SingleOrders.CollectionChanged += SubscribeToSumPropertyChangedSingle;
            PieceOrders.CollectionChanged += SubscribeToSumPropertyChangedPiece;
        }
    }
}
