using ClicBank.ViewModels;

namespace ClicBank.Interfaces
{
    public interface IClicBankService
    {
        Task<IResult> AddTransacao(int id, TransacaoDto transacaoDto);
        Task<IResult> GetExtrato(int id);
        Task Reset();
    }
}
