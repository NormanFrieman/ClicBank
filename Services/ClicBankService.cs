using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClicBank.Services
{
    public class ClicBankService : IClicBankService
    {
        private readonly Context _context;
        public ClicBankService(Context context) =>
            _context = context;

        public async Task<IResult> AddTransacao(int id, TransacaoDto transacaoDto)
        {
            _context.Database.BeginTransaction();

            var cliente = await _context.Clientes.SingleOrDefaultAsync(x => x.Id == id);

            switch (transacaoDto.tipo)
            {
                case 'c':
                    cliente.Saldo += transacaoDto.valor;
                    break;
                case 'd':
                    cliente.Saldo -= transacaoDto.valor;
                    break;
                default:
                    return Results.UnprocessableEntity();
            }

            if (cliente.Saldo + cliente.Limite <= 0)
                return Results.UnprocessableEntity();

            _context.Clientes.Update(cliente);
            _context.Transacoes.Add(new Transacao(cliente, transacaoDto));

            _context.Database.CommitTransaction();

            await _context.SaveChangesAsync();

            return Results.Ok(new SaldoResumo(cliente));
        }

        public async Task<IResult> GetExtrato(int id)
        {
            var cliente = await _context
                .Clientes
                .Include(x => x.Transacoes)
                .SingleOrDefaultAsync(x => x.Id == id);

            return Results.Ok(new ExtratoDto(cliente));
        }
    }
}
