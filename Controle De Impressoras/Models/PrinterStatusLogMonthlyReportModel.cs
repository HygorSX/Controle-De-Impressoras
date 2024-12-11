public class PrinterStatusLogMonthlyReportModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalImpressaoMensal { get; set; }
    public decimal? DifferenceInImpressions { get; set; } // Diferença de impressões do mês atual para o anterior

    public decimal ComparisonWithPreviousMonth { get; set; } // Nova propriedade

}
