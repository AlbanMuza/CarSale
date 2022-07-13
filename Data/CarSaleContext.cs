using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarSale.Models;

namespace CarSale.Data
{
    public class CarSaleContext : DbContext
    {
        public CarSaleContext (DbContextOptions<CarSaleContext> options)
            : base(options)
        {
        }

        public DbSet<CarSale.Models.Car>? Car { get; set; }
    }
}
