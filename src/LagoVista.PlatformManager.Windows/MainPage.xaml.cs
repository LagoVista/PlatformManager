using System.Reflection;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LagoVista.PlatformManager.Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var version = typeof(App).GetTypeInfo().Assembly.GetName().Version;

            var versionInfo = new LagoVista.Core.Models.VersionInfo()
            {
                Major = version.Major,
                Minor = version.Minor,
                Revision = version.Revision,
                Build = version.Build,
            };

            var app = new LagoVista.PlatformManager.App();
            app.SetVersionInfo(versionInfo);
            LoadApplication(app);

        }
    }
}
