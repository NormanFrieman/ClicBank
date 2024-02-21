﻿using ClicBank.Entities;
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

        public async Task<IResult> AddTransacao(int id, Transacao transacao)
        {
            var connString = _context.Database.GetConnectionString();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sql = $"select \"id\", \"limite\", \"saldo\" from atualiza_saldo({id}, {transacao.Valor}, '{transacao.Tipo}', '{transacao.Descricao}');";
            var cliente = await conn.QuerySingleAsync<Cliente>(sql);
            if (cliente.Id == 0)
                return Results.UnprocessableEntity();

            return Results.Ok(new SaldoResumo(cliente));
        }

        public async Task<IResult> GetExtrato(int id)
        {
            var connString = _context.Database.GetConnectionString();
            using var conn = new NpgsqlConnection(connString);
            var sql = $@"
                select
	                c.""Id"", c.""Limite"", c.""Saldo""
                from ""Clientes"" c
                where c.""Id"" = {id};

                select
                    t.""Valor"", t.""Tipo"", t.""Descricao"", t.""Data"" from ""Transacoes"" t
                where t.""ClienteId"" = {id}
                order by t.""Data"" desc
                limit 10;";

            var res = await conn.QueryMultipleAsync(sql);
            var cliente = await res.ReadSingleAsync<Cliente>();
            var transacoes = (await res.ReadAsync<Transacao>()).ToList();
            cliente.Transacoes = transacoes;

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
