using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Model
{
    class ToDo : ObservableObject
    {
        private string title;
        public string Title { get => title; set => Set(ref title, value); }

        private bool done;
        public bool Done { get => done; set => Set(ref done, value); }
    }
}
