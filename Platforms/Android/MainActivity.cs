using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Cash_Tracker
{
    [Activity(Theme = "@style/MainTheme", Label = "", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            SupportRequestWindowFeature((int)Android.Views.WindowFeatures.NoTitle);

            base.OnCreate(savedInstanceState);
        }
    }
}
