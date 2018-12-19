using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using StorageWpfApp;
using StorageWpfApp.Entities;
using StorageWpfApp.Messages;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StorageWpfApp.ViewModel
{
    class AppViewModel : ViewModelBase
    {
        public Action CloseAction { get; set; }

        public string AppVersion { get; set; }

        private readonly ProjectContext _db;

        private ViewModelBase currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return currentViewModel; }
            set { Set(ref currentViewModel, value); }
        }

        public AppViewModel(ProjectContext db)
        {
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Messenger.Default.Register<ViewModelBase>(this,
                param => CurrentViewModel = param);
            _db = db;
        }

        private RelayCommand<Window> _addProduct;
        public RelayCommand<Window> AddProduct
        {
            get => _addProduct ?? (_addProduct = new RelayCommand<Window>(
                mainWnd =>
                {
                    var addWnd = new AddProductWithConsignmentView
                    {
                        DataContext = new AddProductWithConsignmentViewModel(_db)
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                    Messenger.Default.Send(new UpdateProducts());
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

        private RelayCommand<Window> _clientsList;
        public RelayCommand<Window> ClientsList
        {
            get => _clientsList ?? (_clientsList = new RelayCommand<Window>(
                mainWnd =>
                {
                    var addWnd = new SelectClientView
                    {
                        DataContext = new SelectClientViewModel(_db)
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addWnd.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _fullReport;
        public RelayCommand<Window> FullReport
        {
            get => _fullReport ?? (_fullReport = new RelayCommand<Window>(
                mainWnd =>
                {
                    var addWnd = new FullReportView
                    {
                        DataContext = new FullReportViewModel(_db)
                    };
                    addWnd.Owner = mainWnd;
                    addWnd.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _consignmentsList;
        public RelayCommand<Window> ConsignmentsList
        {
            get => _consignmentsList ?? (_consignmentsList = new RelayCommand<Window>(
                mainWnd =>
                {
                    var cWnd = new ConsignmentsView
                    {
                        DataContext = new ConsignmentsViewModel(_db)
                    };
                    cWnd.Owner = mainWnd;
                    cWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    cWnd.ShowDialog();
                   // GC.Collect();
                }
            ));
        }


        private RelayCommand _addGroup;
        public RelayCommand AddGroup
        {
            get => _addGroup ?? (_addGroup = new RelayCommand(
                () =>
                {
                    var addgrp = new AddGroupView
                    {
                        DataContext = new AddGroupViewModel(_db)
                    };
                    addgrp.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _editGroup;
        public RelayCommand<Window> EditGroup
        {
            get => _editGroup ?? (_editGroup = new RelayCommand<Window>(
                wnd =>
                {
                    var vm = new EditGroupViewModel(_db);
                    var editgrp = new EditGroupView
                    {
                        DataContext = vm,
                        Owner = wnd,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    
                    editgrp.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _addConsignment;
        public RelayCommand<Window> AddConsignment
        {
            get => _addConsignment ?? (_addConsignment = new RelayCommand<Window>(
                wnd =>
                {
                    var vm = new AddNewConsignmentViewModel(_db);
                    var addCons = new AddNewConsignmentView
                    {
                        DataContext = vm,
                        Owner = wnd,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    addCons.ShowDialog();
                }
            ));
        }

        private RelayCommand<Window> _composeInvoice;
        public RelayCommand<Window> ComposeInvoice
        {
            get => _composeInvoice ?? (_composeInvoice = new RelayCommand<Window>(
                wnd =>
                {
                    var compose = new ComposeInvoiceView
                    {
                        Owner = wnd,
                        DataContext = new ComposeInvoiceViewModel(_db)
                    };

                    compose.ShowDialog();
                }
            ));
        }

        private RelayCommand _closeAppConmmand;
        public RelayCommand CloseAppConmmand
        {
            get => _closeAppConmmand ?? (_closeAppConmmand = new RelayCommand(
                () => {
                        CloseAction?.Invoke();
                }
            ));
        }

    }
}
