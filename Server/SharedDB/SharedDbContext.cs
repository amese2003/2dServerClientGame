using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDB
{
    public class SharedDbContext : DbContext
    {
        public DbSet<TokenDB> Tokens { get; set; }
        public DbSet<ServerDb> Servers { get; set; }

        // GameServer
        public SharedDbContext()
        {
           
        }
        
        // ASP.NET
        public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
        {

        }


        public static string ConnectionString { get; set; } = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=SharedDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options
                    //.UseLoggerFactory(_logger)
                    .UseSqlServer(ConnectionString);
            }
        }        
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TokenDB>()
                .HasIndex(t => t.AccountDbId)
                .IsUnique();


            builder.Entity<ServerDb>()
                .HasIndex(s => s.Name)
                .IsUnique();
        }

    }
}
