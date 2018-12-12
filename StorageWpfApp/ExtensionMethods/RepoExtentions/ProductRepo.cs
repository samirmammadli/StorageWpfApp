using Microsoft.EntityFrameworkCore;
using StorageWpfApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageWpfApp.ExtensionMethods.RepoExtentions
{
    public static class DbContextExtention
    {
        public static int ProductConsignmentCount(this ProjectContext db, Product product)
        {
            return db.Consignments.Count(x => x.Product == product);
        }
    }
}
