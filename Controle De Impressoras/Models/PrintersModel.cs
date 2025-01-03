﻿using Controle_De_Impressoras.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Controle_De_Impressoras.Models
{
    [Table("PrinterMonitoring")]
    public class PrintersModel
    {
        public int Id { get; set; }
        public string Ip { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório")]
        public string DeviceManufacturer { get; set; }
        public int Ativa { get; set; }


        [Required(ErrorMessage = "O nome do dispositivo é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O nome do dispositivo deve ter no mínimo 5 caracteres")]
        public string DeviceModel { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O modelo deve ter no mínimo 5 caracteres")]
        public string PrinterModel { get; set; }

        [Required(ErrorMessage = "O serial é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial deve ter no mínimo 5 caracteres")]
        public string SerialImpressora { get; set; }

        [Required(ErrorMessage = "O serial da unidade de imagem é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial da unidade de imagem deve ter no mínimo 5 caracteres")]
        public string SerialUniImage { get; set; }

        [Required(ErrorMessage = "A quantidade de impressão total é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número válido")]
        public int? QuantidadeImpressaoTotal { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBlack { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemCyan { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemYellow { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemMagenta { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemFusor { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBelt { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemKitManutencao { get; set; }

        [Required(ErrorMessage = "O status da impressora é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(3, ErrorMessage = "O status da impressora deve ter no mínimo 3 caracteres")]
        public string PrinterStatus { get; set; }

        [Required(ErrorMessage = "O tipo da impressora é obrigatório")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O patrimônio é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O patrimônio deve ser um número válido com no mínimo 1 dígitos")]
        public int? Patrimonio { get; set; }

        [Required(ErrorMessage = "A data e hora de busca são obrigatórias")]
        [FutureDate(ErrorMessage = "A data e hora não podem estar no futuro.")]
        public DateTime DataHoraDeBusca { get; set; }

        // Novos campos
        [Required(ErrorMessage = "A secretaria é obrigatória")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        public string Secretaria { get; set; }

        [Required(ErrorMessage = "A abreviação da secretaria é obrigatória")]
        [MaxLength(20, ErrorMessage = "O comprimento máximo permitido é 20 caracteres.")]
        public string AbrSecretaria { get; set; }

        [Required(ErrorMessage = "O departamento é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        public string Depto { get; set; }
        public string SerialTonnerPreto { get; set; }
        public int? PorcentagemUnidadeImagem { get; set; }
        public int? InstituicaoId { get; set; }
        public string Localizacao { get; set; }


        public static List<PrintersModel> RecuperarImpressoras(string tipo = null, string marca = null, string modelo = null, int patrimonio = 0)
        {
            using (var context = new PrintersContext())
            {
                var query = from pm in context.Printers
                            join psl in context.PrinterStatusLogs
                            on pm.Id equals psl.PrinterId
                            where psl.DataHoraDeBusca == context.PrinterStatusLogs
                                    .Where(psl2 => psl2.PrinterId == pm.Id)
                                    .Max(psl2 => psl2.DataHoraDeBusca)
                            select new
                            {
                                Printer = pm,
                                Status = psl
                            };

                if (!string.IsNullOrEmpty(tipo))
                    query = query.Where(q => q.Printer.Tipo == tipo);

                if (!string.IsNullOrEmpty(marca))
                    query = query.Where(q => q.Printer.DeviceManufacturer.Contains(marca));

                if (!string.IsNullOrEmpty(modelo))
                    query = query.Where(q => q.Printer.PrinterModel.Contains(modelo));

                if (patrimonio > 0)
                    query = query.Where(q => q.Printer.Patrimonio == patrimonio);

                var updatedPrinters = query.ToList().Select(q =>
                {
                    var printer = q.Printer;

                    printer.QuantidadeImpressaoTotal = q.Status.QuantidadeImpressaoTotal;
                    printer.PorcentagemBlack = q.Status.PorcentagemBlack;
                    printer.PorcentagemCyan = q.Status.PorcentagemCyan;
                    printer.PorcentagemYellow = q.Status.PorcentagemYellow;
                    printer.PorcentagemMagenta = q.Status.PorcentagemMagenta;
                    printer.PorcentagemFusor = q.Status.PorcentagemFusor;
                    printer.PorcentagemBelt = q.Status.PorcentagemBelt;
                    printer.PorcentagemKitManutencao = q.Status.PorcentagemKitManutencao;
                    printer.DataHoraDeBusca = q.Status.DataHoraDeBusca;
                    printer.SerialTonnerPreto = q.Status.SerialTonnerPreto;


                    return printer;
                }).ToList();

                return updatedPrinters;
            }
        }


        public static List<ErroImpressora> RecuperarErrosImpressoras()
        {
            using (var context = new PrintersContext())  // Usando o contexto de banco de dados
            {
                var query = context.ErrosImpressoras
                                   .Select(ei => new ErroImpressora
                                   {
                                       Id = ei.Id,
                                       InstituicaoId = ei.InstituicaoId,
                                       Marca = ei.Marca,
                                       Modelo = ei.Modelo,
                                       Ip = ei.Ip,
                                       Patrimonio = ei.Patrimonio,
                                       Secretaria = ei.Secretaria,
                                       AbrSecretaria = ei.AbrSecretaria,
                                       Depto = ei.Depto,
                                       Localizacao = ei.Localizacao,
                                       Motivo = ei.Motivo
                                   })
                                   .ToList(); // Recupera os dados da tabela e converte para lista

                return query;
            }
        }
        public static List<DadosRelatorioModel> DadosRelatorio(DateTime? dataInicio, DateTime? dataFim, string secretaria, string depto, int? instituicaoId)
        {
            using (var context = new PrintersContext())
            {
                // Se dataInicio ou dataFim forem nulos, consideramos todas as datas
                if (!dataInicio.HasValue) dataInicio = DateTime.MinValue;
                if (!dataFim.HasValue) dataFim = DateTime.MaxValue;

                // Consulta para filtrar os dados com base nas datas e nos novos filtros
                var query = from psl in context.PrinterStatusLogs
                            join pm in context.Printers
                            on psl.PrinterId equals pm.Id
                            where psl.DataHoraDeBusca >= dataInicio && psl.DataHoraDeBusca <= dataFim
                            // Filtros adicionais
                            && (string.IsNullOrEmpty(secretaria) || pm.Secretaria.Contains(secretaria))
                            && (string.IsNullOrEmpty(depto) || pm.Secretaria.Contains(depto)) // Assumindo que "Depto" seja em "Secretaria"
                            && (!instituicaoId.HasValue || pm.InstituicaoId == instituicaoId)
                            orderby psl.DataHoraDeBusca descending
                            select new DadosRelatorioModel
                            {
                                DataHoraDeBusca = psl.DataHoraDeBusca,
                                Patrimonio = pm.Patrimonio,
                                Modelo = pm.DeviceModel,
                                Secretaria = pm.AbrSecretaria,
                                Depto = pm.Depto, // Aqui você pode ajustar se a propriedade de "Depto" for diferente.
                                InstituicaoId = pm.InstituicaoId,
                                ColorMono = pm.Tipo,
                                ImpressoesTotais = psl.QuantidadeImpressaoTotal,
                                PorcentagemBlack = psl.PorcentagemBlack,
                                PorcentagemCyan = psl.PorcentagemCyan,
                                PorcentagemYellow = psl.PorcentagemYellow,
                                PorcentagemMagenta = psl.PorcentagemMagenta,
                                UnidadeImagem = psl.PorcentagemUnidadeImagem,
                                Fusor = psl.PorcentagemFusor,
                                Belt = psl.PorcentagemBelt,
                                KitManutencao = psl.PorcentagemKitManutencao
                            };

                // Retorna a lista filtrada
                return query.ToList();
            }
        }






    }
}
