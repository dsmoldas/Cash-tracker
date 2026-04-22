using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace Cash_Tracker.Resources.Database
{
    /// <summary>
    /// šablona pro table v databází
    /// </summary>
    public partial class DataDB : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Castka { get; set; }
        public string Kategorie { get; set; }
        public DateTime Datum { get; set; }
        public bool Prijem { get; set; }

        [ObservableProperty]
        [property: Ignore]
        private bool isSelected;

        [Ignore]
        public int CastkaSeZnamenkem => Prijem ? Castka : -Castka;

        [Ignore]
        public string Mena { get; set; }

        [Ignore]
        public string CastkaSMenou => $"{Castka:N0} {Mena}";
    }
}
