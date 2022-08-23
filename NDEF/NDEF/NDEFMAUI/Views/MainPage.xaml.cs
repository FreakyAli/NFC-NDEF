using NDEFMAUI.ViewModels;
using NDEFMAUI.Views;

namespace NDEFMAUI;

public partial class MainPage : BaseContentPage
{
    private readonly MainViewModel mainvm;

    public MainPage(MainViewModel mainvm) : base(mainvm)
    {
        InitializeComponent();
        this.mainvm = mainvm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        mainvm.ExecuteOnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        mainvm.ExecuteOnDisappearing();
    }
    //SemanticScreenReader.Announce();
}