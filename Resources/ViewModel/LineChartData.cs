namespace Cash_Tracker.Resources.ViewModel
{
    /// <summary>
    /// šablona pro graf
    /// </summary>
    /// <param name="datum"></param>
    /// <param name="castka"></param>
    public class LineChartData(DateTime datum, int castka)
    {
        public DateTime Datum { get; set; } = datum;
        public int[] Castka { get; set; } = [castka];

        public Func<DateTime, string> Formatter { get; set; } = datum => datum.ToString("d");
    }
}
