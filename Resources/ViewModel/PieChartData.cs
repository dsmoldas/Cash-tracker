namespace Cash_Tracker.Resources.ViewModel
{
    /// <summary>
    /// šablona pro koláčový graf
    /// </summary>
    /// <param name="kategorie"></param>
    /// <param name="castka"></param>
    public class PieChartData(string kategorie, double castka)
    {
        public string Kategorie { get; set; } = kategorie;
        public double[] Castky { get; set; } = [castka];
    }
}
