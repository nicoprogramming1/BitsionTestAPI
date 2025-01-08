using BitsionTest.API.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BitsionTest.API.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // para asegurar la configuración predeterminada de Identity

            /**
             * Esto filtra todas las consultas que se hagan para clientes, comparando uno a uno si su atributo
             * isDeleted es true, es decir si ha sido eliminado lógicamente (soft delete)
             */
            builder.Entity<Client>().HasQueryFilter(c => !c.isDeleted);
        }

        public DbSet<Client> Clients { get; set; }

        public override int SaveChanges()
        {
            //HandleTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //HandleTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        /**
        private void HandleTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is Client &&
                                                              (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((Client)entry.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((Client)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
        */
    }
}
