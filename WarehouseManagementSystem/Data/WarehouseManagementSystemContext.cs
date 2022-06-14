using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Data
{
    public class WarehouseManagementSystemContext : DbContext
    {
        public WarehouseManagementSystemContext (DbContextOptions<WarehouseManagementSystemContext> options)
            : base(options)
        {
        }

        public DbSet<WarehouseManagementSystem.Models.Product>? Products { get; set; }

        public DbSet<WarehouseManagementSystem.Models.Warehouse>? Warehouse { get; set; }
    }
}
