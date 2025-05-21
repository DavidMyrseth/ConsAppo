using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Teku
{
    public class Consultation
    {
        public string Teacher { get; set; }
        public string Room { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public List<string> SignedUpStudents { get; set; } = new List<string>();
    }

    public partial class MainPage : ContentPage
    {
        private List<Consultation> _consultations = new List<Consultation>
        {
            new Consultation { Teacher = "Mr. Smith", Room = "101", Time = "10:00-12:00", Day = "Monday" },
            new Consultation { Teacher = "Ms. Johnson", Room = "202", Time = "14:00-16:00", Day = "Tuesday" }
        };

        public MainPage()
        {
            InitializeComponent();
            LoadConsultations();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateUIForUserRole();
            LoadConsultations();
        }

        private void UpdateUIForUserRole()
        {
            AddConsultationButton.IsVisible = AuthService.CurrentUser.Role == UserRole.Teacher;
        }

        private void LoadConsultations()
        {
            ConsultationsStack.Children.Clear();

            foreach (var consultation in _consultations)
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15
                };

                var stack = new VerticalStackLayout();

                stack.Children.Add(new Label { Text = $"Teacher: {consultation.Teacher}", FontAttributes = FontAttributes.Bold });
                stack.Children.Add(new Label { Text = $"Room: {consultation.Room}" });
                stack.Children.Add(new Label { Text = $"Time: {consultation.Time}" });
                stack.Children.Add(new Label { Text = $"Day: {consultation.Day}" });

                if (AuthService.CurrentUser.Role == UserRole.Student)
                {
                    var signUpButton = new Button
                    {
                        Text = consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username) ?
                            "Cancel Sign Up" : "Sign Up",
                        BackgroundColor = consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username) ?
                            Colors.Red : Colors.Green,
                        TextColor = Colors.White,
                        Margin = new Thickness(0, 10)
                    };

                    signUpButton.Clicked += (sender, e) => OnSignUpClicked(consultation, signUpButton);
                    stack.Children.Add(signUpButton);
                }

                frame.Content = stack;
                ConsultationsStack.Children.Add(frame);
            }
        }

        private void OnSignUpClicked(Consultation consultation, Button button)
        {
            if (consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username))
            {
                consultation.SignedUpStudents.Remove(AuthService.CurrentUser.Username);
                button.Text = "Sign Up";
                button.BackgroundColor = Colors.Green;
            }
            else
            {
                consultation.SignedUpStudents.Add(AuthService.CurrentUser.Username);
                button.Text = "Cancel Sign Up";
                button.BackgroundColor = Colors.Red;
            }
        }

        private async void OnAddConsultationClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddConsultationPage((newConsultation) =>
            {
                _consultations.Add(newConsultation);
                LoadConsultations();
            }));
        }
    }
}