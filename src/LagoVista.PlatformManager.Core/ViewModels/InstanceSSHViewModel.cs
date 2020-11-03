using LagoVista.Client.Core.ViewModels;
using LagoVista.Core.Commanding;
using Renci.SshNet;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.PlatformManager.Core.ViewModels
{
    public class SSHResponse
    {
        private string _line;
        public SSHResponse(string line)
        {
            _line = line;
        }

        public string Line => _line;
    }

    public class InstanceSSHViewModel : AppViewModelBase
    {
        public const string HOST_IP_ADDRESS = "HOST_IP_ADDRESS";

        private string _privateKey;

        public InstanceSSHViewModel()
        {
            ConnectCommand = new RelayCommand(Connect, CanConnect);
            DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
            ClearCredentialsCommand = new RelayCommand(ClearCredentials);
            SendSSHTextCommand = new RelayCommand(SendSSHText);
            ShowKeyCommand = new RelayCommand(ShowKey);
        }

    
        ShellStream _stream;

        void ReadFromStream(ShellStream stream)
        {
            Task.Run(() =>
            {
                try
                {
                    String line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        var response = new SSHResponse(line);
                        Debug.WriteLine(line);
                        DispatcherServices.Invoke(() =>
                        {
                            ConsoleResponses.Add(response);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            });
        }

        SshClient _client;

        public override async Task InitAsync()
        {
            if (!LaunchArgs.HasParam(HOST_IP_ADDRESS)) throw new Exception("Must pass in HOST_IP_ADDRESS as parameter.");
            var hostName = LaunchArgs.Parameters[HOST_IP_ADDRESS] as String;

            UserName = await Storage.GetKVPAsync<string>($"UserName-{hostName}");
            Password = await Storage.GetKVPAsync<string>($"Password-{hostName}");
            _privateKey = await Storage.GetKVPAsync<string>($"Key-{hostName}");

            ConnectCommand.RaiseCanExecuteChanged();

            await base.InitAsync();
        }

        public void SendSSHText()
        {
            if (_stream != null)
            {
                _stream.WriteLine(SSHText);
                SSHText = String.Empty;
            }
        }

        private static void Client_HostKeyReceived(object sender, Renci.SshNet.Common.HostKeyEventArgs e)
        {
            Console.WriteLine(e.HostKey);
        }

        public async void Connect()
        {
            if (!LaunchArgs.HasParam(HOST_IP_ADDRESS)) throw new Exception("Must pass in HOST_IP_ADDRESS as parameter.");
            var hostName = LaunchArgs.Parameters[HOST_IP_ADDRESS] as String;

            await PerformNetworkOperation(async () =>
            {
                try
                {
                    var ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(_privateKey));
                    var pkeFile = new PrivateKeyFile(ms, Password);
                    var auth = new PrivateKeyAuthenticationMethod(UserName, pkeFile);
                    var connectionInfo = new ConnectionInfo(hostName, UserName, auth);

                    _client = new SshClient(connectionInfo);
                    _client.HostKeyReceived += Client_HostKeyReceived;

                    _client.Connect();
                    _stream = _client.CreateShellStream("NuvIoT - Platform Manager", 0, 0, 0, 0, 0);
                    ReadFromStream(_stream);
                    await Storage.StoreKVP($"UserName-{hostName}", UserName);
                    await Storage.StoreKVP($"Key-{hostName}", _privateKey);
                    await Storage.StoreKVP($"Password-{hostName}", Password);
                    IsConnected = true;
                    IsDisconnected = false;
                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                }
                catch(Exception ex)
                {
                    await Popups.ShowAsync(ex.Message);
                }
            });
        }

        public void Disconnect()
        {
            if (_client != null)
            {
                _client.Disconnect();
                _client.Dispose();
                _client = null;
                IsConnected = false;
                IsDisconnected = true;
            }

            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }

        private async void ClearCredentials()
        {
            if (!LaunchArgs.HasParam(HOST_IP_ADDRESS)) throw new Exception("Must pass in HOST_IP_ADDRESS as parameter.");
            var hostName = LaunchArgs.Parameters[HOST_IP_ADDRESS] as String;

            await Storage.ClearKVP($"UserName-{hostName}");
            await Storage.ClearKVP($"Key-{hostName}");
            await Storage.ClearKVP($"Password-{hostName}");
        }

        public void ShowKey()
        {
            _key = _privateKey;
            RaisePropertyChanged(nameof(Key));
        }

        private bool CanConnect()
        {

            return _client == null &&
                  !String.IsNullOrEmpty(UserName)
                  && !String.IsNullOrEmpty(_privateKey)
                  && !String.IsNullOrEmpty(Password);
        }

        public void SendText(string text)
        {
            _stream.WriteLine(text);
        }

        private bool CanDisconnect()
        {
            return _client != null && _client.IsConnected;
        }

        public ObservableCollection<SSHResponse> ConsoleResponses { get; } = new ObservableCollection<SSHResponse>();

        public override Task IsClosingAsync()
        {
            if (_client != null)
            {
                _client.Disconnect();
                _client.Dispose();
            }

            return base.IsClosingAsync();
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                Set(ref _userName, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                Set(ref _password, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set
            {
                var key = value.Replace("\r", "\r\n");
                Set(ref _key, key);
                _privateKey = key;
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { Set(ref _isConnected, value); }
        }

        private bool _isDisconnected = true;
        public bool IsDisconnected
        {
            get { return _isDisconnected; }
            set { Set(ref _isDisconnected, value); }
        }

        private string _sshText;
        public string SSHText
        {
            get { return _sshText; }
            set { Set(ref _sshText, value); }
        }

        public RelayCommand ConnectCommand { get; }

        public RelayCommand DisconnectCommand { get; }
        public RelayCommand ClearCredentialsCommand { get; }

        public RelayCommand ShowKeyCommand { get; }
        public RelayCommand SendSSHTextCommand { get; }
    }
}

