namespace ClicBank.ViewModels
{
    public record struct TransacaoDto(object valor, char tipo, string descricao) { }

    public record struct TransacaoHistorico(object valor, char tipo, string descricao, DateTime realizada_em) { }
}
