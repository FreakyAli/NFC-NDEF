using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NDEF
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var navigation = new NavigationPage(new MainPage())
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

