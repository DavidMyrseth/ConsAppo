using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui;

namespace Teku
{
    public partial class MainPage : ContentPage
    {
        private List<Consultation> _consultations;
        private static readonly string ConsultationsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "consultations.txt");

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

        private void UpdateUIForUserRole()
        {
            if (AddConsultationButton == null) return;

            AddConsultationButton.IsVisible = AuthService.CurrentUser.Role == UserRole.Teacher ||
                                           AuthService.CurrentUser.Role == UserRole.Admin;

            AddConsultationButton.Text = AuthService.CurrentUser.Role == UserRole.Admin
                ? "Add Consultation (Admin)"
                : "Add Consultation";
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
                    // Default consultations
                    _consultations.Add(new Consultation
                    {
                        Teacher = "Mr. Podkopaev",
                        Room = "101",
                        Time = "10:00-12:00",
                        Day = "Monday",
                        IsApproved = true
                    });
                    _consultations.Add(new Consultation
                    {
                        Teacher = "Ms. Merkulova",
                        Room = "202",
                        Time = "14:00-16:00",
                        Day = "Tuesday",
                        IsApproved = true
                    });
                    SaveConsultations();
                }

                // Filter for students
                if (AuthService.CurrentUser.Role == UserRole.Student)
                {
                    _consultations = _consultations
                        .Where(c => c.IsApproved)
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

        private void DisplayConsultations()
        {
            if (ConsultationsStack == null) return;

            MainThread.BeginInvokeOnMainThread(() =>
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

                    // Consultation info
                    stack.Children.Add(new Label
                    {
                        Text = consultation.Teacher,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    });

                    stack.Children.Add(new Label { Text = $"📅 {consultation.Day}" });
                    stack.Children.Add(new Label { Text = $"🕒 {consultation.Time}" });
                    stack.Children.Add(new Label { Text = $"🏫 Room: {consultation.Room}" });

                    // Status label
                    stack.Children.Add(new Label
                    {
                        Text = consultation.IsApproved ? "✅ Approved" : "🟡 Pending Approval",
                        FontAttributes = FontAttributes.Bold,
                        TextColor = consultation.IsApproved ? Colors.Green : Colors.Orange,
                        Margin = new Thickness(0, 5)
                    });

                    // Student count if any
                    if (consultation.SignedUpStudents.Any())
                    {
                        stack.Children.Add(new Label
                        {
                            Text = $"👥 Signed up: {consultation.SignedUpStudents.Count}",
                            FontAttributes = FontAttributes.Italic,
                            Margin = new Thickness(0, 5)
                        });
                    }

                    // Add buttons based on user role
                    if (AuthService.CurrentUser.Role == UserRole.Student)
                    {
                        AddStudentActions(stack, consultation);
                    }
                    else if (AuthService.CurrentUser.Role == UserRole.Teacher ||
                            AuthService.CurrentUser.Role == UserRole.Admin)
                    {
                        AddTeacherActions(stack, consultation);
                    }

                    frame.Content = stack;
                    ConsultationsStack.Children.Add(frame);
                }
            });
        }

        private Color GetConsultationColor(Consultation consultation)
        {
            if (consultation.SignedUpStudents.Contains(AuthService.CurrentUser.Username))
            {
                return Color.FromArgb("#E3F2FD"); // Light blue for signed up consultations
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

            var requestButton = new Button
            {
                Text = consultation.RequestedStudents.Contains(AuthService.CurrentUser.Username)
                    ? "Cancel Request" : "Request Consultation",
                BackgroundColor = consultation.RequestedStudents.Contains(AuthService.CurrentUser.Username)
                    ? Colors.OrangeRed : Colors.Orange,
                TextColor = Colors.White,
                Margin = new Thickness(0, 5)
            };

            requestButton.Clicked += (sender, e) => OnRequestConsultationClicked(consultation, requestButton);
            stack.Children.Add(requestButton);
        }

        private void AddTeacherActions(VerticalStackLayout stack, Consultation consultation)
        {
            var actionsStack = new HorizontalStackLayout
            {
                Spacing = 10,
                Margin = new Thickness(0, 10)
            };

            // Delete button
            var deleteButton = new Button
            {
                Text = "🗑️ Delete",
                BackgroundColor = Colors.LightPink,
                TextColor = Colors.Black
            };
            deleteButton.Clicked += (sender, e) => OnDeleteConsultationClicked(consultation);
            actionsStack.Children.Add(deleteButton);

            // Accept button (only for unapproved consultations)
            if (!consultation.IsApproved)
            {
                var acceptButton = new Button
                {
                    Text = "Accept Consultation",
                    BackgroundColor = Colors.LightGreen,
                    TextColor = Colors.Black
                };
                acceptButton.Clicked += (sender, e) => OnAcceptConsultationClicked(consultation);
                actionsStack.Children.Add(acceptButton);
            }

            // Approve all requests button
            if (consultation.RequestedStudents.Any())
            {
                var approveAllButton = new Button
                {
                    Text = "Approve All",
                    BackgroundColor = Colors.LightBlue,
                    TextColor = Colors.Black
                };
                approveAllButton.Clicked += (sender, e) => OnMarkConsultationAvailableClicked(consultation);
                actionsStack.Children.Add(approveAllButton);
            }

            stack.Children.Add(actionsStack);

            // Show requested students if any
            if (consultation.RequestedStudents.Any())
            {
                stack.Children.Add(new Label
                {
                    Text = "📥 Requested Students:",
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                foreach (var student in consultation.RequestedStudents)
                {
                    stack.Children.Add(new Label { Text = $"• {student}" });
                }
            }
        }

        private void OnAcceptConsultationClicked(Consultation consultation)
        {
            consultation.IsApproved = true;
            SaveConsultations();
            DisplayConsultations();
            DisplayAlert("Success", "Consultation has been approved and is now visible to students", "OK");
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

        private void OnRequestConsultationClicked(Consultation consultation, Button button)
        {
            if (consultation.RequestedStudents.Contains(AuthService.CurrentUser.Username))
            {
                consultation.RequestedStudents.Remove(AuthService.CurrentUser.Username);
                button.Text = "Request Consultation";
                button.BackgroundColor = Colors.Orange;
            }
            else
            {
                consultation.RequestedStudents.Add(AuthService.CurrentUser.Username);
                button.Text = "Cancel Request";
                button.BackgroundColor = Colors.OrangeRed;
            }

            SaveConsultations();
            DisplayConsultations();
        }

        private void OnMarkConsultationAvailableClicked(Consultation consultation)
        {
            foreach (var student in consultation.RequestedStudents)
            {
                if (!consultation.SignedUpStudents.Contains(student))
                {
                    consultation.SignedUpStudents.Add(student);
                }
            }

            consultation.RequestedStudents.Clear();
            SaveConsultations();
            DisplayConsultations();
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
    }
}