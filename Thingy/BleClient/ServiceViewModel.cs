using Shiny.BluetoothLE;

namespace Thingy;

public class ServiceViewModel : ViewModel
{
    private IPeripheral peripheral = null!;
    private BleServiceInfo service = null!;

    public ServiceViewModel(BaseServices services) : base(services)
    {
        Load = ReactiveCommand.CreateFromTask(async () =>
        {
            IsBusy = true;
            await peripheral.ConnectAsync();
            Characteristics =
                (await peripheral!.GetCharacteristics(service.Uuid)).ToList();
            IsBusy = false;
        });

        this.WhenAnyValueSelected(
            x => x.SelectedCharacteristic,
            x => Navigation.Navigate(
                "BlePeripheralCharacteristic",
                ("Peripheral", peripheral),
                ("Characteristic", x!)
            )
        );
    }


    public ICommand Load { get; }

    [Reactive]
    public List<BleCharacteristicInfo> Characteristics { get; private set; }

    [Reactive]
    public BleCharacteristicInfo? SelectedCharacteristic { get; set; }


    public override void OnAppearing()
    {
        Load.Execute(null);
    }

    public override Task InitializeAsync(INavigationParameters parameters)
    {
        service = parameters.GetValue<BleServiceInfo>("Service");
        peripheral = parameters.GetValue<IPeripheral>("Peripheral");
        Title = $"{peripheral.Name} - {service.Uuid}";

        return Task.CompletedTask;
    }
}