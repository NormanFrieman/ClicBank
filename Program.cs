using ClicBank.Infra;
using ClicBank.Interfaces;
using ClicBank.Services;
using ClicBank.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddScoped<IClicBankService, ClicBankService>()
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

app.MapPost("/clientes/{id}/transacoes", async Task<IResult>(string id, TransacaoDto transacaoDto, IClicBankService service) =>
{
    if (!int.TryParse(id, out int clienteId))
        return Results.UnprocessableEntity();

    if (clienteId < 1 || clienteId > 5)
        return Results.NotFound();

    if (String.IsNullOrEmpty(transacaoDto.descricao) || transacaoDto.descricao.Length > 10)
        return Results.UnprocessableEntity();

    return await service.AddTransacao(clienteId, transacaoDto);
});

app.MapGet("clientes/{id}/extrato", async Task<IResult> (int id, IClicBankService service) =>
{
    if (id < 1 || id > 5)
        return Results.NotFound();

    return await service.GetExtrato(id);
});

app.Run();
