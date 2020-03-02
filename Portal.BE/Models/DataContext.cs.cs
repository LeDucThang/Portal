using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Portal.Models
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<ApplicationUserDAO> ApplicationUser { get; set; }
        public virtual DbSet<PageDAO> Page { get; set; }
        public virtual DbSet<PermissionDAO> Permission { get; set; }
        public virtual DbSet<PermissionDataDAO> PermissionData { get; set; }
        public virtual DbSet<PermissionPageMappingDAO> PermissionPageMapping { get; set; }
        public virtual DbSet<ProviderDAO> Provider { get; set; }
        public virtual DbSet<ProviderTypeDAO> ProviderType { get; set; }
        public virtual DbSet<RoleDAO> Role { get; set; }
        public virtual DbSet<SiteDAO> Site { get; set; }
        public virtual DbSet<UserRoleMappingDAO> UserRoleMapping { get; set; }
        public virtual DbSet<UserStatusDAO> UserStatus { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=.;initial catalog=Portal;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<ApplicationUserDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.ApplicationUsers)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUser_Provider");

                entity.HasOne(d => d.UserStatus)
                    .WithMany(p => p.ApplicationUsers)
                    .HasForeignKey(d => d.UserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationUser_UserStatus");
            });

            modelBuilder.Entity<PageDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(3000);
            });

            modelBuilder.Entity<PermissionDAO>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Permissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_Role");
            });

            modelBuilder.Entity<PermissionDataDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FilterName).HasMaxLength(500);

                entity.Property(e => e.FilterType).HasMaxLength(500);

                entity.Property(e => e.FilterValue).HasMaxLength(3000);

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionDatas)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionData_Permission");
            });

            modelBuilder.Entity<PermissionPageMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.PermissionId, e.PageId })
                    .HasName("PK_PermissionAction");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PermissionPageMappings)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionPageMapping_Page");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.PermissionPageMappings)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionAction_Permission");
            });

            modelBuilder.Entity<ProviderDAO>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Value).HasMaxLength(3000);

                entity.HasOne(d => d.ProviderType)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.ProviderTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Provider_ProviderType");
            });

            modelBuilder.Entity<ProviderTypeDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<RoleDAO>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<SiteDAO>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.URL)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRoleMappingDAO>(entity =>
            {
                entity.HasKey(e => new { e.ApplicationUserId, e.RoleId });

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.UserRoleMappings)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoleMapping_ApplicationUser");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoleMappings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoleMapping_Role");
            });

            modelBuilder.Entity<UserStatusDAO>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            OnModelCreatingExt(modelBuilder);
        }

        partial void OnModelCreatingExt(ModelBuilder modelBuilder);
    }
}
