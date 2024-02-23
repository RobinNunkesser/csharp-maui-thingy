using Foundation;
using UIKit;

namespace Thingy;


[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() 
    {
        return MauiProgram.CreateMauiApp();
    }

}
