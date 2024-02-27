using Shiny.BluetoothLE;

namespace Thingy;

public class SensorsViewModel : ViewModel
{
    IPeripheral peripheral = null!;
    
    public SensorsViewModel(BaseServices services) : base(services)
    {
        this.Load = ReactiveCommand.CreateFromTask(async () =>
        {
            var services = await this.peripheral
                .WithConnectIf()
                .Select(x => x.GetServices())
                .Switch()
                .ToTask();
            foreach (var service in services)
            {
                switch (service.Uuid)
                {
                    
                }
            }
        });
    }
    
    public ICommand Load { get; }
    
    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        this.peripheral = parameters.GetValue<IPeripheral>("Peripheral")!;
        this.Title = this.peripheral.Name ?? "No Name";

        this.Load.Execute(null);
    }
}

