using GalaSoft.MvvmLight;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.Navigation;
using StorageWpfApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageWpfApp
{
    class ViewModelLocator : IDisposable
    {
        public AppViewModel appViewModel;
        public MainViewModel mainViewModel;
        public EditProductViewModel editProductViewModel;
        private ProjectContext _db;
        static private NavigationService navigationService = new NavigationService();

        public ViewModelLocator()
        {
            _db = new ProjectContext();
            LoadAllTables();
            appViewModel = new AppViewModel(_db);
            mainViewModel = new MainViewModel(navigationService, _db);
            editProductViewModel = new EditProductViewModel(_db);

            navigationService.AddPage(mainViewModel, VM.Add);
            navigationService.AddPage(editProductViewModel, VM.EditProduct);

            navigationService.NavigateTo(VM.Add);
        }

        private void LoadAllTables()
        {
            _db.Groups.Load();
            _db.Products.Load();
            _db.Consignments.Load();
            _db.Clients.Load();
            _db.SingleOrders.Load();
            _db.PieceOrders.Load();
            _db.Debts.Load();
            _db.Invoices.Load();
            _db.DebtPayments.Load();
        }

        public static ViewModelBase GetCurrent()
        {
            return navigationService.Current;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
