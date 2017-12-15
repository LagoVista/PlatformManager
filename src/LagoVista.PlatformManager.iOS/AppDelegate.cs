using Foundation;
using System;
using UIKit;

namespace LagoVista.PlatformManager.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // class-level declarations

        private const string MOBILE_CENTER_KEY = "e3d48293-22fe-4bfb-9a7d-9fa41112c23f";

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            LagoVista.XPlat.iOS.Startup.Init(app, MOBILE_CENTER_KEY);

            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            app.StatusBarHidden = false;

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            App.Instance.HandleURIActivation(new Uri(url.AbsoluteString));
            return true;
        }


    }
}


