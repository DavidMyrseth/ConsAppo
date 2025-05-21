using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

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
                await DisplayAlert("Success", "Registration successful", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "Username already exists", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
