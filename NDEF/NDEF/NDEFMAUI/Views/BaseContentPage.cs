using NDEFMAUI.ViewModels;

namespace NDEFMAUI.Views;

public class BaseContentPage : ContentPage
{
    public BaseContentPage(BaseViewModel baseViewModel)
    {
        BindingContext = baseViewModel;
    }
}
