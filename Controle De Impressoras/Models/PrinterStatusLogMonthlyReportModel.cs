public class PrinterStatusLogMonthlyReportModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalImpressaoMensal { get; set; }
    public decimal? DifferenceInImpressions { get; set; } // Diferen�a de impress�es do m�s atual para o anterior

    public decimal ComparisonWithPreviousMonth { get; set; } // Nova propriedade

}
