using System.ComponentModel.DataAnnotations;

namespace ClicBank.Entities
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public int Limite { get; set; }
        public int Saldo { get; set; }

        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        public Cliente() { }
    }
}
