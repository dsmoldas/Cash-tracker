using Cash_Tracker.Resources.Database;
using Cash_Tracker.Resources.Ukladani;

namespace Cash_Tracker.Resources.Pages;

public partial class DataPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly JSONService _jsonService;

    private DataDB vybranaPolozka;

    public string mena = "Kč";

    /// <summary>
    /// initializace prvků po prvním spuštěním
    /// </summary>
    /// <param name="databaseService"></param>
    /// <param name="jSONService"></param>
    public DataPage(DatabaseService databaseService, JSONService jSONService)
    {
        InitializeComponent();

        _databaseService = databaseService;
        _jsonService = jSONService;
        btnOdeber.IsVisible = false;
    }

    /// <summary>
    /// aktualizace hodnot při návratu
    /// </summary>
    protected override async void OnAppearing()
    {
        shimer.IsActive = true;
        base.OnAppearing();

        await currentMena();
        await AktualizujCV();

        await Task.Delay(1500);
        shimer.IsActive = false;
        Obsah.IsVisible = true;
    }

    /// <summary>
    /// aktualizace collection view
    /// </summary>
    /// <returns></returns>
    private async Task AktualizujCV()
    {
        var list = await _databaseService.VypisVse();

        var serazenySeznam = list.OrderByDescending(x => x.Datum).ToList();

        if (serazenySeznam.Count == 0)
        {
            btnOdeber.IsVisible = false;
        }

        foreach (var polozka in serazenySeznam)
        {
            polozka.Mena = mena;
        }

        CVHistorie.ItemsSource = serazenySeznam;
    }

    /// <summary>
    /// načtení aktuální měny
    /// </summary>
    /// <returns></returns>
    private async Task currentMena()
    {
        mena = await _jsonService.NactiString();

        if (mena.Length == 0)
        {
            mena = "Kč";
        }
    }

    /// <summary>
    /// výběr položky v collection view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var border = (Border)sender;
        var polozka = (DataDB)border.BindingContext;

        if (polozka == null)
        {
            btnOdeber.IsVisible = false;
            return;
        }

        if (polozka.IsSelected)
        {
            polozka.IsSelected = false;
            btnOdeber.IsVisible = false;
        }
        else
        {
            var seznam = CVHistorie.ItemsSource as IEnumerable<DataDB>;

            if (seznam != null)
            {
                foreach (var item in seznam)
                {
                    item.IsSelected = false;
                }
            }

            vybranaPolozka = polozka;
            btnOdeber.IsVisible = true;
            polozka.IsSelected = true;
        }
    }

    /// <summary>
    /// smazání vybrané položky z databáze
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnOdeber_Clicked(object sender, EventArgs e)
    {
        await _databaseService.SmazPodleID(vybranaPolozka);

        await DisplayAlert("Smazáno", $"Byla smazána položka:\n{vybranaPolozka.Kategorie}\n{vybranaPolozka.Castka}\n{vybranaPolozka.Datum.ToString("d")}", "OK");
        await AktualizujCV();
        btnOdeber.IsVisible = false;
    }
}