using Shiny.BluetoothLE;

namespace Thingy;

public class ScanViewModel : ViewModel
{
    private IDisposable? scanSub;


    public ScanViewModel(BaseServices services, IBleManager bleManager) : base(
        services)
    {
        IsScanning = bleManager?.IsScanning ?? false;

        this.WhenAnyValueSelected(x => x.SelectedPeripheral, async x =>
        {
            StopScan();
            await Navigation.Navigate("BlePeripheral",
                ("Peripheral", x.Peripheral));
        });

        ScanToggle = ReactiveCommand.CreateFromTask(
            async () =>
            {
                if (bleManager == null)
                {
                    await Alert("Platform Not Supported");
                    return;
                }

                if (IsScanning)
                {
                    StopScan();
                }
                else
                {
                    Peripherals.Clear();
                    IsScanning = true;

                    scanSub = bleManager
                        .Scan()
                        .Buffer(TimeSpan.FromSeconds(1))
                        .Where(x => x?.Any() ?? false)
                        .SubOnMainThread(
                            results =>
                            {
                                var list = new List<PeripheralItemViewModel>();
                                foreach (var result in results)
                                {
                                    var peripheral =
                                        Peripherals.FirstOrDefault(x =>
                                            x.Equals(result.Peripheral));
                                    if (peripheral == null)
                                        peripheral = list.FirstOrDefault(x =>
                                            x.Equals(result.Peripheral));

                                    if (peripheral != null)
                                    {
                                        peripheral.Update(result);
                                    }
                                    else
                                    {
                                        peripheral =
                                            new PeripheralItemViewModel(
                                                result.Peripheral);
                                        peripheral.Update(result);
                                        list.Add(peripheral);
                                    }
                                }

                                if (list.Any())
                                    // XF is not able to deal with an observablelist/addrange properly
                                    foreach (var item in list)
                                        Peripherals.Add(item);
                            },
                            ex => Alert(ex.ToString())
                        );
                }
            }
        );
    }

    public ICommand NavToTest { get; }
    public ICommand ScanToggle { get; }

    public ObservableCollection<PeripheralItemViewModel> Peripherals { get; } =
        new();

    [Reactive] public PeripheralItemViewModel? SelectedPeripheral { get; set; }
    [Reactive] public bool IsScanning { get; private set; }


    private void StopScan()
    {
        scanSub?.Dispose();
        scanSub = null;
        IsScanning = false;
    }
}