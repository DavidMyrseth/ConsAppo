using System;
using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class AddConsultationPage : ContentPage
    {
        private readonly Action<Consultation> _addCallback;

        public AddConsultationPage(Action<Consultation> addCallback)
        {
            InitializeComponent(); // XAML инициализация
            _addCallback = addCallback;

            // Пример автозаполнения имени преподавателя, если авторизация есть
            if (AuthService.CurrentUser != null)
            {
                TeacherEntry.Text = AuthService.CurrentUser.Username;
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TeacherEntry.Text) ||
                string.IsNullOrWhiteSpace(RoomEntry.Text) ||
                string.IsNullOrWhiteSpace(TimeEntry.Text) ||
                string.IsNullOrWhiteSpace(DayEntry.Text))
            {
                await DisplayAlert("Error", "All fields are required", "OK");
                return;
            }

            var newConsultation = new Consultation
            {
                Teacher = TeacherEntry.Text,
                Room = RoomEntry.Text,
                Time = TimeEntry.Text,
                Day = DayEntry.Text
            };

            _addCallback?.Invoke(newConsultation);
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
