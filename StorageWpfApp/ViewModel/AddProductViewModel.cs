using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class AddProductViewModel : ViewModelBase
    {
        private ProjectContext _db;

        public Product Product { get; set; }

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

        public AddProductViewModel(ProjectContext context)
        {
            _db = context;

            Groups = _db.Groups.Local.ToObservableCollection();
        }

        private bool CheckAll()
        {
            if (IsPieceAllowed)
                return !string.IsNullOrWhiteSpace(PPieceCount) && !string.IsNullOrWhiteSpace(Pcode);
            return !string.IsNullOrWhiteSpace(Pcode);
        }

        private string _pName = "";
        public string PName
        {
            get => _pName;
            set => Set(ref _pName, value);
        }

        private string _pPieceCount = "";
        public string PPieceCount
        {
            get => _pPieceCount;
            set
            {
                if (value.IsCorrectInt())
                    Set(ref _pPieceCount, value);
            }
        }


        private bool _isPieceAllowed;
        public bool IsPieceAllowed
        {
            get { return _isPieceAllowed; }
            set { _isPieceAllowed = value; }
        }


        private string _pCode = "";
        public string Pcode
        {
            get => _pCode;
            set => Set(ref _pCode, value);
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
            Product = new Product();
            Product.Code = Pcode;
            Product.Name = PName;
            Product.Group = SelectedGroup;
            Product.IsPieceProduct = false;

            if (IsPieceAllowed)
            {
                Product.IsPieceProduct = true;
                Product.PieceQuantity = PPieceCount.StringToInteger();
            }
        }

        private RelayCommand<Window> _saveChanges;
        public RelayCommand<Window> SaveChanges
        {
            get => _saveChanges ?? (_saveChanges = new RelayCommand<Window>(
                window =>
                {
                    if (_db.Products.Any(p => p.Code.Equals(Pcode, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"Товар с кодом \"{Pcode}\" уже существует!");
                        return;
                    }
                    SaveChangesToProduct();
                    _db.Products.Add(Product);
                    _db.SaveChanges();
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
