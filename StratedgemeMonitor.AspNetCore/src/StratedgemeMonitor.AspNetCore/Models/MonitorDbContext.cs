﻿using Capital.GSG.FX.Data.Core.ContractData;
using Capital.GSG.FX.Data.Core.ExecutionData;
using Capital.GSG.FX.Data.Core.MarketData;
using Capital.GSG.FX.Data.Core.OrderData;
using Microsoft.EntityFrameworkCore;

namespace StratedgemeMonitor.AspNetCore.Models
{
    public class MonitorDbContext : DbContext
    {
        public MonitorDbContext(DbContextOptions<MonitorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Execution> Executions { get; set; }
        public DbSet<FXEvent> FXEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>().HasKey(c => new { c.Broker, c.ContractID });

            modelBuilder.Entity<FXEvent>().HasKey(e => e.EventId);
        }
    }
}
