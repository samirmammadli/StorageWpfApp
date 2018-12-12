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
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class AddProductWithConsignmentViewModel : ViewModelBase
    {
        private ProjectContext _db;

        public Product SelectedProduct { get; set; }

        private Consignment _selectedConsignment;
        public Consignment SelectedConsignment
        {
            get { return _selectedConsignment; }
            set { Set(ref _selectedConsignment, value); }
        }

        public AddProductWithConsignmentViewModel(ProjectContext context)
        {
            _db = context;
            Groups = _db.Groups.Local.ToObservableCollection();
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

        private DateTime _consDate = DateTime.Now;
        public DateTime ConsDate
        {
            get { return _consDate; }
            set { Set(ref _consDate, value); }
        }

        private bool _isConsignmentAddingSelected = false;
        public bool IsConsignmentAddingSelected
        {
            get => _isConsignmentAddingSelected;
            set => Set(ref _isConsignmentAddingSelected, value);
        }

        private bool CheckAll()
        {
            if (IsConsignmentAddingSelected)
            {
                if (IsPieceAllowed)
                {
                    if (string.IsNullOrWhiteSpace(PPieceCount) || string.IsNullOrWhiteSpace(PPieceSellPrice))
                        return false;
                }


                if (string.IsNullOrWhiteSpace(PCount) || 
                    string.IsNullOrWhiteSpace(PBuyPrice) || 
                    string.IsNullOrWhiteSpace(PSellPrice))
                    return false;
            }

            if (IsPieceAllowed)
                return !string.IsNullOrWhiteSpace(Pcode) && !string.IsNullOrWhiteSpace(PPieceCount);

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

        private string _totalPieceCount = "";
        public string TotalPieceCount
        {
            get => _totalPieceCount;
            set => Set(ref _totalPieceCount, value);
        }

        private void SetTotalPieceCount()
        {
            if (PCount == "" || PPieceCount == "")
            {
                TotalPieceCount = "";
                return;
            }

            if (PCount.IsCorrectInt() && PPieceCount.IsCorrectInt())
            {
                int.TryParse(PCount, out int val1);
                int.TryParse(PPieceCount, out int val2);

                TotalPieceCount = (val1 * val2).ToString();
            }

        }

        private string _pPieceCount = "";
        public string PPieceCount
        {
            get => _pPieceCount;
            set
            {
                if (value.IsCorrectInt())
                {
                    Set(ref _pPieceCount, value);
                    SetTotalPieceCount();
                }
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
                    Set(ref _pBuyPrice, value);
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

        private bool _isPieceAllowed = false;
        public bool IsPieceAllowed
        {
            get => _isPieceAllowed;
            set => Set(ref _isPieceAllowed, value);

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

        private void SaveChangesToProduct()
        {
            SelectedProduct = new Product
            {
                Code = Pcode,
                Name = PName,
                Group = SelectedGroup,
                IsPieceProduct = IsPieceAllowed
            };

            if (IsPieceAllowed)
                SelectedProduct.PieceQuantity = PPieceCount.StringToInteger();

            _db.Products.Add(SelectedProduct);
            _db.SaveChanges();

            if (IsConsignmentAddingSelected)
            {
                SelectedConsignment = new Consignment
                {
                    Date = ConsDate,
                    Product = SelectedProduct,
                    Quantity = PCount.StringToInteger(),
                    PurchasePrice = PBuyPrice.StringToDouble(),
                    SellingPrice = PSellPrice.StringToDouble(),
                    IsPieceAllowed = IsPieceAllowed
                };

                if (IsPieceAllowed)
                {
                    SelectedConsignment.PiecePrice = PPieceSellPrice.StringToDouble();
                    SelectedConsignment.CurrentPieceQuantity = (int)SelectedProduct.PieceQuantity * SelectedConsignment.Quantity;
                }

                _db.Consignments.Add(SelectedConsignment);
                _db.SaveChanges();
            }
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
