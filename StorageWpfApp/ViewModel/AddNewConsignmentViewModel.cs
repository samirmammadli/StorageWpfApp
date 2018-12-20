using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.ExtensionMethods.RepoExtentions;
using StorageWpfApp.Messages;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class AddNewConsignmentViewModel : ViewModelBase
    {
        private ProjectContext _db;

        public AddNewConsignmentViewModel(ProjectContext context)
        {
            _db = context;
            ConsDate = DateTime.Now;
        }

        public AddNewConsignmentViewModel(ProjectContext context, Product prd)
        {
            _db = context;
            ConsDate = DateTime.Now;
            SelectedProduct = prd;
        }

        private bool _isEnable;
        public bool IsEnable
        {
            get { return _isEnable; }
            set => Set(ref _isEnable, value);
        }

        private string _piecePriceCalculated;
        public string PiecePriceCalculated
        {
            get { return _piecePriceCalculated; }
            set => Set(ref _piecePriceCalculated, value);
        }



        private DateTime _consDate;
        public DateTime ConsDate
        {
            get { return _consDate; }
            set { Set(ref _consDate, value); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                Set(ref _selectedProduct, value);
                if (value != null)
                    IsEnable = true;
                else
                    IsEnable = false;

            }
        }

        private Consignment _selectedConsignment;
        public Consignment SelectedConsignment
        {
            get { return _selectedConsignment; }
            set { Set(ref _selectedConsignment, value); }
        }

        private string _totalPieceCount = "";
        public string TotalPieceCount
        {
            get => _totalPieceCount;
            set => Set(ref _totalPieceCount, value);
        }

        public AddNewConsignmentViewModel(ProjectContext context, Consignment cons)
        {
            SelectedConsignment = cons;
            _db = context;

            if (SelectedConsignment != null)
            {
                if (cons.Date != null)
                {
                    ConsDate = (DateTime)cons.Date;
                }
                
                PBuyPrice = cons.PurchasePrice.ToString();
                PPieceSellPrice = cons.PiecePrice.ToString();
                PSellPrice = cons.SellingPrice.ToString();
                PCount = cons.Quantity.ToString();
                SelectedProduct = cons.Product;
            }
        }

        private void SetTotalPieceCount()
        {
            if (PCount == "")
            {
                TotalPieceCount = "";
                return;
            }

            if (PCount.IsCorrectInt() && SelectedProduct != null && SelectedProduct.IsPieceProduct)
            {
                int.TryParse(PCount, out int val1);

                TotalPieceCount = (val1 * SelectedProduct.PieceQuantity).ToString();
            }
                
        }

        private string _pCount = "";
        public string PCount
        {
            get => _pCount;
            set
            {
                if (value.IsCorrectInt())
                {
                    Set(ref _pCount, value);
                    SetTotalPieceCount();
                }
            }
        }

        private string _pBuyPrice = "";
        public string PBuyPrice
        {
            get => _pBuyPrice;
            set
            {
                if (value.IsDouble())
                {
                    var numb = value.StringToDouble();

                    if (SelectedProduct != null && SelectedProduct.IsPieceProduct)
                    {
                        PiecePriceCalculated = (numb / SelectedProduct.PieceQuantity.Value).ToString("#0.00");
                    }
                    Set(ref _pBuyPrice, value);
                }
            }
        }


        private string _pSellPrice = "";
        public string PSellPrice
        {
            get => _pSellPrice;
            set
            {
                if (value.IsDouble())
                    Set(ref _pSellPrice, value);
            }
        }

        private string _pPieceSellPrice = "";
        public string PPieceSellPrice
        {
            get => _pPieceSellPrice;
            set
            {
                if (value.IsDouble())
                    Set(ref _pPieceSellPrice, value);
            }
        }

        private bool CheckAll()
        {
            if (SelectedProduct == null)
                return false;

            if (SelectedProduct.IsPieceProduct)
            {
                if (string.IsNullOrWhiteSpace(PPieceSellPrice) || PPieceSellPrice.StringToDouble() < PiecePriceCalculated.StringToDouble())
                {
                    //MessageBox.Show((PPieceSellPrice.StringToDouble() < PiecePriceCalculated.StringToDouble()).ToString());
                    return false;
                }
            }


            return !(string.IsNullOrWhiteSpace(PCount) ||
                string.IsNullOrWhiteSpace(PBuyPrice) ||
                string.IsNullOrWhiteSpace(PSellPrice));
        }

        private RelayCommand<TextBox> _clearSearchBox;
        public RelayCommand<TextBox> ClearSearchBox
        {
            get => _clearSearchBox ?? (_clearSearchBox = new RelayCommand<TextBox>(
                prm =>
                {
                    if (string.IsNullOrWhiteSpace(prm.Text))
                    {
                        prm.Text = "";
                        return;
                    }
                    prm.Text = "";
                }
            ));
        }

        private RelayCommand<Window> _addProduct;
        public RelayCommand<Window> AddProduct
        {
            get => _addProduct ?? (_addProduct = new RelayCommand<Window>(
                mainWnd =>
                {
                    var cont = new AddProductViewModel(_db);
                    var addWnd = new AddProductView
                    {
                        DataContext = cont
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                    Messenger.Default.Send(new UpdateProducts());

                    if (cont.Product != null)
                    {
                        SelectedProduct = cont.Product;
                    }
                }
            ));
        }

        private RelayCommand<Window> _selectProduct;
        public RelayCommand<Window> SelectProduct
        {
            get => _selectProduct ?? (_selectProduct = new RelayCommand<Window>(
                mainWnd =>
                {
                    var cont = new MainViewModel(null, _db, true);
                    var addWnd = new Window
                    {
                        Content = new MainView(),
                        DataContext = cont
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                    SelectedProduct = cont.SelectedProduct;
                    Messenger.Default.Send(new UpdateProducts());
                }
            ));
        }

        private void SaveChangesToProduct()
        {

            var consignments = _db.Consignments.Where(c => c.Product == SelectedProduct);

            foreach (var cons in consignments)
            {
                if (cons.SellingPrice == PSellPrice.StringToDouble() &&
                    cons.PurchasePrice == PBuyPrice.StringToDouble())
                {
                    if ((SelectedProduct.IsPieceProduct && cons.PiecePrice == PPieceSellPrice.StringToDouble()) || !SelectedProduct.IsPieceProduct)
                    {
                        ChangeIfConsignmentIsSame(cons);
                        return;
                    }
                }
            }

            SelectedConsignment = new Consignment
            {
                Date = ConsDate,
                Product = SelectedProduct,
                Quantity = PCount.StringToInteger(),
                PurchasePrice = PBuyPrice.StringToDouble(),
                SellingPrice = PSellPrice.StringToDouble(),
                IsPieceAllowed = SelectedProduct.IsPieceProduct
            };

            if (SelectedProduct.IsPieceProduct)
            {
                SelectedConsignment.PiecePrice = PPieceSellPrice.StringToDouble();
                SelectedConsignment.CurrentPieceQuantity = (int)SelectedProduct.PieceQuantity * SelectedConsignment.Quantity;
            }

            _db.AddCodeToConsignment(SelectedConsignment);
            _db.Consignments.Add(SelectedConsignment);
        }

        private void ChangeIfConsignmentIsSame(Consignment cons)
        {
            cons.Date = ConsDate;
            cons.Quantity += PCount.StringToInteger();

            if (SelectedProduct.IsPieceProduct)
            {
                cons.CurrentPieceQuantity += (int)SelectedProduct.PieceQuantity * PCount.StringToInteger();
            }
        }

        private RelayCommand<Window> _saveChanges;
        public RelayCommand<Window> SaveChanges
        {
            get => _saveChanges ?? (_saveChanges = new RelayCommand<Window>(
                window =>
                {
                    SaveChangesToProduct();
                    Messenger.Default.Send<UpdateProducts>(null);
                    _db.SaveChanges();
                    MessageBox.Show(PiecePriceCalculated);
                    window.Close();
                },
                window => CheckAll()
            ));
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
    }
}
