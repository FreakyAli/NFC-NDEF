using NDEF.MAUI.Interfaces;
using NDEF.MAUI.ViewModels;

namespace NDEF.MAUI;

public partial class App : Application
{
	public App(IServiceProvider provider)
	{
		InitializeComponent();
		var viewModel = provider.GetService<MainViewModel>();
		MainPage =  new MainPage(viewModel);
	}
}

