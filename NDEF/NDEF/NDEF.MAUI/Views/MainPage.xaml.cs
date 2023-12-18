using NDEF.MAUI.ViewModels;
using NDEF.MAUI.Views;

namespace NDEF.MAUI;

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
}
