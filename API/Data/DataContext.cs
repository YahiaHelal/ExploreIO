using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    //NOTE: this is the ORM dealing with the DB
    public class DataContext : DbContext
    {
        // passing some options in the startup class to add it to the DI Container
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected DataContext()
        {
        }
        public DbSet<AppUser> Users {get;set;}
    }
}