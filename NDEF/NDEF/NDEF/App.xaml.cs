using System;
using NDEF.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NDEF
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var navigation = new NavigationPage(new MainPage(new MainViewModel()))
            {
                BarTextColor = Color.White,
                BarBackgroundColor = Color.Blue,
            };
            MainPage = navigation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

