using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using LagoVista.Core;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class MonitorInstanceViewModel : MonitoringViewModelBase
    {

        enum ResourceType
        {
            Host,
            Instance,
        }

        public override Task InitAsync()
        {
            PerformNetworkOperation(async () =>
            {
                var uri = $"/api/deployment/instance/{LaunchArgs.ChildId}";
                var response = await RestClient.GetAsync<DetailResponse<DeploymentInstance>>(uri);
                if (response.Successful)
                {
                    Instance = response.Result.Model;
                }
                else
                {
                    await ShowServerErrorMessageAsync(response.ToInvokeResult());
                    CloseScreen();
                }
            });

            return base.InitAsync();
        }

        DeploymentInstance _instance;
        public DeploymentInstance Instance
        {
            get { return _instance; }
            set { Set(ref _instance, value); }
        }

        public override string GetChannelURI()
        {
            return $"/api/wsuri/instance/{LaunchArgs.ChildId}/normal";
        }

        private void SendAction(ResourceType resourceType, string action, string message)
        {
            PerformNetworkOperation(async () =>
            {
                var resourceId = resourceType == ResourceType.Host ? Instance.Host.Id : Instance.Id;
                var uri = $"/api/deployment/{resourceType.ToString().ToLower()}/{resourceId}/{action}";
                Debug.WriteLine("Request Id: " + uri);

                var response = await RestClient.GetAsync(uri);
                if (response.Success)
                {
                    MessagesFromServer.Insert(0, new Notification()
                    {
                        Text = message,
                        DateStamp = DateTime.UtcNow.ToJSONString()
                    });
                    if (MessagesFromServer.Count > 100) MessagesFromServer.RemoveAt(100);
                }
                else
                {
                    await ShowServerErrorMessageAsync(response.ToInvokeResult());
                    MessagesFromServer.Insert(0, new Notification()
                    {
                        Text = response.ErrorMessage,
                        DateStamp = DateTime.UtcNow.ToJSONString()
                    });
                    if (MessagesFromServer.Count > 100) MessagesFromServer.RemoveAt(100);

                }
            });
        }

        public override void HandleMessage(Notification notification)
        {
            if (!String.IsNullOrEmpty(notification.PayloadType))
            {
                Debug.WriteLine("----");
                Debug.WriteLine(notification.PayloadType);
                Debug.WriteLine(notification.Payload);
                Debug.WriteLine("----");
            }
            else
            {
                if (!String.IsNullOrEmpty(notification.Text))
                {
                    DispatcherServices.Invoke(() =>
                    {
                        MessagesFromServer.Insert(0, notification);
                        if (MessagesFromServer.Count > 100)
                        {
                            MessagesFromServer.RemoveAt(100);
                        }
                    });
                }
                Debug.WriteLine(notification.Text);
            }
        }

        public MonitorInstanceViewModel()
        {
            DeployHostCommand = new RelayCommand(() => SendAction(ResourceType.Instance, "deployhost", Resources.PlatformManagerResources.ServerAction_SentDeploy));
            RemoveServerCommand = new RelayCommand(() => SendAction(ResourceType.Instance, "destroyhost", Resources.PlatformManagerResources.ServerAction_SentRemove));
            RestartContainer = new RelayCommand(() => SendAction(ResourceType.Instance, "resetartcontainer", Resources.PlatformManagerResources.ServerAction_SentResetContainer));
            RestartHost = new RelayCommand(() => SendAction(ResourceType.Instance, "restarthost", Resources.PlatformManagerResources.ServerAction_SentReset));
            StartApplication = new RelayCommand(() => SendAction(ResourceType.Instance, "start", Resources.PlatformManagerResources.ServerAction_SentStart));
            PauseApplication = new RelayCommand(() => SendAction(ResourceType.Instance, "pause", Resources.PlatformManagerResources.ServerAction_SentPause));
            StopApplication = new RelayCommand(() => SendAction(ResourceType.Instance, "stop", Resources.PlatformManagerResources.ServerAction_SentStop));
            ReloadSolution = new RelayCommand(() => SendAction(ResourceType.Instance, "reloadsolution", Resources.PlatformManagerResources.ServerAction_SentReloadSolution));
            UpdateRuntime = new RelayCommand(() => SendAction(ResourceType.Instance, "updateruntime", Resources.PlatformManagerResources.ServerAction_SentUpdateRuntime));

            CanRemoveServer = true;
            CanResetartServer = true;
            CanRestartContainer = true;
            CanUpdateRuntime = true;
            CanDeployHost = true;
            CanStopApplication = true;
            CanPauseApplication = true;
            CanStartApplication = true;
            CanReloadSolution = true;
        }

        public RelayCommand DeployHostCommand { get; private set; }
        public RelayCommand RemoveServerCommand { get; private set; }
        public RelayCommand RestartContainer { get; private set; }
        public RelayCommand RestartHost { get; private set; }
        public RelayCommand StartApplication { get; private set; }
        public RelayCommand PauseApplication { get; private set; }
        public RelayCommand StopApplication { get; private set; }
        public RelayCommand ReloadSolution { get; private set; }
        public RelayCommand UpdateRuntime { get; private set; }

        public RelayCommand ShowHostTelemetry { get; private set; }
        public RelayCommand ShowInstanceTelemetry { get; private set; }

        public ObservableCollection<Notification> MessagesFromServer { get; private set; } = new ObservableCollection<Notification>();

        private bool _canRemoveServer;
        public bool CanRemoveServer
        {
            get { return _canRemoveServer; }
            set { Set(ref _canRemoveServer, value); }
        }

        private bool _canRestartServer;
        public bool CanResetartServer
        {
            get { return _canRestartServer; }
            set { Set(ref _canRestartServer, value); }
        }

        private bool _canRestartContainer;
        public bool CanRestartContainer
        {
            get { return _canRestartContainer; }
            set { Set(ref _canRestartContainer, value); }
        }

        private bool _canUpdateRuntime;
        public bool CanUpdateRuntime
        {
            get { return _canUpdateRuntime; }
            set { Set(ref _canUpdateRuntime, value); }
        }

        private bool _canDeployHost;
        public bool CanDeployHost
        {
            get { return _canDeployHost; }
            set { Set(ref _canDeployHost, value); }
        }

        private bool _canStopApplication;
        public bool CanStopApplication
        {
            get { return _canStopApplication; }
            set { Set(ref _canStopApplication, value); }
        }

        private bool _canPauseApplication;
        public bool CanPauseApplication
        {
            get { return _canPauseApplication; }
            set { Set(ref _canPauseApplication, value); }
        }

        private bool _canStartApplication;
        public bool CanStartApplication
        {
            get { return _canStartApplication; }
            set { Set(ref _canStartApplication, value); }
        }

        private bool _canReloadSolution;
        public bool CanReloadSolution
        {
            get { return _canReloadSolution; }
            set { Set(ref _canReloadSolution, value); }
        }


    }
}
