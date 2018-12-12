using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Navigation
{
    enum VM
    {
        ToDoList,
        Add,
        EditProduct
    }

    class NavigationService 
    {
        private Dictionary<VM, ViewModelBase> pages = new Dictionary<VM, ViewModelBase>();
        public ViewModelBase Current { get; set; }

        public void NavigateTo(VM name)
        {
            try
            {
                Current = pages[name];
                Messenger.Default.Send(Current);
            }
            catch (Exception)
            {
                throw new Exception("Page not found!");
            }
        }

        public void NavigateTo<T>(VM name, T data)
        {
            try
            {
                Current = pages[name];
                if (data != null)
                    Messenger.Default.Send(data);
                Messenger.Default.Send(Current);
            }
            catch (Exception)
            {
                throw new Exception("Page not found!");
            }
        }

        public void AddPage(ViewModelBase page, VM name)
        {
            if (pages.ContainsKey(name))
                pages[name] = page;
            else
                pages.Add(name, page);
        }

        public void RemovePage(VM name)
        {
            try
            {
                pages.Remove(name);
            }
            catch (Exception)
            {
                throw new Exception("Page not found!");
            }
        }
    }
}
