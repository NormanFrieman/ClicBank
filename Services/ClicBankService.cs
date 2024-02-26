using ClicBank.ViewModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace ClicBank.Services
{
    public static class ClicBankService
    {
        private readonly static Dictionary<int, int> _limites = new()
        {
            { 1, 100000 },
            { 2, 80000 },
            { 3, 1000000 },
            { 4, 10000000 },
            { 5, 500000 }
        };

        public static async Task<IResult> AddTransacao(int id, int valor, char tipo, string descricao, string connStr)
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            var dataReader = await conn.ExecuteReaderAsync(_sqlAtualizaSaldo, new { id, valor, tipo, descricao });
            await dataReader.ReadAsync();

            if (dataReader.GetBoolean(1))
                return Results.UnprocessableEntity();

            var saldo = dataReader.GetInt32(0);

            _limites.TryGetValue(id, out int limite);
            return Results.Ok(new SaldoResumo(saldo, limite));
        }

        public static async Task<IResult> GetExtrato(int id, string connStr)
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            var res = await conn.QueryMultipleAsync(_sqlExtrato, new { id });
            var saldo = await res.ReadSingleAsync<int>();
            var transacoes = (await res.ReadAsync<(int, char, string, DateTime)>())
                .Select(x => new TransacaoHistorico(x.Item1, x.Item2, x.Item3, x.Item4));

            _limites.TryGetValue(id, out int limite);
            var saldoDto = new SaldoDto() { limite = limite, total = saldo };
            return Results.Ok(new ExtratoDto(saldoDto, transacoes));
        }

        public static async Task Reset(string connStr)
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            await conn.ExecuteAsync(_sqlReset);
        }

        #region Scripts
        private static readonly string _sqlReset = @"
            DELETE FROM public.""Transacoes"";
            DELETE FROM public.""Clientes"";
            
            INSERT INTO public.""Clientes"" (""Id"", ""Limite"", ""Saldo"")
            VALUES
                (1, 100000, 0),
                (2, 80000, 0),
                (3, 1000000, 0),
                (4, 10000000, 0),
                (5, 500000, 0);";

        private static readonly string _sqlAtualizaSaldo = @"
            SELECT ""saldo"", ""erro"" FROM atualiza_saldo(@id, @valor, @tipo, @descricao);";

        private static readonly string _sqlExtrato = @"
            SELECT
	            c.""Saldo""
            FROM ""Clientes"" c
            WHERE c.""Id"" = @id;

            SELECT
                t.""Valor"", t.""Tipo"", t.""Descricao"", t.""Data"" FROM ""Transacoes"" t
            WHERE t.""ClienteId"" = @id
            ORDER BY t.""Data"" desc
            LIMIT 10;";
        #endregion
    }
}
