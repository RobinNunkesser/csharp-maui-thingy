namespace Thingy;

public class MainViewModel : ViewModel
{
    
    public MainViewModel(BaseServices services) : base(services)
    {
        
        
        this.Navigate = ReactiveCommand.CreateFromTask<string>(async uri =>
        {
            await services.Navigator.Navigate("NavigationPage/" + uri);
            this.IsMenuVisible = false;
        });
    }


    [Reactive] public string Property { get; set; }
    [Reactive] public bool IsMenuVisible { get; private set; }
    public ICommand Navigate { get; }
}