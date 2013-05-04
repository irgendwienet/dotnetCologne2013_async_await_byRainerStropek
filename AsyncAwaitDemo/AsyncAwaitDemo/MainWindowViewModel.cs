using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace AsyncAwaitDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private SyncHeatSensor syncSensor = new SyncHeatSensor();
        private AsyncHeatSensor asyncSensor = new AsyncHeatSensor();

        private Action<string> stateNavigator;
        private CancellationTokenSource cts;

        public MainWindowViewModel(Action<string> stateNavigator)
        {
            this.stateNavigator = stateNavigator;

            this.InternalConnectAndUpdateSync = new DelegateCommand(
                this.ConnectSync,
                () => !this.IsUpdating);

            this.InternalConnectAndUpdateAsync = new DelegateCommand(
                this.ConnectAsync,
                () => !this.IsUpdating);
            this.InternalCancelConnectAndUpdateAsync = new DelegateCommand(
                () => { if (this.cts != null) this.cts.Cancel(); },
                () => this.IsUpdating);
        }

        private void ConnectSync()
        {
            var address = Dns.GetHostAddresses("localhost");
            this.syncSensor.Connect(address.FirstOrDefault());
            this.syncSensor.UploadFirmware(new byte[] { 0, 1, 2 });
            this.syncSensor.TryDisconnect();
            MessageBox.Show("Successfully updated");
        }

        private async void ConnectAsync()
        {
            this.IsUpdating = true;
            this.cts = new CancellationTokenSource();
            this.stateNavigator("Updating");
            var ip = await Dns.GetHostAddressesAsync("localhost");
            await this.asyncSensor.ConnectAsync(ip.FirstOrDefault());
            var success = false;
            try
            {
                await this.asyncSensor.UploadFirmwareAsync(
                    new byte[] { 0, 1, 2 },
                    this.cts.Token,
                    new Progress<int>(p => this.Progress = p));
                success = true;
            }
            catch (OperationCanceledException)
            {
            }

            await this.asyncSensor.TryDisconnectAsync();
            this.stateNavigator(success ? "Updated" : "Cancelled");
            this.IsUpdating = false;
            if (success)
            {
                MessageBox.Show("Successfully updated");
            }
        }

        private DelegateCommand InternalConnectAndUpdateSync;
        public ICommand ConnectAndUpdateSync
        {
            get
            {
                return this.InternalConnectAndUpdateSync;
            }
        }

        private DelegateCommand InternalConnectAndUpdateAsync;
        public ICommand ConnectAndUpdateAsync
        {
            get
            {
                return this.InternalConnectAndUpdateAsync;
            }
        }

        private DelegateCommand InternalCancelConnectAndUpdateAsync;
        public ICommand CancelConnectAndUpdateAsync
        {
            get
            {
                return this.InternalCancelConnectAndUpdateAsync;
            }
        }

        private bool IsUpdatingValue;
        public bool IsUpdating
        {
            get
            {
                return this.IsUpdatingValue;
            }

            set
            {
                if (this.IsUpdatingValue != value)
                {
                    this.IsUpdatingValue = value;
                    this.RaisePropertyChanged();
                    this.InternalConnectAndUpdateAsync.RaiseCanExecuteChanged();
                    this.InternalCancelConnectAndUpdateAsync.RaiseCanExecuteChanged();
                }
            }
        }

        private int ProgressValue;
        public int Progress
        {
            get
            {
                return this.ProgressValue;
            }

            set
            {
                if (this.ProgressValue != value)
                {
                    this.ProgressValue = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}