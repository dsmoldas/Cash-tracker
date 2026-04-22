using Cash_Tracker.Resources.Database;
using Cash_Tracker.Resources.Ukladani;

namespace Cash_Tracker.Resources.Pages;

public partial class AddPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly JSONService _jsonService;
    public List<string> kategorie = new List<string>();
    bool prijem = true;

    /// <summary>
    /// initializace prvků po prvním spuštění
    /// </summary>
    /// <param name="databaseService"></param>
    /// <param name="jSONService"></param>
    public AddPage(DatabaseService databaseService, JSONService jSONService)
    {
        InitializeComponent();

        _databaseService = databaseService;
        _jsonService = jSONService;
    }

    /// <summary>
    /// aktualizace dat při návratu
    /// </summary>
    protected override async void OnAppearing()
    {
        Shimmer.IsActive = true;
        Obsah.IsVisible = false;
        base.OnAppearing();

        kategorie = await _jsonService.NactiSeznam();
        MaxDatum();
        PickerOptions();

        await Task.Delay(1000);
        Obsah.IsVisible = true;
        Shimmer.IsActive = false;
    }

    /// <summary>
    /// resetování zadaných hodnot
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        ECastka.Text = string.Empty;
        PiKategorie.SelectedIndex -= 1;
        DataPiDatum.Date = DateTime.Now;
    }

    /// <summary>
    /// omezení aby nešlo zadat datum, ketré ještě nebylo
    /// </summary>
    private void MaxDatum()
    {
        DateTime maxDatum = DateTime.Now;

        DataPiDatum.MaximumDate = maxDatum;
    }

    /// <summary>
    /// výběr mezi příjmy a výdaji
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnPrijmy_Clicked(object sender, EventArgs e)
    {
        BtnPrijmy.BackgroundColor = Color.FromHex("#ac99ea");
        BtnVydaje.BackgroundColor = Color.FromHex("#8B009C");

        BtnPrijmy.TextColor = Color.FromRgb(0, 0, 0);
        BtnVydaje.TextColor = Color.FromRgb(255, 255, 255);

        prijem = true;
    }

    /// <summary>
    /// výběr mezi příjmy a výdaji
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnVydaje_Clicked(object sender, EventArgs e)
    {
        BtnVydaje.BackgroundColor = Color.FromHex("#ac99ea");
        BtnPrijmy.BackgroundColor = Color.FromHex("#8B009C");

        BtnVydaje.TextColor = Color.FromRgb(0, 0, 0);
        BtnPrijmy.TextColor = Color.FromRgb(255, 255, 255);

        prijem = false;
    }

    /// <summary>
    /// přidání možností, které si může uživatel vybírat
    /// </summary>
    private void PickerOptions()
    {
        PiKategorie.Items.Clear();
        kategorie.Sort();
        foreach (string x in kategorie)
        {
            PiKategorie.Items.Add(x);
        }
    }

    /// <summary>
    /// uložení záznamu do databáze
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (ECastka.Text.Length == 0 || PiKategorie.SelectedItem == null)
        {
            await DisplayAlert("Chyba", "Zadejte všechny parametry", "OK");
            return;
        }
        else
        {
            int castka = int.Parse(ECastka.Text);
            DateTime datum = DataPiDatum.Date;

            await _databaseService.UlozZaznam(castka, PiKategorie.SelectedItem.ToString(), datum, prijem);

            await DisplayAlert("Hotovo", "Byl přidán nový záznam", "OK");

            //resetování zadaných hodnot
            ECastka.Text = string.Empty;
            PiKategorie.SelectedIndex = -1;
            DataPiDatum.Date = DateTime.Now;
        }
    }
}