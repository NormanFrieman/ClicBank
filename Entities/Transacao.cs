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

        [MaxLength(10)]
        public string Descricao { get; set; } = null!;
        public DateTime Data { get; set; }

        public Transacao() { }

        public Transacao(int clienteId, int valor, char tipo, string descricao)
        {
            ClienteId = clienteId;
            Valor = valor;
            Tipo = tipo;
            Descricao = descricao;
        }
    }
}
