using ClicBank.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ClicBank.Entities
{
    public class Transacao
    {
        [Key]
        public Guid Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int Valor { get; set; }
        public char Tipo { get; set; }
        public string Descricao { get; set; } = null!;
        public DateTime Data { get; set; }

        public Transacao() { }

        public Transacao(Cliente cliente, TransacaoDto transacao)
        {
            ClienteId = cliente.Id;
            Cliente = cliente;
            Valor = transacao.valor;
            Tipo = transacao.tipo;
            Descricao = transacao.descricao;
            Data = DateTime.UtcNow;
        }
    }
}
