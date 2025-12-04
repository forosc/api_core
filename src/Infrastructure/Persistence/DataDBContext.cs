using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities;
using Domain.Model.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public partial class DataDBContext : IdentityDbContext<User>
{
    public DataDBContext()
    {
    }

    public DataDBContext(DbContextOptions<DataDBContext> options)
        : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userName = "system";

        foreach (var entry in ChangeTracker.Entries<BaseDomainModel>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.createDate = DateTime.UtcNow;
                    entry.Entity.createBy = userName;
                    break;
                case EntityState.Modified:
                    entry.Entity.lastModifiedDate = DateTime.UtcNow;
                    entry.Entity.lastModifiedBy = userName;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);

    }

    public virtual DbSet<AltasSuspencione1> AltasSuspenciones1 { get; set; }

    public virtual DbSet<Altassuspencione> Altassuspenciones { get; set; }

    public virtual DbSet<AltassuspencionesBase> AltassuspencionesBases { get; set; }

    public virtual DbSet<Altassuspencionesconcepto> Altassuspencionesconceptos { get; set; }

    public virtual DbSet<CdrResuman> CdrResumen { get; set; }

    public virtual DbSet<Cortesap> Cortesaps { get; set; }

    public virtual DbSet<CortesapTemp> CortesapTemps { get; set; }

    public virtual DbSet<CutoffMultimedium> CutoffMultimedia { get; set; }

    public virtual DbSet<Sapresuman> Sapresumen { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pg_catalog", "adminpack")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("postgres_fdw");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
