using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ClicBank.Services
{
    public class ClicBankService : IClicBankService
    {
        private readonly Context _context;
        public ClicBankService(Context context) =>
            _context = context;

        public async Task<IResult> AddTransacao(int id, TransacaoDto transacaoDto)
        {
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            var tran = conn.BeginTransaction(IsolationLevel.Serializable);
            _context.Database.UseTransaction(tran);

            var cliente = await _context.Clientes.SingleOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
                throw new Exception("Ih rapaz...");

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

            await _context.SaveChangesAsync();

            tran.Commit();
            conn.Close();

            return Results.Ok(new SaldoResumo(cliente));
        }

        public async Task<IResult> GetExtrato(int id)
        {
            var cliente = await _context
                .Clientes
                .Include(x => x.Transacoes)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
                throw new Exception("Ih rapaz...");


            return Results.Ok(new ExtratoDto(cliente));
        }

        public async Task Reset()
        {
            await _context.Transacoes.ExecuteDeleteAsync();

            var clientes = await _context.Clientes.ToArrayAsync();
            var limites = new int[] { 100000, 80000, 1000000, 10000000, 500000 };

            foreach (var (i, cliente) in clientes.OrderBy(x => x.Id).Select((cliente, i) => ( i, cliente )))
            {
                cliente.Limite = limites[i];
                cliente.Saldo = 0;

                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
        }
    }
}
