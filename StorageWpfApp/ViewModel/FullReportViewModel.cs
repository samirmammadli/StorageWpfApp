using GalaSoft.MvvmLight;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StorageWpfApp.ViewModel
{
    public class FullReportViewModel : ViewModelBase
    {
        private ProjectContext _db { get; }

        private ListCollectionView _invoices;
        public ListCollectionView Invoices
        {
            get { return _invoices; }
            set { _invoices = value; }
        }

        public FullReportViewModel(ProjectContext db)
        {
            _db = db;
            Invoices  = new ListCollectionView( _db.Invoices.Local.ToObservableCollection());
        }

    }
}
