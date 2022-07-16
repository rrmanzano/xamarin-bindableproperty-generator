using Xam.BindableProperty.Generator.Demo.ViewModels;
using Xamarin.Forms;

namespace Xam.BindableProperty.Generator.Demo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainViewModel();
        }
    }
}
