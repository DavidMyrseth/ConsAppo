using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Please enter both username and password", "OK");
                return;
            }

            if (AuthService.Login(UsernameEntry.Text, PasswordEntry.Text))
            {
                // Закрываем модальное окно и переходим на главную страницу
                await Shell.Current.Navigation.PopModalAsync();
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Просто закрываем модальное окно
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Переход на страницу регистрации
            await Shell.Current.Navigation.PushModalAsync(new RegisterPage());
        }
    }
}