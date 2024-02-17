using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.ViewModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            var connString = _context.Database.GetConnectionString();
            var conn = new NpgsqlConnection(connString);

            conn.Open();

            var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                var cliente = await conn.QuerySingleAsync<Cliente>($@"
                    update ""Clientes""
                    set ""Saldo"" =
	                    (case 
		                    when '{transacaoDto.tipo}' = 'c'
			                    then ""Saldo"" + {transacaoDto.valor}
			                    else ""Saldo"" - {transacaoDto.valor}
	                    end)
                    where ""Id"" = {id};

                    select ""Id"", ""Limite"", ""Saldo"" from ""Clientes"" c where ""Id"" = {id};", transaction: tran);

                if (cliente.Saldo + cliente.Limite <= 0)
                {
                    tran.Rollback();
                    conn.Close();
                    return Results.UnprocessableEntity();
                }

                tran.Commit();
                conn.Close();

                _context.Transacoes.Add(new Transacao(cliente.Id, transacaoDto));
                await _context.SaveChangesAsync();

                return Results.Ok(new SaldoResumo(cliente));
            }
            catch (Exception)
            {
                tran.Rollback();
                conn.Close();

                throw;
            }
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
