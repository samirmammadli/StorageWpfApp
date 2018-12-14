using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods.RepoExtentions
{
    public static class ProjectRepoExtensions
    {
        public static int ProductConsignmentCount(this ProjectContext db, Product product)
        {
            return db.Consignments.Count(x => x.Product == product);
        }

        public static void AddCodeToConsignment(this ProjectContext db, Consignment cons)
        {
            if (cons != null)
            {
                var findedCons = db.Consignments.Local.Where(x => x.Product == cons.Product);

                if (findedCons != null && findedCons.Any())
                    cons.Code = findedCons.Max(x => x.Code) + 1;
                else
                    cons.Code = 1;
            }
        }
    }
}
