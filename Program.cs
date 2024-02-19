using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.Services;
using ClicBank.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddScoped<IClicBankService, ClicBankService>()
    .AddDbContext<Context>(opt => opt.UseNpgsql(connection));

var app = builder.Build();

app.MapGet("/helloWorld", () => "Hello World");

app.MapPost("/clientes/{id}/transacoes", async Task<IResult>(string id, TransacaoDto transacaoDto, IClicBankService service) =>
{
    if (!int.TryParse(id, out int clienteId))
        return Results.UnprocessableEntity();

    if (!int.TryParse(transacaoDto.valor.ToString(), out int valor))
        return Results.UnprocessableEntity();

    if (clienteId < 1 || clienteId > 5)
        return Results.NotFound();

     if (String.IsNullOrEmpty(transacaoDto.descricao) || transacaoDto.descricao.Length > 10)
         return Results.UnprocessableEntity();

     if (transacaoDto.tipo != 'c' && transacaoDto.tipo != 'd')
         return Results.UnprocessableEntity();

     return await service.AddTransacao(clienteId, new Transacao(clienteId, valor, transacaoDto.tipo, transacaoDto.descricao));
});

app.MapGet("clientes/{id}/extrato", async Task<IResult> (int id, IClicBankService service) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    return await service.GetExtrato(id);
});

app.MapPut("reset", async Task (IClicBankService service) => await service.Reset());

app.Run();
