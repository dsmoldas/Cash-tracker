using Cash_Tracker.Resources.Database;
using Cash_Tracker.Resources.Ukladani;

namespace Cash_Tracker.Resources.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly JSONService _jsonService;

    List<string> Mena = new List<string>() { "$", "€", "¥", "Kč", "Ł" };
    public List<string> kategorie = new List<string>();

    /// <summary>
    /// initializace prvků po prvním spuštěním
    /// </summary>
    /// <param name="jSONService"></param>
    /// <param name="databaseService"></param>
    public SettingsPage(JSONService jSONService, DatabaseService databaseService)
    {
        InitializeComponent();

        _jsonService = jSONService;
        _databaseService = databaseService;
        pickerMenaOptions();
    }

    /// <summary>
    /// aktualizace dat po návratu
    /// </summary>
    protected override async void OnAppearing()
    {
        Shimmer.IsActive = true;
        Obsah.IsVisible = false;
        base.OnAppearing();

        kategorie = await _jsonService.NactiSeznam();
        await pickerKategorieOptions();

        await Task.Delay(750);
        Obsah.IsVisible = true;
        Shimmer.IsActive = false;
    }

    /// <summary>
    /// resetování zadaných hodnot
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PiMena.SelectedIndex = -1;
    }

    /// <summary>
    /// uložení vybrané měny
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BtnUlozMena_Clicked(object sender, EventArgs e)
    {
        if (PiMena.SelectedIndex == -1)
        {
            await DisplayAlert("Chyba", "Prosim zadejte měnu...", "OK");
        }
        else
        {
            await _jsonService.UlozString(PiMena.SelectedItem.ToString());
            PiMena.SelectedIndex = -1;
        }
    }

    /// <summary>
    /// přidání možností měn do prvku
    /// </summary>
    private void pickerMenaOptions()
    {
        foreach (string x in Mena)
        {
            PiMena.Items.Add(x);
        }
    }

    /// <summary>
    /// přidání možností kategorií do prvku
    /// </summary>
    /// <returns></returns>
    private async Task pickerKategorieOptions()
    {
        PiOdeberKategorii.Items.Clear();
        kategorie.Sort();
        foreach (string x in kategorie)
        {
            PiOdeberKategorii.Items.Add(x);
        }
    }

    /// <summary>
    /// uložení nové kategorie
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BtnUlozKategorii_Clicked(object sender, EventArgs e)
    {
        if (EKategorie.Text.Length == 0)
        {
            await DisplayAlert("Chyba", "Prosím zadejte nějakou kategorii", "OK");
            return;
        }
        else if (kategorie.Contains(EKategorie.Text) == true)
        {
            await DisplayAlert("Chyba", "Prosím zadejte kategorii, kterou tam ještě nemáte", "OK");
            return;
        }
        else
        {
            kategorie.Add(EKategorie.Text.ToLower());
            await _jsonService.UlozSeznam(kategorie);
        }
        EKategorie.Text = string.Empty;
    }

    /// <summary>
    /// vymazání dané kategorie
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BtnZrusitKategorii_Clicked(object sender, EventArgs e)
    {
        if (PiOdeberKategorii.SelectedIndex == -1)
        {
            await DisplayAlert("Chyba", "Prosím zvolte jakoukoliv aktegorii", "OK");
            return;
        }
        else
        {
            kategorie.Remove(PiOdeberKategorii.SelectedItem.ToString());
            await _jsonService.UlozSeznam(kategorie);
        }
        PiOdeberKategorii.SelectedIndex = -1;
    }
}