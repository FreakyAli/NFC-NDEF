using System;
using NDEF.MAUI.ViewModels;

namespace NDEF.MAUI.Views
{
	public class BaseContentPage: ContentPage
	{
		public BaseContentPage(BaseViewModel baseViewModel)
		{
			BindingContext = baseViewModel;
		}
	}
}

