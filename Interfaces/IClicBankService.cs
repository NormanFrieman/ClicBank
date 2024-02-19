using ClicBank.Entities;

namespace ClicBank.Interfaces
{
    public interface IClicBankService
    {
        Task<IResult> AddTransacao(int id, Transacao transacao);
        Task<IResult> GetExtrato(int id);
        Task Reset();
    }
}
