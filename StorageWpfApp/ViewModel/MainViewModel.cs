using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Converters;
using StorageWpfApp.Entities;
using StorageWpfApp.ExtensionMethods;
using StorageWpfApp.Messages;
using StorageWpfApp.Navigation;
using StorageWpfApp.View;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StorageWpfApp.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private ListCollectionView _coll;
        public ListCollectionView Collection
        {
            get { return _coll; }
            set { Set(ref _coll, value); }
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

        private bool _isSelectionMode;
        public bool IsSelectionMode
        {
            get { return _isSelectionMode; }
            set { _isSelectionMode = value; }
        }

        private Visibility _selectedVisibility;
        public Visibility SelectionVisibility
        {
            get => _selectedVisibility;
            set => Set(ref _selectedVisibility, value);
        }
        private Visibility _mainVisibility;
        public Visibility MainVisibility
        {
            get => _mainVisibility;
            set => Set(ref _mainVisibility, value);
        }

        private string _pCode = "";
        public string  Pcode
        {
            get => _pCode;
            set => Set(ref _pCode, value);
        }

        private int _minQuantityCount = ConvertersStaticData.MinValue;
        public int MinQuantityCount
        {
            get => _minQuantityCount;
            set { ConvertersStaticData.MinValue = value; Set(ref _minQuantityCount, value); }
        }

        

        private string _pName = "";
        public string PName
        {
            get => _pName;
            set => Set(ref _pName, value);
        }



        private Product _selectedProduct;

        public Product SelectedProduct
        {
            get { return _selectedProduct;}
            set { Set(ref _selectedProduct, value); }
        }

        private int? _selectedIndex;
        public int? SelectedIndex
        {
            get { return _selectedIndex; }
            set { Set(ref _selectedIndex, value); }
        }

        private ObservableCollection<Product> _prd;

        private void LoadProducts()
        {
            _prd = _db.Products.Local.ToObservableCollection();
            Groups = _db.Groups.Local.ToObservableCollection();
            //Products = _db.Products.Local.ToObservableCollection();

            //Products = _prd;
        }

        private bool _exactCode = true;
        public bool ExactCode
        {
            get { return _exactCode; }
            set => Set(ref _exactCode, value);
        }


        private void SearchBy()
        {

            Collection.Filter = p =>
            {
                var prd = p as Product;
                return (CodePatternSearch(prd.Code) && prd.Name.Contains(PName) &&
                                                        (prd.Group?.Id == SelectedGroup?.Id || SelectedGroup?.Id == -1));
            };

            //Collection.Refresh();

            //Products = new ObservableCollection<Product>(_prd.Where(x => CodePatternSearch(x.Code) &&

            //                                            x.Name.Contains(PName) &&
            //                                            (x.Group?.Id == SelectedGroup?.Id || SelectedGroup?.Id == -1)));

        }


        private bool CodePatternSearch(string val)
        {
            if (ExactCode && !string.IsNullOrEmpty(Pcode))
                return Pcode.Equals(val, System.StringComparison.OrdinalIgnoreCase);

            return val.Contains(Pcode);
        }

        private RelayCommand<object> _editProduct;
        public RelayCommand<object> EditProduct
        {

            get => _editProduct ?? (_editProduct = new RelayCommand<object>(
                mainWnd =>
                {
                    var wnd = new EditProduct
                    {
                        DataContext = new EditProductViewModel(_db, SelectedProduct)
                    };
                    wnd.Owner = Window.GetWindow(mainWnd as UserControl);
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    wnd.ShowDialog();
                },
                x => SelectedProduct != null
            ));
        }


        private RelayCommand<UserControl> _closeWindow;
        public RelayCommand<UserControl> CloseWindow
        {
            get => _closeWindow ?? (_closeWindow = new RelayCommand<UserControl>(
                window =>
                {
                    if (IsSelectionMode)
                    Window.GetWindow(window).Close();
                }
            ));
        }


        private RelayCommand<UserControl> _addConsignment;
        public RelayCommand<UserControl> AddConsignment
        {
            get => _addConsignment ?? (_addConsignment = new RelayCommand<UserControl>(
                wnd =>
                {
                    if (IsSelectionMode)
                    {
                        Window.GetWindow(wnd).Close();
                        return;
                    }

                    var vm = new AddNewConsignmentViewModel(_db, SelectedProduct);
                    var addCons = new AddNewConsignmentView
                    {
                        DataContext = vm,
                        Owner = Window.GetWindow(wnd),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    addCons.ShowDialog();
                },
                wnd => SelectedProduct != null
            ));
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


        private RelayCommand _save;
        public RelayCommand Save
        {
            get => _save ?? (_save = new RelayCommand(
                () =>  _db.SaveChanges()
            ));
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get => _loadCommand ?? (_loadCommand = new RelayCommand(
                () => LoadProducts()
            ));
        }

        private RelayCommand _search;
        public RelayCommand Search
        {
            get => _search ?? (_search = new RelayCommand(
                () => { SearchBy(); }
            ));
        }


        private RelayCommand<UserControl> _selectConsignment;
        public RelayCommand<UserControl> SelectConsignment
        {
            get => _selectConsignment ?? (_selectConsignment = new RelayCommand<UserControl>(
                mainWnd =>
                {
                    Window.GetWindow(mainWnd).Close();
                }, 
                mainWnd => SelectedProduct != null
            ));
        }

        private RelayCommand<UserControl> _deleteProduct;
        public RelayCommand<UserControl> DeleteProduct
        {
            get => _deleteProduct ?? (_deleteProduct = new RelayCommand<UserControl>(
                mainWnd =>
                {
                    var consignments = _db.Consignments.Where(x => x.Product == SelectedProduct);
                    if (consignments != null && consignments.Count() > 0)
                    {
                        foreach (var item in consignments)
                        {
                            if (_db.PieceOrders.Any(x => x.Consignment == item) || _db.SingleOrders.Any(x => x.Consignment == item))
                            {
                                MessageBox.Show("На партии данного товара имеются накладные!", "Нельзя удалить!", MessageBoxButton.OK, MessageBoxImage.Hand);
                                return;
                            }
                        }

                        var result = MessageBox.Show("На данный товар имеются партии\nВсе партии будут так же удалены!\nУдалить?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            _db.Consignments.RemoveRange(consignments);
                            _db.Products.Remove(SelectedProduct);
                            _db.SaveChanges();
                            MessageBox.Show("Товар успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            return;
                        }
                    }
                    else
                    {
                        _db.Products.Remove(SelectedProduct);
                        _db.SaveChanges();
                        MessageBox.Show("Товар успешно удален!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    } 
                },
                mainWnd => SelectedProduct != null
            ));
        }

        private RelayCommand<UserControl> _viewConsignments;
        public RelayCommand<UserControl> ViewConsignments
        {
            get => _viewConsignments ?? (_viewConsignments = new RelayCommand<UserControl>(
                userControl =>
                {
                    var consWnd = new ConsignmentsView();
                    consWnd.DataContext = new ConsignmentsViewModel(_db, SelectedProduct);

                    consWnd.Owner = Window.GetWindow(userControl);
                    consWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    consWnd.ShowDialog();
                },
                mainWnd => SelectedProduct != null
            ));
        }

        private RelayCommand<UserControl> _addProduct;
        public RelayCommand<UserControl> AddProduct
        {
            get => _addProduct ?? (_addProduct = new RelayCommand<UserControl>(
                mainWnd =>
                {
                    var addWnd = new AddProductWithConsignmentView
                    {
                        DataContext = new AddProductWithConsignmentViewModel(_db)
                    };
                    addWnd.Owner = Window.GetWindow(mainWnd);
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                }
            ));
        }

        private readonly NavigationService navigation;
        private readonly ProjectContext _db;

        public MainViewModel(NavigationService navigation, ProjectContext db, bool isSelectionMode = false)
        {
            Messenger.Default.Register<UpdateProducts>(this, param => SearchBy() );
            Messenger.Default.Register<UpdateGroupsMsg>(this, param => SearchBy() );

            ConvertersStaticData.MinValue = 50;
            this.navigation = navigation;
            this._db = db;

            IsSelectionMode = isSelectionMode;

            if (isSelectionMode)
            {
                SelectionVisibility = Visibility.Visible;
                MainVisibility = Visibility.Collapsed;
            }
            else
            {
                SelectionVisibility = Visibility.Collapsed;
                MainVisibility = Visibility.Visible;
            }

            LoadProducts();

            Collection = new ListCollectionView(_prd);
        }
    }
}
