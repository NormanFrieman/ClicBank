using ClicBank.Entities;

namespace ClicBank.Interfaces
{
    public interface IClienteRepository
    {
        IQueryable<Cliente> Get();
        Task Update(Cliente cliente);
    }
}
