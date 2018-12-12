using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.ListCollectionSorting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;




namespace StorageWpfApp.ViewModel
{
    class ConsignmentsViewModel : ViewModelBase
    {
        private ProjectContext _db;


        public ConsignmentSelectionType SelectionType { get; set; }


        private bool _showEmptyConsignments;
        public bool ShowEmptyConsignments
        {
            get { return _showEmptyConsignments; }
            set => Set(ref _showEmptyConsignments, value);
        }

        private ListCollectionView _consignments;
        public ListCollectionView Consignments
        {
            get => _consignments;
            set => Set(ref _consignments, value);
        }

        private Consignment _selectedConsignment;
        public Consignment SelectedConsignment
        {
            get => _selectedConsignment;
            set => Set(ref _selectedConsignment, value);
        }

        public ConsignmentsViewModel(ProjectContext db)
        {
            _db = db;

            Groups = _db.Groups.Local.ToObservableCollection();
            Consignments = new ListCollectionView(_db.Consignments.Local.ToObservableCollection());
            Consignments.CustomSort = new ConsignmentDateSorting();
            ProductsPricingCalculating();
        }

        public ConsignmentsViewModel(ProjectContext db, ConsignmentSelectionType type)
        {
            _db = db;
            SelectionType = type;
            Groups = _db.Groups.Local.ToObservableCollection();
            Consignments = new ListCollectionView(_db.Consignments.Local.ToObservableCollection());
            Consignments.CustomSort = new ConsignmentDateSorting();
            ProductsPricingCalculating();
            SearchBy();
        }

        public ConsignmentsViewModel(ProjectContext db, Product prd)
        {
            _db = db;
            Pcode = prd.Code;
            Groups = _db.Groups.Local.ToObservableCollection();
            SelectedGroup = new ProductGroup { Id = -1 };
            Consignments = new ListCollectionView(_db.Consignments.Local.ToObservableCollection());
            Consignments.CustomSort = new ConsignmentDateSorting();
            SearchBy();
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

        private bool _exactCode = true;
        public bool ExactCode
        {
            get { return _exactCode; }
            set => Set(ref _exactCode, value);
        }

        private string _pCode = "";
        public string Pcode
        {
            get => _pCode;
            set => Set(ref _pCode, value);
        }

        private string _pName = "";
        public string PName
        {
            get => _pName;
            set => Set(ref _pName, value);
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

        private RelayCommand _search;
        public RelayCommand Search
        {
            get => _search ?? (_search = new RelayCommand(
                SearchBy
            ));
        }

        private string _totalProductsCount;
        public string TotalProductsCount
        {
            get { return _totalProductsCount; }
            set => Set(ref _totalProductsCount, value);
        }

        private string _totalPurchaseSum;
        public string TotalPurchaseSum
        {
            get { return _totalPurchaseSum; }
            set => Set(ref _totalPurchaseSum, value);
        }

        private string _totalSellingSum;
        public string TotalSellingSum
        {
            get { return _totalSellingSum; }
            set => Set(ref _totalSellingSum, value);
        }


        private void SearchBy()
        {

            Consignments.Filter = c =>
            {
                var cons = c as Consignment;
                return CodePatternSearch(cons.Product.Code) &&
                                                        FilterBySelectionType(cons) &&
                                                        cons.Product.Name.Contains(PName) &&
                                                        GroupSearch(cons.Product.Group);
            };

            ProductsPricingCalculating();
        }

        //Todo: Show Empty Consignments
        private bool SearchEmptyConsignments(Consignment cons)
        {
            if (ShowEmptyConsignments)
                return true;
            else
                return (cons.CurrentPieceQuantity > 0 && cons.Quantity > 0);
        }

        private bool FilterBySelectionType(Consignment cons)
        {
            if (SelectionType == ConsignmentSelectionType.Piece && !cons.IsPieceAllowed)
                return false;

            return true;
        }

        private void ProductsPricingCalculating()
        {
            var items = Consignments.Cast<Consignment>().ToList();

            TotalProductsCount = items.Sum(x => x.Quantity).ToString("#,###") + " шт.";
            TotalPurchaseSum = items.Sum(x => x.PurchasePrice * x.Quantity).ToString("#,###0.00") + " AZN";
            TotalSellingSum = items.Sum(x => x.SellingPrice * x.Quantity).ToString("#,###0.00") + " AZN";
        }

        private bool CodePatternSearch(string val)
        {
            if (ExactCode && !string.IsNullOrEmpty(Pcode))
                return Pcode.Equals(val, System.StringComparison.OrdinalIgnoreCase);

            return val.Contains(Pcode);
        }


        private bool GroupSearch(ProductGroup grp)
        {
            if (SelectedGroup == null)
                return false;
            if (grp == null)
                return true;

            return grp.Id == SelectedGroup.Id || SelectedGroup?.Id == -1;
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
                    SearchBy();
                }
            ));
        }
    }
}
