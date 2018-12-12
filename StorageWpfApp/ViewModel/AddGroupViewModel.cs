using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class AddGroupViewModel : ViewModelBase
    {
        private readonly ProjectContext _db;

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set => Set(ref _groupName, value);
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


        private bool CheckIfGroupExists()
        {
             return _db.Groups.Any(x => x.Name.Equals(GroupName, StringComparison.OrdinalIgnoreCase));
        }

        private RelayCommand<Window> _saveChanges;
        public RelayCommand<Window> SaveChanges
        {
            get =>
                _saveChanges ?? (_saveChanges = new RelayCommand<Window>(prm =>
                {
                    if (CheckIfGroupExists())
                    {
                        var answ = MessageBox.Show("Категория с таким именем уже существует!\nДобавить?", "Внимание!", MessageBoxButton.YesNo);
                        if (answ == MessageBoxResult.No)
                            return;
                    }
                    _db.Groups.Add(new ProductGroup { Name = GroupName });
                    _db.SaveChanges();
                    prm.Close();
                },
                   prm => !string.IsNullOrWhiteSpace(GroupName)
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

        public AddGroupViewModel(ProjectContext db)
        {
            this._db = db;
        }
    }
}
