﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
            set => Set(ref _invoices, value);
        }

        private DateTime _singleDate = DateTime.Now;
        public DateTime SingleDate
        {
            get { return _singleDate; }
            set
            {
                if (value <= DateTime.Now)
                    Set(ref _singleDate, value);
                else
                    Set(ref _singleDate, DateTime.Now);
            }
        }

        private bool _isSingleDate = true;
        public bool IsSingleDate
        {
            get { return _isSingleDate; }
            set => Set(ref _isSingleDate, value);
        }


        private DateTime _dateFrom;
        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set
            {
                if (value < DateTo)
                    Set(ref _dateFrom, value);
            }
        }

        private DateTime _dateTo;
        public DateTime DateTo
        {
            get { return _dateTo; }
            set
            {
                if (value <= DateTime.Now && value > DateFrom)
                    Set(ref _dateTo, value);
                else
                    Set(ref _dateTo, DateTime.Now);
            }
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

            TotalSoldProductsAmount = invoices.Sum(i => i.AmountToPay).ToString("#,###0.00") + " AZN";
            TotalIncome = (totalInvoicesSold - singleProductsSellingPrice - pieceProductsSellingPrice).ToString("#,###0.00") + " AZN";
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get => _searchCommand ?? (_searchCommand = new RelayCommand(() => invoicesSearch()));
        }

        private void invoicesSearch()
        {
            if (IsSingleDate)
            {
                Invoices.Filter = i =>
                {
                    var invoice = i as Invoice;
                    return invoice.Date.Date == SingleDate.Date;
                };
            }
            else
            {
                Invoices.Filter = i =>
                {
                    var invoice = i as Invoice;
                    return invoice.Date.Date >= DateFrom.Date && invoice.Date.Date <= DateTo.Date;
                };
            }
        }

        public FullReportViewModel(ProjectContext db)
        {
            _db = db;
            Invoices  = new ListCollectionView( _db.Invoices.Local.ToObservableCollection());

            DateTo = DateTime.Now;
            DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            Calculate();
        }

    }
}
