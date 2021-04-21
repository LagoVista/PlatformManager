using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.IoT.Deployment.Admin.Models;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class AllActiveInstancesViewModel : ListViewModelBase<IoT.Deployment.Admin.Models.DeploymentInstanceSummary>
    {
        protected override string GetListURI()
        {
            return $"/sys/api/deployment/instances";
        }

        protected override async void ItemSelected(DeploymentInstanceSummary model)
        {
            await NavigateAndViewAsync<MonitorInstanceViewModel>(model.Id);
        }
    }
}
