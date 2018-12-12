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
    class AddNewClientViewModel : ViewModelBase
    {

        private ProjectContext _db;


        public AddNewClientViewModel(ProjectContext db)
        {
            _db = db;
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

        private bool AddClient()
        {
            var client = new Client { Email = ClientMail, Name = ClientName, Surname = ClientSurname, PhoneNumber = ClientPhone };

            if (!string.IsNullOrWhiteSpace(ClientPhone) && _db.Clients.Any(c => c.PhoneNumber.Equals(ClientPhone, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Клиент с таким телефоном уже существует!");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(ClientMail) && _db.Clients.Any(c => c.Email.Equals(ClientMail, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Клиент с такой почтой уже существует!");
                return false;
            }
            try
            {

                _db.Clients.Add(client);
                _db.SaveChanges();
                Messenger.Default.Send(new UpdateClientsMsg());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }

            MessageBox.Show($"Клиент успешно добавлен!\nКод: {client.Id}");

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
