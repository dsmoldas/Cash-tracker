using CommunityToolkit.Mvvm.ComponentModel;

namespace Cash_Tracker.Resources.Moduls
{
    public partial class MojePoložka : ObservableObject
    {
        public string jmeno { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}
