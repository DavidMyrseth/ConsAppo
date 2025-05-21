using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
namespace Teku;

public partial class TeacherConsultationsPage : ContentPage
{
	public TeacherConsultationsPage()
	{
		InitializeComponent();
        LoadStudentRequests();
    }

    private void LoadStudentRequests()
    {
        RequestsStack.Children.Clear();

        var teacherConsultations = AppData.Consultations
            .Where(c => c.Teacher == AuthService.CurrentUser.Username)
            .ToList();

        if (teacherConsultations.Count == 0)
        {
            RequestsStack.Children.Add(new Label
            {
                Text = "No consultation requests yet",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            });
            return;
        }

        foreach (var consultation in teacherConsultations)
        {
            if (consultation.SignedUpStudents.Count > 0)
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10)
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
    }
}