using Android.App;
using Android.OS;
using Android.Content.PM;
using static LagoVista.PlatformManager.Droid.Resource;

namespace LagoVista.PlatformManager.Droid
{
    [Activity(Label = "Platform Manager", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string MOBILE_CENTER_KEY = "1276e0bd-4c34-4b88-81d1-61c999e0dd7e";

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Layout.Tabbar;
            ToolbarResource = Layout.Toolbar;

            //https://play.google.com/apps/publish/?dev_acc=12258406958683843289
            LagoVista.XPlat.Droid.Startup.Init(BaseContext, MOBILE_CENTER_KEY);

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

