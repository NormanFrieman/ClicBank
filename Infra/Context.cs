using ClicBank.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClicBank.Infra
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
    }
}
