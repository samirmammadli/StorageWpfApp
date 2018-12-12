using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using StorageWpfApp.Messages;
using StorageWpfApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StorageWpfApp.ViewModel
{
    class EditGroupViewModel : ViewModelBase
    {
        private ProjectContext _db;

        private ObservableCollection<ProductGroup>  _groups;
        public ObservableCollection<ProductGroup> Groups
        {
            get { return _groups; }
            set => Set(ref _groups, value);
        }

        public EditGroupViewModel(ProjectContext db)
        {
            this._db = db;
            SetGroupsSource();
        }

        private void SetGroupsSource()
        {
            Groups = _db.Groups.Local.ToObservableCollection();
        }

        private ProductGroup _selectedGroup;
        public ProductGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                Set(ref _selectedGroup, value);
                if (SelectedGroup != null)
                {
                    GroupName = SelectedGroup.Name;
                    SelectedGroupPrdCount = _db.Products.Count(x => x.Group == SelectedGroup).ToString();
                }
            }
        }

        private string _selectedGroupPrdCount;
        public string SelectedGroupPrdCount
        {
            get { return _selectedGroupPrdCount; }
            set => Set(ref _selectedGroupPrdCount, value);
        }


        private ProductGroup _fromGroup;
        public ProductGroup FromGroup
        {
            get { return _fromGroup; }
            set => Set(ref _fromGroup, value);
        }

        private ProductGroup _toGroup;
        public ProductGroup ToGroup
        {
            get { return _toGroup; }
            set => Set(ref _toGroup, value);
        }

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set => Set(ref _groupName, value);
        }

        private bool _isFromListChecked = true;
        public bool IsFromListChecked
        {
            get { return _isFromListChecked; }
            set => Set(ref _isFromListChecked, value);
        }

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get { return _isAllChecked; }
            set => Set(ref _isAllChecked, value);
        }

        private bool _isAllUngrouppedChecked;
        public bool IsAllUngrouppedChecked
        {
            get { return _isAllUngrouppedChecked; }
            set => Set(ref _isAllUngrouppedChecked, value);
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

        private bool CheckNameChange()
        {
            return SelectedGroup != null && !string.IsNullOrWhiteSpace(GroupName) && !SelectedGroup.Name.Equals(GroupName);
        }

        private bool IsMovable()
        {
            if (IsFromListChecked)
                return FromGroup != null && ToGroup != null && FromGroup != ToGroup;
            return ToGroup != null;
        }

        private void Move()
        {
            if (IsFromListChecked)
                _db.Products.Where(x => x.Group == FromGroup).ToList().ForEach(x => x.Group = ToGroup);
            else if (IsAllChecked)
                _db.Products.Where(x => x.Group !=null).ToList().ForEach(x => x.Group = ToGroup);
            else if (_isAllUngrouppedChecked)
                _db.Products.Where(x => x.Group == null).ToList().ForEach(x => x.Group = ToGroup);
            else
                return;

            _db.SaveChanges();
            Messenger.Default.Send<UpdateGroupsMsg>(null);
        }

        private RelayCommand _changeGroupNameCommand;
        public RelayCommand ChangeGroupNameCommand
        {
            get => _changeGroupNameCommand ?? (_changeGroupNameCommand = new RelayCommand(
                () =>
                {
                    SelectedGroup.Name = GroupName;
                    _db.SaveChanges();
                    Messenger.Default.Send<UpdateGroupsMsg>(null);
                }, CheckNameChange
            ));
        }

        private RelayCommand _deleteGroupCommand;
        public RelayCommand DeleteGroupCommand
        {
            get => _deleteGroupCommand ?? (_deleteGroupCommand = new RelayCommand(
                () =>
                {
                    Groups.Remove(SelectedGroup);
                    _db.SaveChanges();
                    Messenger.Default.Send<UpdateGroupsMsg>(null);
                }, () => SelectedGroup != null
            ));
        }

        private RelayCommand<Window> _addGroup;
        public RelayCommand<Window> AddGroup
        {
            get => _addGroup ?? (_addGroup = new RelayCommand<Window>(
                wnd =>
                {
                    var addgrp = new AddGroupView
                    {
                        DataContext = new AddGroupViewModel(_db)
                    };
                    addgrp.Owner = wnd;
                    addgrp.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    addgrp.ShowDialog();
                }
            ));
        }

        private RelayCommand _moveItemsFromGroupCommand;
        public RelayCommand MoveItemsFromGroupCommand
        {
            get => _moveItemsFromGroupCommand ?? (_moveItemsFromGroupCommand = new RelayCommand(Move, IsMovable));
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
