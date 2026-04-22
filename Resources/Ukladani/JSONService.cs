using System.Text.Json;

namespace Cash_Tracker.Resources.Ukladani
{
    public class JSONService
    {
        //cesty souborů
        string pathSeznam = Path.Combine(FileSystem.AppDataDirectory, "kategorie.json");
        string pathMena = Path.Combine(FileSystem.AppDataDirectory, "mena.json");

        /// <summary>
        /// uložní záznamu "kategorie"
        /// </summary>
        /// <param name="kategorie"></param>
        /// <returns></returns>
        public async Task UlozSeznam(List<string> kategorie)
        {
            try
            {
                string json = JsonSerializer.Serialize(kategorie);

                await File.WriteAllTextAsync(pathSeznam, json);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyba", "Nepodařilo se uložit", "OK");
            }
        }

        /// <summary>
        /// výpis "kategorií"
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> NactiSeznam()
        {
            if (!File.Exists(pathSeznam))
            {
                return new List<string>();
            }

            try
            {
                string json = await File.ReadAllTextAsync(pathSeznam);

                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyba", $"Nepodařilo se najít {ex.Message}", "OK");
                return new List<string>();
            }
        }

        /// <summary>
        /// uložení záznamu "měna"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task UlozString(string str)
        {
            try
            {
                string json = JsonSerializer.Serialize(str);

                await File.WriteAllTextAsync(pathMena, json);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyba", "Nepodařilo se uložit", "OK");
            }

        }

        /// <summary>
        /// výpis uložené měny
        /// </summary>
        /// <returns></returns>
        public async Task<string> NactiString()
        {
            if (!File.Exists(pathMena))
            {
                return new string("");
            }

            try
            {
                string json = await File.ReadAllTextAsync(pathMena);

                return JsonSerializer.Deserialize<string>(json) ?? new string("");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Chyby", $"Nepodařilo se najít {ex.Message}", "Ok");
                return new string("");
            }
        }
    }
}