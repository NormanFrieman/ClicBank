using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;

namespace ClicBank.Repository
{
    public class ClienteRepository(Context _context) : IClienteRepository
    {
        public async Task Update(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Cliente> Get() => _context.Clientes.AsQueryable();
    }
}
