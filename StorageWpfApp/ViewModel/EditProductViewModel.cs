using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.Messages;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class EditProductViewModel : ViewModelBase
    {
        private ProjectContext _db;

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (_selectedProduct != null && _selectedProduct.IsPieceProduct)
                    IsPieceVisible = Visibility.Visible;
                else
                    IsPieceVisible = Visibility.Collapsed;
            }
        }

        public EditProductViewModel(ProjectContext context)
        {
            _db = context;
        }

        private ObservableCollection<ProductGroup> _groups;
        public ObservableCollection<ProductGroup> Groups
        {
            get { return _groups; }
            set { Set(ref _groups, value); }
        }

        private ProductGroup _selectedGroup;
        public ProductGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set { Set(ref _selectedGroup, value); }
        }

        public EditProductViewModel(ProjectContext context, Product prd)
        {
            SelectedProduct = prd;
            _db = context;

            Groups = _db.Groups.Local.ToObservableCollection();
            if(prd != null)
            {
                Pcode = prd.Code;
                PName = prd.Name;
                SelectedGroup = prd.Group;

                if (SelectedProduct != null && SelectedProduct.IsPieceProduct)
                    PPieceQuantity = SelectedProduct.PieceQuantity.ToString();
            }
        }

        private bool CheckAll()
        {
            if (SelectedProduct != null && SelectedProduct.IsPieceProduct && string.IsNullOrWhiteSpace(PPieceQuantity))
                return false;

            return !string.IsNullOrWhiteSpace(Pcode);
        }

        private string _pName = "";
        public string PName
        {
            get => _pName;
            set => Set(ref _pName, value);
        }

        private string _pCode = "";
        public string Pcode
        {
            get => _pCode;
            set => Set(ref _pCode, value);
        }

        private string _pPieceQuantity = "";
        public string PPieceQuantity
        {
            get => _pPieceQuantity;
            set
            {
                if (value.IsCorrectInt())
                    Set(ref _pPieceQuantity, value);
            }
        }

        private Visibility _isPieceVisible;
        public Visibility IsPieceVisible
        {
            get { return _isPieceVisible; }
            set => Set(ref _isPieceVisible, value);
        }

        //private string _pBuyPrice = "";
        //public string PBuyPrice
        //{
        //    get => _pBuyPrice;
        //    set
        //    {
        //        if (value.IsDouble())
        //            Set(ref _pBuyPrice, value);
        //    }
        //}


        //private string _pSellPrice = "";
        //public string PSellPrice
        //{
        //    get => _pSellPrice;
        //    set
        //    {
        //        if (value.IsDouble())
        //            Set(ref _pSellPrice, value);
        //    }
        //}

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

        private void SaveChangesToProduct()
        {
            try
            {
                //SelectedProduct.PurchasePrice = double.Parse(PBuyPrice, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                //try
                //{
                //    SelectedProduct.SellingPrice = double.Parse(PSellPrice, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                //}
                //catch (Exception)
                //{

                //    throw;
                //}

                //try
                //{
                //    SelectedProduct.Quantity = int.Parse(PCount);
                //}
                //catch (Exception)
                //{

                //    throw;
                //}
                
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные значения!");
            }

            SelectedProduct.Code = Pcode;
            SelectedProduct.Name = PName;
            SelectedProduct.Group = SelectedGroup;

            if (SelectedProduct.IsPieceProduct)
                SelectedProduct.PieceQuantity = PPieceQuantity.StringToInteger();
        }

        private RelayCommand<Window> _saveChanges;
        public RelayCommand<Window> SaveChanges
        {
            get => _saveChanges ?? (_saveChanges = new RelayCommand<Window>(
                window =>
                {
                    var prd = _db.Products.SingleOrDefault(p => p.Code == Pcode);
                    if (prd != null && prd != SelectedProduct)
                    {
                        MessageBox.Show($"Товар с кодом \"{Pcode}\" уже существует!");
                        return;
                    }

                    SaveChangesToProduct();
                    _db.SaveChanges();
                    Messenger.Default.Send<UpdateProducts>(null);
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
