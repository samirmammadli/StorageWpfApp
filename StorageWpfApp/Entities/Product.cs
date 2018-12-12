using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.Entities
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int? GroupId { get; set; }

        public int? PieceQuantity { get; set; }

        public ProductGroup Group { get; set; }

        public List<Consignment> Consignments { get; set; }

        public bool IsPieceProduct { get; set; }
    }
}


//партия