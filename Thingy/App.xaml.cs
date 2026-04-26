using System.Diagnostics;
using Shiny.BluetoothLE;

namespace Thingy;

public partial class App : Application
{
    private readonly IBleManager bleManager;

    public App(IBleManager bleManager)
    {
        InitializeComponent();
        this.bleManager = bleManager;
    }

    protected override async void OnStart()
    {
        var access = await bleManager.RequestAccess();
        if (access != AccessState.Available)
            Debug.WriteLine(
                "BLE Access not available, please check your permissions and try again.");
        // handle accordingly
    }
}