using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.Repository;
using ClicBank.ViewModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddScoped<IClienteRepository, ClienteRepository>()
    .AddScoped<ITransacaoRepository, TransacaoRepository>()
    .AddScoped<ISeed, Seed>()
    .AddDbContext<Context>(opt => opt.UseInMemoryDatabase("test"));

var app = builder.Build();

app.MapGet("/api", async (ISeed seed) =>
{
    await seed.IncluirClientes();
    return "opa bom dia";
});

app.MapPost("/clientes/{id:int}/transacoes", async Task<IResult>(int id, TransacaoDto transacaoDto, IClienteRepository clienteRepository, ITransacaoRepository transacaoRepository) =>
{
    var cliente = await clienteRepository.Get().SingleOrDefaultAsync(x => x.Id == id);
    if (cliente == null)
        return Results.NotFound();

    if ("c".Equals(transacaoDto.tipo))
        cliente.Limite -= transacaoDto.valor;
    else
        cliente.Saldo -= transacaoDto.valor;

    if (cliente.Saldo + cliente.Limite < 0)
        return Results.UnprocessableEntity();

    await clienteRepository.Update(cliente);
    await transacaoRepository.Add(new Transacao(cliente, transacaoDto));

    return Results.Ok(new SaldoResumo(cliente));
});

app.MapGet("clientes/{id}/extrato", async Task<IResult> (int id, IClienteRepository clienteRepository, ITransacaoRepository transacaoRepository) =>
{
    var cliente = await clienteRepository.Get().Include(x => x.Transacoes).SingleOrDefaultAsync(x => x.Id == id);
    if (cliente == null)
        return Results.NotFound();

    return Results.Ok(new ExtratoDto(cliente));
});

app.Run();
