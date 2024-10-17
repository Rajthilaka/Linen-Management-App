using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TDSSWebApplication.Models;

public partial class LinenManagementContext : DbContext
{
    public LinenManagementContext()
    {
    }

    public LinenManagementContext(DbContextOptions<LinenManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartLog> CartLogs { get; set; }

    public virtual DbSet<CartLogDetail> CartLogDetails { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Linen> Linens { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasComment("Valid Values = SOILED, CLEAN");
            entity.Property(e => e.Weight).HasComment("Weight in Pounds (lb)");
        });

        modelBuilder.Entity<CartLog>(entity =>
        {
            entity.ToTable("CartLog");

            entity.Property(e => e.ActualWeight).HasComment("Weight of linen in pounds (lb) received by client");
            entity.Property(e => e.Comments).HasMaxLength(2000);
            entity.Property(e => e.DateWeighed)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.EmployeeId).HasComment("Employee that has weighed the cart");
            entity.Property(e => e.LocationId).HasComment("Location where the cart is kept");
            entity.Property(e => e.ReceiptNumber)
                .HasMaxLength(50)
                .HasComment("The delivery receipt number from the laundry vendor");
            entity.Property(e => e.ReportedWeight).HasComment("Total weight of linen in Punds (lb) that is reported by the vendor.");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartLogs)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartLog_Carts");

            entity.HasOne(d => d.Employee).WithMany(p => p.CartLogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartLog_Employees");

            entity.HasOne(d => d.Location).WithMany(p => p.CartLogs)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartLog_Locations");
        });

        modelBuilder.Entity<CartLogDetail>(entity =>
        {
            entity.HasKey(e => e.CartLogDetailId).HasName("PK_CartLogDetatil");

            entity.ToTable("CartLogDetail");

            entity.HasOne(d => d.CartLog).WithMany(p => p.CartLogDetails)
                .HasForeignKey(d => d.CartLogId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CartLogDetatil_CartLog");

            entity.HasOne(d => d.Linen).WithMany(p => p.CartLogDetails)
                .HasForeignKey(d => d.LinenId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CartLogDetatil_Linen");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasComment("Used by JWT authentication to refresh access token");
        });

        modelBuilder.Entity<Linen>(entity =>
        {
            entity.ToTable("Linen");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Weight).HasColumnType("decimal(6, 2)");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasComment("Valid Values = SOILED_ROOM, CLEAN_ROOM");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
