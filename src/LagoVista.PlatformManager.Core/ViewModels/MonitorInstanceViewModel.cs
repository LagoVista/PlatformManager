using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class MonitorInstanceViewModel : MonitoringViewModelBase
    {


        public override string GetChannelURI()
        {
            return $"/api/wsuri/instance/{LaunchArgs.ChildId}/normal";
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
                if(!String.IsNullOrEmpty(notification.Text))
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

        public ObservableCollection<Notification> MessagesFromServer { get; private set; } = new ObservableCollection<Notification>();
    }
}
