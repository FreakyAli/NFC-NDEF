using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDEF.ViewModels;
using NDEF.Views;
using Xamarin.Forms;

namespace NDEF
{
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
}

