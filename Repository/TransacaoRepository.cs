using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;

namespace ClicBank.Repository
{
    public class TransacaoRepository(Context _context) : ITransacaoRepository
    {
        public async Task Add(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Transacao> Get() => _context.Transacoes.AsQueryable();
    }
}
