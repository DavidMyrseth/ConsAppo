using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Teku
{
    public partial class MainPage : ContentPage
    {
        private List<Consultation> _consultations;
        private static readonly string ConsultationsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "consultations.txt");

        public MainPage()
        {
            InitializeComponent();
            LoadConsultations();
            AuthService.UserChanged += OnUserChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateUIForUserRole();
            LoadConsultations();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AuthService.UserChanged -= OnUserChanged;
        }

        private void OnUserChanged()
        {
            UpdateUIForUserRole();
            LoadConsultations();
        }

        private void LoadConsultations()
        {
            try
            {
                _consultations = new List<Consultation>();

                if (File.Exists(ConsultationsFilePath))
                {
                    foreach (var line in File.ReadAllLines(ConsultationsFilePath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            _consultations.Add(Consultation.FromString(line));
                        }
                    }
                }
                else
                {
                    _consultations.Add(new Consultation
                    {
                        Teacher = "Mr. Podkopaev",
                        Room = "101",
                        Time = "10:00-12:00",
                        Day = "Monday"
                    });
                    _consultations.Add(new Consultation
                    {
                        Teacher = "Ms. Merkulova",
                        Room = "202",
                        Time = "14:00-16:00",
                        Day = "Tuesday"
                    });
                    SaveConsultations();
                }

                if (AuthService.CurrentUser.Role == UserRole.Student && !string.IsNullOrEmpty(AuthService.CurrentUser.Group))
                {
                    _consultations = _consultations
                        .Where(c => c.Teacher == AuthService.CurrentUser.Group ||
                                    c.SignedUpStudents.Contains(AuthService.CurrentUser.Username))
                        .ToList();
                }

                DisplayConsultations();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading consultations: {ex.Message}");
                DisplayAlert("Error", "Failed to load consultations", "OK");
            }
        }

        private void SaveConsultations()
        {
            try
            {
                File.WriteAllLines(ConsultationsFilePath, _consultations.Select(c => c.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving consultations: {ex.Message}");
                DisplayAlert("Error", "Failed to save consultations", "OK");
            }
        }

        private void DisplayConsultations()
        {
            ConsultationsStack.Children.Clear();

            if (!_consultations.Any())
            {
                ConsultationsStack.Children.Add(new Label
                {
                    Text = "No consultations available",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20)
                });
                return;
            }

            foreach (var consultation in _consultations.OrderBy(c => c.Day).ThenBy(c => c.Time))
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10),
                    BackgroundColor = GetConsultationColor(consultation)
                };

                var stack = new VerticalStackLayout();

                stack.Children.Add(new Label
                {
                    Text = consultation.Teacher,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16
                });

                stack.Children.Add(new Label { Text = $"📅 {consultation.Day}" });
                stack.Children.Add(new Label { Text = $"🕒 {consultation.Time}" });
                stack.Children.Add(new Label { Text = $"🏫 Room: {consultation.Room}" });

                if (consultation.SignedUpStudents.Any())
                {
                    stack.Children.Add(new Label
                    {
                        Text = $"👥 Signed up: {consultation.SignedUpStudents.Count}",
                        FontAttributes = FontAttributes.Italic,
                        Margin = new Thickness(0, 5)
                    });
                }

                if (AuthService.CurrentUser.Role == UserRole.Student)
                {
                    AddStudentActions(stack, consultation);
                }
                else if (AuthService.CurrentUser.Role == UserRole.Teacher)
                {
                    AddTeacherActions(stack, consultation);
                }

                frame.Content = stack;
                ConsultationsStack.Children.Add(frame);
            }
        }

        private Color GetConsultationColor(Consultation consultation)
        {
            if (consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username))
            {
                return Color.FromArgb("#E3F2FD");
            }
            return Colors.White;
        }

        private void AddStudentActions(VerticalStackLayout stack, Consultation consultation)
        {
            var signUpButton = new Button
            {
                Text = consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username)
                    ? "Cancel Sign Up" : "Sign Up",
                BackgroundColor = consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username)
                    ? Colors.Red : Colors.Green,
                TextColor = Colors.White,
                Margin = new Thickness(0, 10)
            };

            signUpButton.Clicked += (sender, e) => OnSignUpClicked(consultation, signUpButton);
            stack.Children.Add(signUpButton);
        }

        private void AddTeacherActions(VerticalStackLayout stack, Consultation consultation)
        {
            var actionsStack = new HorizontalStackLayout
            {
                Spacing = 10,
                Margin = new Thickness(0, 10)
            };

            var editButton = new Button
            {
                Text = "✏️ Edit",
                BackgroundColor = Colors.LightGray,
                Command = new Command(async () => await OnEditConsultationClicked(consultation))
            };

            var deleteButton = new Button
            {
                Text = "🗑️ Delete",
                BackgroundColor = Colors.LightPink,
                Command = new Command(() => OnDeleteConsultationClicked(consultation))
            };

            actionsStack.Children.Add(editButton);
            actionsStack.Children.Add(deleteButton);
            stack.Children.Add(actionsStack);
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
            SaveConsultations();
            DisplayConsultations();
        }

        private async Task OnEditConsultationClicked(Consultation consultation)
        {
            await Navigation.PushModalAsync(new AddConsultationPage((updatedConsultation) =>
            {
                var index = _consultations.IndexOf(consultation);
                if (index >= 0)
                {
                    _consultations[index] = updatedConsultation;
                    SaveConsultations();
                    DisplayConsultations();
                }
            }, consultation));
        }

        private async void OnDeleteConsultationClicked(Consultation consultation)
        {
            bool confirm = await DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete consultation with {consultation.Teacher}?",
                "Delete", "Cancel");

            if (confirm)
            {
                _consultations.Remove(consultation);
                SaveConsultations();
                DisplayConsultations();
            }
        }

        private async void OnAddConsultationClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddConsultationPage((newConsultation) =>
            {
                _consultations.Add(newConsultation);
                SaveConsultations();
                DisplayConsultations();
            }));
        }

        private void UpdateUIForUserRole()
        {
            AddConsultationButton.IsVisible = AuthService.CurrentUser.Role == UserRole.Teacher;
            AddConsultationButton.Text = AuthService.CurrentUser.Role == UserRole.Admin
                ? "Add Consultation (Admin)" : "Add Consultation";
        }
    }

    public class Consultation
    {
        public string Teacher { get; set; }
        public string Room { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public List<string> SignedUpStudents { get; set; } = new List<string>();

        public override string ToString()
        {
            var students = string.Join(",", SignedUpStudents);
            return $"{Teacher}|{Room}|{Time}|{Day}|{students}";
        }

        public static Consultation FromString(string line)
        {
            var parts = line.Split('|');
            return new Consultation
            {
                Teacher = parts[0],
                Room = parts[1],
                Time = parts[2],
                Day = parts[3],
                SignedUpStudents = parts.Length > 4 && !string.IsNullOrEmpty(parts[4])
                    ? parts[4].Split(',').ToList()
                    : new List<string>()
            };
        }
    }
}
