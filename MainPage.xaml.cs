using Cash_Tracker.Resources.Database;
using Cash_Tracker.Resources.Ukladani;
using Cash_Tracker.Resources.ViewModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace Cash_Tracker
{
    public partial class MainPage : ContentPage
    {
        public List<PieChartData> PieDataPrijmy { get; set; } = new List<PieChartData>();
        public List<PieChartData> PieChartDataVydaje { get; set; } = new List<PieChartData>();
        public List<LineChartData> LineChartData { get; set; } = new List<LineChartData>();

        public TimeSpan TimeSpanDay => TimeSpan.FromDays(1);
        public Func<DateTime, string> Formatter { get; set; } = datum => datum.ToString("d");

        private readonly JSONService _jsonService;
        private readonly DatabaseService _databaseService;

        private int currentCash;
        private List<string> kategorie;
        private string mena = "Kč";

        /// <summary>
        /// Initializace prvků při prvním spuštěním
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="jSONService"></param>
        public MainPage(DatabaseService databaseService, JSONService jSONService)
        {
            _databaseService = databaseService;
            _jsonService = jSONService;

            InitializeComponent();

            BindingContext = this;
        }

        /// <summary>
        /// aktualizace při návratu
        /// </summary>
        protected override async void OnAppearing()
        {
            Shimmer.IsActive = true;
            Obsah.IsVisible = false;
            base.OnAppearing();

            await LineChartXAML();
            await AktualniPenize();
            await AktualniMena();

            kategorie = await _jsonService.NactiSeznam();

            LblZustatek.Text = $"Aktuální zůstatek: {currentCash} {mena}";

            await Task.Delay(1000); // dealay pro načítání
            Shimmer.IsActive = false;
            Obsah.IsVisible = true;

            await PieChartPrijmyXAML(); //aktualizace dat
            await PieCharVydajePXAML(); //aktualizace dat
        }

        /// <summary>
        /// výpis aktuálního zůstatku
        /// </summary>
        /// <returns></returns>
        private async Task AktualniPenize()
        {
            int prijmy = 0;
            int vydaje = 0;

            var zaznamy = await _databaseService.VypisVse();

            //načtení z databáze
            var udaje = zaznamy.Select(x => new
            {
                x.Castka,
                x.Prijem
            }).ToList();

            //rozdělení na příjmy a výdaje
            foreach (var x in udaje)
            {
                if (x.Prijem == true)
                {
                    prijmy += x.Castka;
                }
                else
                {
                    vydaje += x.Castka;
                }
            }

            currentCash = prijmy - vydaje;

            //přiřazení barev pro lepši přehlednost
            if (currentCash < 0)
            {
                LblZustatek.TextColor = Color.FromHex("#ff0000");
            }
            else if (currentCash > 0)
            {
                LblZustatek.TextColor = Color.FromHex("#2bff00");
            }
            else
            {
                LblZustatek.TextColor = Color.FromHex("#E1E1E1");
            }
        }

        /// <summary>
        /// přiřazení měny uložené v JSON
        /// </summary>
        /// <returns></returns>
        private async Task AktualniMena()
        {
            mena = await _jsonService.NactiString();

            //když měna není přiřazena
            if (mena.Length == 0)
            {
                mena = "Kč";
            }
        }

        /// <summary>
        /// dávání dat do koláčového grafu
        /// </summary>
        /// <returns></returns>
        private async Task PieChartPrijmyXAML()
        {
            PieDataPrijmy.Clear();

            var list = await _databaseService.VypisVse();
            var udaje = list.Where(x => x.Prijem == true).GroupBy(x => x.Kategorie).Select(y => new PieChartData(y.Key, y.Sum(polozka => polozka.Castka)));

            if (list == null || list.Count == 0)
            {
                PieChartPrijmy.IsVisible = false;
                return;
            }
            else
            {
                PieChartPrijmy.IsVisible = true;

                foreach (var item in udaje)
                {
                    PieDataPrijmy.Add(item);
                }
            }

            PieChartPrijmy.SeriesSource = PieDataPrijmy.ToArray();
        }

        /// <summary>
        /// dávání dat do koláčového grafu
        /// </summary>
        /// <returns></returns>
        private async Task PieCharVydajePXAML()
        {
            PieChartDataVydaje.Clear();

            var list = await _databaseService.VypisVse();
            var udaje = list.Where(x => x.Prijem == false).GroupBy(x => x.Kategorie).Select(y => new PieChartData(y.Key, y.Sum(polozka => polozka.Castka)));

            if (list == null || list.Count == 0)
            {
                PieChartVydaje.IsVisible = false;
                return;
            }
            else
            {
                PieChartVydaje.IsVisible = true;

                foreach (var item in udaje)
                {
                    PieChartDataVydaje.Add(item);
                }
            }

            PieChartVydaje.SeriesSource = PieChartDataVydaje.ToArray();
        }

        /// <summary>
        /// dávání dat do grafu
        /// </summary>
        /// <returns></returns>
        private async Task LineChartXAML()
        {
            var list = await _databaseService.VypisVse();
            var udaje = list.GroupBy(x => x.Datum.Date).Select(g => (g.Key, g.Sum(p => p.CastkaSeZnamenkem))).OrderBy(x => x.Key);

            if (list == null || list.Count == 0)
            {
                GrafCelkovaCastka.IsVisible = false;
                return;
            }

            GrafCelkovaCastka.IsVisible = true;

            var bodyProGraf = new List<DateTimePoint>();

            int celkovyZustatek = 0;

            foreach (var item in udaje)
            {
                celkovyZustatek += item.Item2;
                bodyProGraf.Add(new DateTimePoint(item.Key, celkovyZustatek));
            }

            var lineSeries = new LineSeries<DateTimePoint>
            {
                Values = bodyProGraf,
                Name = "Celkový zůstatek:",
                Fill = null,
                GeometrySize = 10,
            };

            GrafCelkovaCastka.Series = new ISeries[] { lineSeries };
        }

    }
}