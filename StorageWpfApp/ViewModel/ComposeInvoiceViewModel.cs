﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.EntityFrameworkCore;
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

        private string _errorMessage;

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

        private Client _client;
        public Client Client
        {
            get => _client;
            set
            {
                Set(ref _client, value);
                ClientsFullName = $"{_client?.Name} {_client?.Surname}";
            }
        }

        private string _clientsFullName;
        public string ClientsFullName
        {
            get { return _clientsFullName; }
            set => Set(ref _clientsFullName, value);
        }


        private double totalSumWithDiscountTemp;

        private double _totalPriceWithDiscount;
        public double TotalPriceWithDiscount
        {
            get => Math.Round(_totalPriceWithDiscount,2);
            set => Set(ref _totalPriceWithDiscount, value);
        }

        private double _totalPriceWithoutDiscount;
        public double TotalPriceWithoutDiscount
        {
            get => Math.Round(_totalPriceWithoutDiscount, 2);
            set => Set(ref _totalPriceWithoutDiscount, value);
        }

        private double _totalSumToPay;
        public double TotalSumToPay
        {
            get => Math.Round(_totalSumToPay, 2);
            set => Set(ref _totalSumToPay, value);
        }

        private string _additionalTotalDiscount;
        public string AdditionalTotalDiscount
        {
            get => _additionalTotalDiscount;
            set
            {
                if (value.IsDouble() && value.StringToDouble() < totalSumWithDiscountTemp)
                {
                    Set(ref _additionalTotalDiscount, value);
                    CalculateTotalSum();
                }
            }
        }

        private string _debtAmount;
        public string DebtAmount
        {
            get => _debtAmount;
            set
            {
                if (value.IsDouble() && value.StringToDouble() <= totalSumWithDiscountTemp)
                {

                    Set(ref _debtAmount, value);

                    CalculateTotalSum();
                }
            }
        }

        private RelayCommand<Window> _closeWindow;
        public RelayCommand<Window> CloseWindow
        {
            get => _closeWindow ?? (_closeWindow = new RelayCommand<Window>(
                window =>
                {
                    window.Close();
                }
            ));
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

        private RelayCommand<Window> _addClient;
        public RelayCommand<Window> AddClient
        {
            get => _addClient ?? (_addClient = new RelayCommand<Window>(
                wnd =>
                {
                    var context = new SelectClientViewModel(_db, true);
                    var view = new SelectClientView
                    {
                        Owner = wnd,
                        DataContext = context
                    };
                    var result = view.ShowDialog();
                    if (result.HasValue && result.Value)
                        Client = context.SelectedClient;
                }

            ));
        }

        private bool AddInvoiceToDatabase()
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    SubstractProductsFromConsignments();

                    Invoice.Date = InvoiceDate;
                    Invoice.TotalPayed = TotalSumToPay;
                    Invoice.TotalDiscount = TotalPriceWithoutDiscount - TotalPriceWithDiscount;
                    Invoice.TotalAmount = TotalPriceWithoutDiscount;

                    _db.Invoices.Add(Invoice);

                    _db.SaveChanges();

                    if (PieceOrders.Count > 0)
                        _db.PieceOrders.AddRange(PieceOrders);
                    if (SingleOrders.Count > 0)
                        _db.SingleOrders.AddRange(SingleOrders);

                    if (Client != null)
                    {
                        Invoice.Client = Client;
                        if (DebtAmount.StringToDouble() > 0)
                        {
                            var clientDebt = new Debt
                            {
                                Client = Client,
                                Invoice = Invoice,
                                Amount = DebtAmount.StringToDouble(),
                            };

                            _db.Debts.Add(clientDebt);
                            _db.SaveChanges();

                            Invoice.Debt = clientDebt;
                        }
                    }
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    var cons = SingleOrders.Select(x => x.Consignment).Union(PieceOrders.Select(x => x.Consignment)).Distinct();

                    foreach (var item in cons)
                    {
                        _db.Entry<Consignment>(item).Reload();
                    }

                    return false;
                }

                transaction.Commit();
            }

            return true;
        }

        private void SubstractProductsFromConsignments()
        {
            foreach (var item in SingleOrders)
            {
                item.Consignment.Quantity -= item.Count;

                if (item.Consignment.IsPieceAllowed)
                {
                    item.Consignment.CurrentPieceQuantity -= item.Count * item.Consignment.Product.PieceQuantity.Value;
                }
            }


            foreach (var item in PieceOrders)
            {
                var balance = item.Consignment.CurrentPieceQuantity % item.Consignment.Product.PieceQuantity.Value;

                var balanceFromCount = balance - item.Count;

                if (balanceFromCount < 0)
                {
                    var singleProductsToSubstract = Math.Abs(balanceFromCount) / item.Consignment.Product.PieceQuantity.Value;

                    if (Math.Abs(balanceFromCount) % item.Consignment.Product.PieceQuantity.Value == 0)
                        item.Consignment.Quantity -= singleProductsToSubstract;
                    else
                        item.Consignment.Quantity -= (singleProductsToSubstract + 1);

                    if (item.Consignment.Quantity < 0)
                    {
                        _errorMessage = $"Введены неверные значения количества\nв товаре под кодом: {item.Consignment.Product.Code} с кодом партии: {item.Consignment.Code}";
                        throw new ArgumentException("Products count is not correct");
                    }
                }

                item.Consignment.CurrentPieceQuantity -= item.Count;
            }
        }

        private bool CheckAll()
        {
            return InvoiceDate != null && (SingleOrders.Any() || PieceOrders.Any());
        }

        /// <summary>
        /// 
        /// </summary>
        private RelayCommand<Window> _saveInvoiceCommand;
        public RelayCommand<Window> SaveInvoiceCommand
        {
            get => _saveInvoiceCommand ?? (_saveInvoiceCommand = new RelayCommand<Window>(
                wnd =>
                {
                    if (AddInvoiceToDatabase())
                    {
                        MessageBox.Show($"Номер накладной: {Invoice.Id}.", "Накладная создана!", MessageBoxButton.OK, MessageBoxImage.Information);
                        wnd.Close();
                    }
                    else
                    {
                        var msg = "Что то пошло не так!";
                        if (_errorMessage != null)
                            msg = _errorMessage;
                        MessageBox.Show(msg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        _errorMessage = null;
                    }
                },
                wnd => CheckAll()
            ));
        }

        private RelayCommand<SingleOrder> _removeFromSingle;
        public RelayCommand<SingleOrder> RemoveFromSingle
        {
            get => _removeFromSingle ?? (_removeFromSingle = new RelayCommand<SingleOrder>(
                order =>
                {
                    if (order != null)
                        SingleOrders.Remove(order);
                }

            ));
        }

        private RelayCommand _clearClient;
        public RelayCommand ClearClient
        {
            get => _clearClient ?? (_clearClient = new RelayCommand(
                () =>
                {
                    Client = null;
                    DebtAmount = "";
                },
                () => Client != null
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
            if (cons.Quantity <= 0)
            {
                MessageBox.Show("Нельзя добавить товар с нулевым количеством!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
            if (cons.CurrentPieceQuantity <= 0)
            {
                MessageBox.Show("Нельзя добавить товар с нулевым количеством!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

            CalculateTotalSum();
        }

        private void SubscribeToSumPropertyChangedPiece(object sender, NotifyCollectionChangedEventArgs e)
        {

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

            CalculateTotalSum();
        }

        private void CalculateTotalSumEventHandler(object sender, PropertyChangedEventArgs e)
        {
            CalculateTotalSum();
        }

        private void CalculateTotalSum()
        {
            totalSumWithDiscountTemp = Math.Round(SingleOrders.Sum(x => x.Sum) + PieceOrders.Sum(x => x.Sum), 2);

            if (AdditionalTotalDiscount.StringToDouble() > totalSumWithDiscountTemp)
            {
                _additionalTotalDiscount = "0";
                RaisePropertyChanged(nameof(AdditionalTotalDiscount));
            }

            TotalPriceWithDiscount = totalSumWithDiscountTemp - AdditionalTotalDiscount.StringToDouble();

            TotalPriceWithoutDiscount = TotalPriceWithDiscount + SingleOrders.Sum(x => x.Discount * x.Count) + PieceOrders.Sum(x => x.Discount * x.Count) + AdditionalTotalDiscount.StringToDouble();

            if (DebtAmount.StringToDouble() > TotalPriceWithDiscount)
            {
                _debtAmount = TotalPriceWithDiscount.ToString();
                RaisePropertyChanged(nameof(DebtAmount));
            }

            TotalSumToPay = TotalPriceWithDiscount - DebtAmount.StringToDouble();
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
