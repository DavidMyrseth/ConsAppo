using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Teku
{
    public partial class TeacherConsultationsPage : ContentPage
    {
        private static readonly string ConsultationsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "consultations.txt");

        public TeacherConsultationsPage()
        {
            InitializeComponent();
            LoadStudentRequests();
        }

        private List<Consultation> LoadConsultationsFromFile()
        {
            var consultations = new List<Consultation>();

            if (File.Exists(ConsultationsFilePath))
            {
                foreach (var line in File.ReadAllLines(ConsultationsFilePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        consultations.Add(Consultation.FromString(line));
                    }
                }
            }

            return consultations;
        }

        private void LoadStudentRequests()
        {
            RequestsStack.Children.Clear();

            var teacherConsultations = LoadConsultationsFromFile()
                .Where(c => c.Teacher == AuthService.CurrentUser?.Username)
                .ToList();

            if (!teacherConsultations.Any())
            {
                RequestsStack.Children.Add(new Label
                {
                    Text = "You don't have any consultations yet",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
                return;
            }

            bool hasRequests = false;

            foreach (var consultation in teacherConsultations)
            {
                if (consultation.SignedUpStudents.Count > 0)
                {
                    hasRequests = true;
                    var frame = new Frame
                    {
                        CornerRadius = 10,
                        HasShadow = true,
                        Padding = 15,
                        Margin = new Thickness(0, 0, 0, 10),
                        BackgroundColor = Colors.White
                    };

                    var stack = new VerticalStackLayout();

                    stack.Children.Add(new Label
                    {
                        Text = $"Consultation: {consultation.Day} at {consultation.Time}",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    });

                    stack.Children.Add(new Label
                    {
                        Text = $"Room: {consultation.Room}",
                        FontSize = 14
                    });

                    stack.Children.Add(new Label
                    {
                        Text = "Students signed up:",
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 10, 0, 0)
                    });

                    foreach (var student in consultation.SignedUpStudents)
                    {
                        stack.Children.Add(new Label
                        {
                            Text = $"• {student}",
                            Margin = new Thickness(10, 0, 0, 0)
                        });
                    }

                    frame.Content = stack;
                    RequestsStack.Children.Add(frame);
                }
            }

            if (!hasRequests)
            {
                RequestsStack.Children.Add(new Label
                {
                    Text = "No students have signed up for your consultations yet",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
            }
        }
    }
}