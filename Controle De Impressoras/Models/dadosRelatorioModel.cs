using Controle_De_Impressoras.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    public class DadosRelatorioModel
    {
        public DateTime DataHoraDeBusca { get; set; }
        public int? Patrimonio { get; set; }
        public string Modelo { get; set; }
        public string Secretaria { get; set; }
        public string Depto { get; set; }
        public string ColorMono { get; set; }
        public int? ImpressoesTotais { get; set; }
        public int? PorcentagemBlack { get; set; }
        public int? PorcentagemCyan { get; set; }
        public int? PorcentagemYellow { get; set; }
        public int? PorcentagemMagenta { get; set; }
        public int? UnidadeImagem { get; set; }
        public int? Fusor { get; set; }
        public int? Belt { get; set; }
        public int? KitManutencao { get; set; }
        public int? InstituicaoId { get; set; }
    }
}