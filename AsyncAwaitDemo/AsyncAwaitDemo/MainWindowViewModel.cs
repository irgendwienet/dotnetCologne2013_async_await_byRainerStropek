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

        public MainWindowViewModel()
        {
            this.InternalConnectAndUpdateSync = new DelegateCommand(
                this.ConnectSync,
                () => true);
        }

        private void ConnectSync()
        {
            var address = Dns.GetHostAddresses("localhost");
            this.syncSensor.Connect(address.FirstOrDefault());
            this.syncSensor.UploadFirmware(new byte[] { 0, 1, 2 });
            this.syncSensor.TryDisconnect();
            MessageBox.Show("Successfully updated");
        }

        private DelegateCommand InternalConnectAndUpdateSync;
        public ICommand ConnectAndUpdateSync
        {
            get
            {
                return this.InternalConnectAndUpdateSync;
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