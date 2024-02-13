using ClicBank.Entities;

namespace ClicBank.Infra
{
    public class Seed(Context context) : ISeed
    {
        private readonly Context _context = context;

        public async Task IncluirClientes()
        {
            var limites = new List<int>() { 100000, 80000, 1000000, 10000000, 500000 };

            foreach (var limite in limites)
            {
                var cliente = new Cliente();
                cliente.Limite = limite;

                _context.Clientes.Add(cliente);
            }

            await _context.SaveChangesAsync();
        }
    }

    public interface ISeed
    {
        Task IncluirClientes();
    }
}
