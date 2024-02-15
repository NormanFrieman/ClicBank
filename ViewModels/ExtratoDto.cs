using ClicBank.Entities;

namespace ClicBank.ViewModels
{
    public class ExtratoDto(Cliente cliente)
    {
        public SaldoDto saldo { get; set; } = new SaldoDto(cliente);
        public IEnumerable<TransacaoHistorico> ultimas_transacoes { get; set; } = cliente.Transacoes.Select(transacao => new TransacaoHistorico(transacao));
    }

    public class SaldoDto(Cliente cliente)
    {
        public int total { get; set; } = cliente.Saldo;
        public DateTime data_extrato { get; set; } = DateTime.UtcNow;
        public int limite { get; set; } = cliente.Limite;
    }

    public class SaldoResumo(Cliente cliente)
    {
        public int saldo { get; set; } = cliente.Saldo;
        public int limite { get; set; } = cliente.Limite;
    }
}
