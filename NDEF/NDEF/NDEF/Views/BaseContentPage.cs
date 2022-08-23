using System;
using NDEF.ViewModels;
using Xamarin.Forms;

namespace NDEF.Views
{
	public class BaseContentPage : ContentPage
	{
		public BaseContentPage(BaseViewModel baseViewModel)
		{
			BindingContext = baseViewModel;
		}
	}
}

