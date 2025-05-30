using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            RolePicker.SelectedIndexChanged += OnRoleSelected;
        }

        private void OnRoleSelected(object sender, EventArgs e)
        {
            GroupEntry.IsVisible = RolePicker.SelectedItem?.ToString() == "Student";
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Username and password are required", "OK");
                return;
            }

            var role = RolePicker.SelectedItem?.ToString() == "Teacher" ? UserRole.Teacher : UserRole.Student;
            var group = role == UserRole.Student ? GroupEntry.Text : null;

            if (AuthService.Register(UsernameEntry.Text, PasswordEntry.Text, role, group))
            {
                // Автоматический вход после регистрации
                if (AuthService.Login(UsernameEntry.Text, PasswordEntry.Text))
                {
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await DisplayAlert("Error", "Auto-login failed", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Username already exists", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}