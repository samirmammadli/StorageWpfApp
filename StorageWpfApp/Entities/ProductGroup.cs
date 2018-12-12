using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class ProductGroup : ObservableObject
    {
        public int Id { get; set; }

        [NotMapped]
        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

    }
}
