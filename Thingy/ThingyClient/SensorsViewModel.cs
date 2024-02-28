/// https://nordicsemiconductor.github.io/Nordic-Thingy52-FW/documentation/firmware_architecture.html#fw_arch_ble_services

using System.Diagnostics;
using Shiny.BluetoothLE;

namespace Thingy;

public class SensorsViewModel : ViewModel
{
    private IPeripheral peripheral = null!;

    public SensorsViewModel(BaseServices services) : base(services)
    {
        Load = ReactiveCommand.CreateFromTask(async () =>
        {
            var services = await peripheral
                .WithConnectIf()
                .Select(x => x.GetServices())
                .Switch()
                .ToTask();
            foreach (var service in services)
                switch (service.Uuid.ToUpper())
                {
                    case ThingyUUIDs.ThingyConfigurationUuid: break;
                    case ThingyUUIDs.WeatherStationUuid:
                        WeatherStationService();
                        break;
                    case ThingyUUIDs.SecureDFUuuid: break;
                    case ThingyUUIDs.ThingyMotionUuid: break;
                    case ThingyUUIDs.BatteryUuid:
                        BatteryService();
                        break;
                    case ThingyUUIDs.UIuuid: break;
                    case ThingyUUIDs.ThingySoundUuid: break;
                }
        });
    }

    [Reactive] public byte BatteryLevel { get; private set; }

    public ICommand Load { get; }

    private async Task<List<BleCharacteristicInfo>> GetCharacteristics(
        string uuid)
    {
        return (await peripheral!.GetCharacteristics(uuid)).ToList();
    }

    private async Task<List<BleDescriptorInfo>> GetDescriptors(
        BleCharacteristicInfo characteristicInfo)
    {
        return (await peripheral!.GetDescriptorsAsync(characteristicInfo))
            .ToList();
    }

    private async void WeatherStationService()
    {
        var characteristics =
            await GetCharacteristics(ThingyUUIDs.WeatherStationUuid);
        foreach (var characteristic in characteristics)
        {
            var descriptors = await GetDescriptors(characteristic);
            foreach (var descriptor in descriptors) Console.Write(descriptor);
        }
    }

    private async void BatteryService()
    {
        var characteristics =
            await GetCharacteristics(ThingyUUIDs.BatteryUuid);
        foreach (var characteristic in characteristics)
            if (characteristic.CanRead())
            {
                var result =
                    await peripheral!.ReadCharacteristicAsync(characteristic);
                if (result.Data != null)
                {
                    BatteryLevel = result.Data[0];
                    Debug.WriteLine($"Battery level read as {BatteryLevel}");
                }
            }
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        peripheral = parameters.GetValue<IPeripheral>("Peripheral")!;
        Title = peripheral.Name ?? "No Name";

        Load.Execute(null);
    }
}