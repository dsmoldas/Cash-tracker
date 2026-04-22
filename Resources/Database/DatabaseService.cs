using SQLite;

namespace Cash_Tracker.Resources.Database
{
    public class DatabaseService
    {
        SQLiteAsyncConnection DBConection;

        async Task Init()
        {
            if (DBConection != null)
            {
                return;
            }
            else
            {
                //vytvoření cesty souboru databáze
                var path = Path.Combine(FileSystem.AppDataDirectory, "datazaznamu.db3");
                DBConection = new SQLiteAsyncConnection(path);

                await DBConection.CreateTableAsync<DataDB>();
            }
        }

        /// <summary>
        /// uložíní záznamu
        /// </summary>
        /// <param name="castka"></param>
        /// <param name="kategorie"></param>
        /// <param name="datum"></param>
        /// <param name="prijem"></param>
        /// <returns></returns>
        public async Task UlozZaznam(int castka, string kategorie, DateTime datum, bool prijem)
        {
            await Init();

            var novyZaznam = new DataDB
            {
                Castka = castka,
                Kategorie = kategorie,
                Datum = datum,
                Prijem = prijem
            };

            await DBConection.InsertAsync(novyZaznam);
        }

        /// <summary>
        /// výpis z databáze
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataDB>> VypisVse()
        {
            await Init();

            return await DBConection.Table<DataDB>().ToListAsync();
        }

        /// <summary>
        /// smazání položky z databáze podle ID
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SmazPodleID(DataDB data)
        {
            await Init();

            await DBConection.DeleteAsync(data);
        }
    }
}