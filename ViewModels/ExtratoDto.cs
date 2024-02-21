namespace ClicBank.ViewModels
{
    public record struct ExtratoDto(SaldoDto saldo, IEnumerable<TransacaoHistorico> ultimas_transacoes) { }

    public record struct SaldoDto
    {
        public SaldoDto() { }

        public int total { get; set; }
        public int limite { get; set; }
        public DateTime data_extrato { get; } = DateTime.UtcNow;
    }

    public record struct SaldoResumo(int saldo, int limite) { }
}
