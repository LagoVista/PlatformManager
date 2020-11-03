using LagoVista.PlatformManager.Core.ViewModels;
using LagoVista.XPlat.Core;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace LagoVista.PlatformManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstanceSSHView : LagoVistaContentPage
    {
        public InstanceSSHView()
        {
            InitializeComponent();
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var vm = BindingContext as InstanceSSHViewModel;
            if (vm != null)
            {
                vm.ConsoleResponses.CollectionChanged += ConsoleResponses_CollectionChanged;
            }
        }

        private void ConsoleResponses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var vm = BindingContext as InstanceSSHViewModel;
            var lastItem = vm.ConsoleResponses.LastOrDefault();
            if (lastItem != null)
            {
                SSHResponses.ScrollTo(lastItem, Xamarin.Forms.ScrollToPosition.MakeVisible, true);
            }
        }
    }
}