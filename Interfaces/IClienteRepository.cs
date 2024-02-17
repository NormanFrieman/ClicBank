using ClicBank.Entities;
using ClicBank.Infra;

namespace ClicBank.Interfaces
{
    public interface IClienteRepository
    {
        IQueryable<Cliente> Get();
        Task Update(Cliente cliente);
        Context GetContext();
    }
}
