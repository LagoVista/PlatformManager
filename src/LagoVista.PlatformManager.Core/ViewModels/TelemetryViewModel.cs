using LagoVista.Client.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class TelemetryViewModel : AppViewModelBase
    {

        public string ViewType { get; set; }
        public string Id { get; set; }

        public void ShowStatusHistory()
        {
            var statusHistoryUri = $"/api/deployment/{ViewType}/{Id}/statushistory";
        }

        public void ShowCustomEvents()
        {
            var telemetry = $"/api/telemetry/customEvents/{ViewType}/{Id}";
        }


        public void ShowExceptions()
        {
            var telemetry = $"/api/telemetry/error/{ViewType}/{Id}";
        }

        public ObservableCollection<LagoVista.IoT.Deployment.Admin.Models.TelemetryReportData> ReportData {get; set;}
    }
}
