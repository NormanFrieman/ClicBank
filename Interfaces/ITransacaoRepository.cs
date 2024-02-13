using ClicBank.Entities;

namespace ClicBank.Interfaces
{
    public interface ITransacaoRepository
    {
        Task Add(Transacao transacao);
        IQueryable<Transacao> Get();
    }
}
