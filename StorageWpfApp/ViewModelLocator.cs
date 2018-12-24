using GalaSoft.MvvmLight;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.Navigation;
using StorageWpfApp.ViewModel;
using System;
using System.Collections.Generic;
using StorageWpfApp.ExtensionMethods.RepoExtentions;
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
            //Todo Remove after release
            FillDbWithFakeData();
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

        private void FillDbWithFakeData()
        {
            if (!_db.Groups.Any())
            {
                _db.Groups.AddRange(
                        new ProductGroup { Name = "Гвозди" },
                        new ProductGroup { Name = "Краны" },
                        new ProductGroup { Name = "Споты" }
                        );


                var rnd = new Random();
                for (int i = 0; i < 10000; i++)
                {
                    var IsPiece = rnd.Next(2) == 0 ? false : true;
                    var prd = new Product { Code = Guid.NewGuid().ToString().Substring(0, 10), IsPieceProduct = IsPiece, Name = Guid.NewGuid().ToString().Substring(0, 8), GroupId = rnd.Next(1, 4) };
                    if (IsPiece)
                    {
                        prd.PieceQuantity = rnd.Next(10, 150);
                    }
                    _db.Products.Add(prd);
                }

                _db.SaveChanges();

                for (int i = 0; i < 10000; i++)
                {
                    var index = rnd.Next(1, 150);
                    var prd = _db.Products.FirstOrDefault(p => p.Id == index);
                    var purchPrice = rnd.Next(10, 150);
                    var cons = new Consignment { Date = new DateTime(rnd.Next(2015, 2019), rnd.Next(1, 13), rnd.Next(1, 28)), IsPieceAllowed = prd.IsPieceProduct, ProductId = index, PurchasePrice = purchPrice, Quantity = i, SellingPrice = purchPrice + purchPrice * 0.20 };
                    if (prd.IsPieceProduct)
                    {
                        cons.CurrentPieceQuantity = prd.PieceQuantity.Value * i;
                        var price = cons.SellingPrice / prd.PieceQuantity;
                        cons.PiecePrice = price;
                    }
                    _db.Consignments.Add(cons);
                    _db.AddCodeToConsignment(cons);
                }

                _db.Clients.AddRange(
                    new Client { Name = "Ujal", Surname = "Zeynalov", Email = "zeynalov.u@gmail.com", PhoneNumber = "+994505052813" },
                    new Client { Name = "Zahid", Surname = "Abbasli", Email = "abbasli.zahid@gmail.com", PhoneNumber = "+994558542512" },
                    new Client { Name = "Samir", Surname = "Mammadli", Email = "mammadli.s.r@gmail.com", PhoneNumber = "+994557099110" });

                _db.SaveChanges();
            }
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
