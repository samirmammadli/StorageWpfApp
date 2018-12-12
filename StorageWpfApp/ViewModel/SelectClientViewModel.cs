using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using StorageWpfApp.Entities;
using StorageWpfApp.Messages;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StorageWpfApp.ViewModel
{
    class SelectClientViewModel : ViewModelBase
    {
        private ProjectContext _db;

        private ListCollectionView _clients;
        public ListCollectionView Clients
        {
            get { return _clients; }
            set => Set(ref _clients, value);
        }


        public SelectClientViewModel(ProjectContext db)
        {
            Messenger.Default.Register<UpdateClientsMsg>(this, prm => Search() );
            _db = db;
            _clients = new ListCollectionView(_db.Clients.Local.ToObservableCollection());
        }

        public Visibility IsVisible { get; set; } = Visibility.Collapsed;

        private string _clientName = "";
        public string ClientName
        {
            get => _clientName;
            set => Set(ref _clientName, value);
        }

        private string _clientSurname = "";
        public string ClientSurname
        {
            get => _clientSurname;
            set => Set(ref _clientSurname, value);
        }

        private string _clientMail = "";
        public string ClientMail
        {
            get => _clientMail;
            set => Set(ref _clientMail, value);
        }

        private string _clientPhone = "";
        public string ClientPhone
        {
            get => _clientPhone;
            set => Set(ref _clientPhone, value);
        }

        private string _clientCode = "";
        public string ClientCode
        {
            get => _clientCode;
            set => Set(ref _clientCode, value);
        }

        private void Search()
        {
            Clients.Filter = c =>
            {
                var client = c as Client;
                return client.Email.Contains(ClientMail) &&
                        client.Id.ToString().Contains(ClientCode) &&
                        client.Name.ToLower().Contains(ClientName.ToLower()) &&
                        client.Surname.ToLower().Contains(ClientSurname.ToLower()) &&
                        client.PhoneNumber.Contains(ClientPhone);
            };

        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
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

        private RelayCommand<Window> _addNewClient;
        public RelayCommand<Window> AddNewClient
        {
            get => _addNewClient ?? (_addNewClient = new RelayCommand<Window>(
                mainWnd =>
                {
                    var addWnd = new AddNewClientView
                    {
                        DataContext = new AddNewClientViewModel(_db)
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _selectClient;
        public RelayCommand<Window> SelectClient
        {
            get => _selectClient ?? (_selectClient = new RelayCommand<Window>(
                mainWnd =>
                {
                    if (IsVisible == Visibility.Visible)
                    {
                        mainWnd.Close();
                        return;
                    }

                    var editWnd = new AddNewClientView
                    {
                        Owner = mainWnd,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        DataContext = new EditClientViewModel(_db, SelectedClient)
                    };
                    editWnd.ShowDialog();
                }, mainWnd => SelectedClient != null
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

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get => _searchCommand ?? (_searchCommand = new RelayCommand(
               Search
            ));
        }

        private RelayCommand _deleteClient;
        public RelayCommand DeleteClient
        {
            get => _deleteClient ?? (_deleteClient = new RelayCommand(
               () =>
               {
                   try
                   {
                       if (SelectedClient.Debit > 0)
                       {
                           MessageBox.Show("У клиента имеется долг!", "Невозможно удалить!", MessageBoxButton.OK, MessageBoxImage.Warning);
                           return;
                       }
                       _db.Clients.Remove(SelectedClient);
                       _db.SaveChanges();
                   }
                   catch (Exception ex)
                   {
                       MessageBox.Show(ex.Message);
                   }
               }, () => SelectedClient != null
            ));
        }


        //SelectItemsCommand

    }
}
