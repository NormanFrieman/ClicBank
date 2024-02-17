using ClicBank.Entities;
using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.Repository;
using ClicBank.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddScoped<IClienteRepository, ClienteRepository>()
    .AddScoped<ITransacaoRepository, TransacaoRepository>()
    .AddScoped<ISeed, Seed>()
    .AddDbContext<Context>(opt =>
    {
        opt.UseInMemoryDatabase("test");
        opt.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    });

var app = builder.Build();

app.UseExceptionHandler(x => x.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    if (exception is not null)
        context.Response.StatusCode = 422;
}));

app.MapGet("/api", async (ISeed seed) =>
{
    await seed.IncluirClientes();
    return "opa bom dia";
});

app.MapPost("/clientes/{id}/transacoes", async Task<IResult>(string id, TransacaoDto transacaoDto, IClienteRepository clienteRepository, ITransacaoRepository transacaoRepository) =>
{
    if (!int.TryParse(id, out int clienteId))
        return Results.UnprocessableEntity();

    if (clienteId < 1 || clienteId > 5)
        return Results.NotFound();

    if (String.IsNullOrEmpty(transacaoDto.descricao) || transacaoDto.descricao.Length > 10)
        return Results.UnprocessableEntity();

    var context = clienteRepository.GetContext();

    context.Database.BeginTransaction();

    var cliente = await context.Clientes.SingleOrDefaultAsync(x => x.Id == clienteId);

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

    context.Clientes.Update(cliente);
    context.Transacoes.Add(new Transacao(cliente, transacaoDto));

    context.Database.CommitTransaction();

    await context.SaveChangesAsync();
    
    return Results.Ok(new SaldoResumo(cliente));
});

app.MapGet("clientes/{id}/extrato", async Task<IResult> (int id, IClienteRepository clienteRepository, ITransacaoRepository transacaoRepository) =>
{
    var cliente = await clienteRepository
        .Get()
        .Include(x => x.Transacoes)
        .SingleOrDefaultAsync(x => x.Id == id);
    if (cliente == null)
        return Results.NotFound();

    return Results.Ok(new ExtratoDto(cliente));
});

app.Run();
