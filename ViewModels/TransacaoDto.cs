using ClicBank.Entities;
using System.Text.Json.Serialization;

namespace ClicBank.ViewModels
{
    public class TransacaoDto
    {
        public object valor { get; set; }
        public char tipo { get; set; }
        public string descricao { get; set; }

        [JsonConstructor]
        public TransacaoDto(object valor, char tipo, string descricao)
        {
            this.valor = valor;
            this.tipo = tipo;
            this.descricao = descricao;
        }

        public TransacaoDto(Transacao transacao)
        {
            valor = transacao.Valor;
            tipo = transacao.Tipo;
            descricao = transacao.Descricao;
        }
    }

    public class TransacaoHistorico : TransacaoDto
    {
        public DateTime realizada_em { get; set; }

        public TransacaoHistorico(Transacao transacao) : base(transacao)
        {
            realizada_em = transacao.Data;
        }
    }
}
