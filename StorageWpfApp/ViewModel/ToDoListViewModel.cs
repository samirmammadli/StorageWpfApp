using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StorageWpfApp.Messages;
using StorageWpfApp.Model;
using StorageWpfApp.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class ToDoListViewModel : ViewModelBase
    {
        private ObservableCollection<ToDo> list = new ObservableCollection<ToDo>();
        public ObservableCollection<ToDo> List
        {
            get { return list; }
            set { Set(ref list, value); }
        }

        private readonly NavigationService navigation;

        public ToDoListViewModel(NavigationService navigation)
        {
            this.navigation = navigation;
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get => addCommand ?? (addCommand = new RelayCommand(
                () => navigation.NavigateTo(VM.Add) 
            ));
        }
    }
}
