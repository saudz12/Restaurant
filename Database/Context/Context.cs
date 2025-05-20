using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Context;

public class RestaurantDatabaseContext : DbContext
{
    public RestaurantDatabaseContext(DbContextOptions<RestaurantDatabaseContext> options)
       : base(options)
    {
    }
    public RestaurantDatabaseContext() { }

    public DbSet<Menu>      Meniuri     { get; set; }
    public DbSet<Preparat>  Preparate   { get; set; }
    public DbSet<User>      Users       { get; set; }
    public DbSet<Comanda>   Comenzi     { get; set; }
    public DbSet<MenuPreparat> MenuPreparat { get; set; }
    public DbSet<PreparatCantitate> PreparatCantitate { get; set; }
    public DbSet<Alergen> Alergeni { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasKey(u => u.Email);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Comenzi)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Comanda entity
        modelBuilder.Entity<Comanda>()
            .HasMany(c => c.PreparatCantitate)
            .WithOne()
            .HasForeignKey(pc => pc.ComandaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure PreparatCantitate entity
        modelBuilder.Entity<PreparatCantitate>()
            .HasOne(pc => pc.Preparat)
            .WithMany()
            .HasForeignKey(pc => pc.PreparatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure MenuPreparat entity
        modelBuilder.Entity<MenuPreparat>()
            .HasOne(mp => mp.Preparat)
            .WithMany()
            .HasForeignKey(mp => mp.PreparatId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MenuPreparat>()
            .HasOne<Menu>()
            .WithMany(m => m.ListaPreparate)
            .HasForeignKey(mp => mp.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Preparat entity
        modelBuilder.Entity<Preparat>()
            .Property(p => p.Pret)
            .HasColumnType("decimal(18,2)");

        // Configure many-to-many relationship between Preparat and Alergen
        modelBuilder.Entity<Preparat>()
            .HasMany(p => p.Alergeni)
            .WithMany(a => a.Preparate)
            .UsingEntity(j => j.ToTable("PreparatAlergen"));

        // Set initial data to Alergen
        modelBuilder.Entity<Alergen>().HasData(
            new Alergen { Id = 1, Nume = "Gluten", TipAlergen = Enums.Alergeni.Gluten },
            new Alergen { Id = 2, Nume = "Oua", TipAlergen = Enums.Alergeni.Oua },
            new Alergen { Id = 3, Nume = "Lactoza", TipAlergen = Enums.Alergeni.Lactoza },
            new Alergen { Id = 4, Nume = "Alune", TipAlergen = Enums.Alergeni.Alune }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("data source=localhost;Initial Catalog=RestaurantMVP2;Persist Security Info=True;User ID=saudshaikh;Password=saudshaikh720;Connection Timeout=60;TrustServerCertificate=True");
        }
    }

}
