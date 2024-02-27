using Shiny.BluetoothLE;

namespace Thingy;

public class ConnectionViewModel : ViewModel
{
    IDisposable? _scanSub;
    
    public ConnectionViewModel(BaseServices services, IBleManager bleManager) : base(
        services)
    {
        Scan(bleManager); 
    }

    private async void Scan(IBleManager bleManager)
    {
        this.IsScanning = bleManager?.IsScanning ?? false;
        
            if (bleManager == null)
            {
                await this.Alert("Platform Not Supported");
                return;
            }
            if (this.IsScanning)
            {
                this.StopScan();
            }
            else
            {
                this.IsScanning = true;

                async void OnNextScanResults(IList<ScanResult> results)
                {
                    foreach (var result in results)
                    {
                        if (result.Peripheral.Name is not "Thingy") continue;
                        this.StopScan();
                        await this.Navigation.Navigate("Sensors", ("Peripheral", result.Peripheral));
                    }
                }

                this._scanSub = bleManager
                    .Scan()
                    .Buffer(TimeSpan.FromSeconds(1))
                    .Where(x => x?.Any() ?? false)
                    .SubOnMainThread(OnNextScanResults,
                        ex => this.Alert(ex.ToString(), "ERROR")
                    );
            }
    }
    
    [Reactive] public bool IsScanning { get; private set; }

    private void StopScan()
    {
        this._scanSub?.Dispose();
        this._scanSub = null;
        this.IsScanning = false;
    }
}