using ClicBank.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClicBank.Infra
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Transacoes)
                .WithOne(t => t.Cliente)
                .HasForeignKey(t => t.ClienteId)
                .HasPrincipalKey(c => c.Id);

            modelBuilder.Entity<Cliente>()
                .HasData(
                    new Cliente { Id = 1, Limite = 100000, Saldo = 0 },
                    new Cliente { Id = 2, Limite = 80000, Saldo = 0 },
                    new Cliente { Id = 3, Limite = 1000000, Saldo = 0 },
                    new Cliente { Id = 4, Limite = 10000000, Saldo = 0 },
                    new Cliente { Id = 5, Limite = 500000, Saldo = 0 }
                );
        }
    }
}
