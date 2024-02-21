using ClicBank.Services;
using ClicBank.ViewModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Ih rapaz...");

var app = builder.Build();

app.MapGet("/helloWorld", () => "Hello World");

app.MapPost("/clientes/{id}/transacoes", async Task<IResult>(string id, TransacaoDto transacaoDto) =>
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

     return await ClicBankService.AddTransacao(clienteId, valor, transacaoDto.tipo, transacaoDto.descricao, connStr);
});

app.MapGet("clientes/{id}/extrato", async Task<IResult> (int id) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    return await ClicBankService.GetExtrato(id, connStr);
});

app.MapPut("reset", async Task () => await ClicBankService.Reset(connStr));

app.Run();
