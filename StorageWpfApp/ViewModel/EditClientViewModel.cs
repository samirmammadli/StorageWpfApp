using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using StorageWpfApp.Entities;
using StorageWpfApp.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class EditClientViewModel : ViewModelBase
    {

        private ProjectContext _db;

        private Client _selectedClient;

        public EditClientViewModel(ProjectContext db, Client client)
        {
            _db = db;
            _selectedClient = client;

            if (client != null)
            {
                ClientName = client.Name;
                ClientCode = client.Id.ToString();
                ClientSurname = client.Surname;
                ClientPhone = client.PhoneNumber;
                ClientMail = client.Email;
            }
        }

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

        private bool CheckFields()
        {
            return !(string.IsNullOrWhiteSpace(ClientName) && string.IsNullOrWhiteSpace(ClientMail) && string.IsNullOrWhiteSpace(ClientSurname) && string.IsNullOrWhiteSpace(ClientMail) && string.IsNullOrWhiteSpace(ClientPhone));
        }

        private void AddChanges()
        {
            _selectedClient.Email = ClientMail;
            _selectedClient.Name = ClientName;
            _selectedClient.Surname = ClientSurname;
            _selectedClient.PhoneNumber = ClientPhone;
        }

        private bool AddClient()
        {
            
            if (!string.IsNullOrWhiteSpace(ClientMail))
            {
                var byMail = _db.Clients.FirstOrDefault(c => c.Email.Equals(ClientMail, StringComparison.OrdinalIgnoreCase));
                if (byMail != null && byMail.Id != _selectedClient.Id)
                {
                    MessageBox.Show("Клиент с такой почтой уже существует!");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(ClientPhone))
            {
                var byPhone = _db.Clients.FirstOrDefault(c => c.PhoneNumber.Equals(ClientPhone, StringComparison.OrdinalIgnoreCase));
                if (byPhone != null && byPhone.Id != _selectedClient.Id)
                {
                    MessageBox.Show("Клиент с таким телефоном уже существует!");
                    return false;
                }
            }
            
            try
            {
                AddChanges();
                _db.SaveChanges();
                Messenger.Default.Send(new UpdateClientsMsg());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }

            MessageBox.Show($"Клиент успешно изменен!");

            return true;
        }

        public RelayCommand<Window> _saveChanges;
        public RelayCommand<Window> SaveChanges
        {
            get => _saveChanges ?? (_saveChanges = new RelayCommand<Window>(
                wnd => { if(AddClient()) wnd.Close(); }, wnd => CheckFields()
                ));
        }
    }
}
