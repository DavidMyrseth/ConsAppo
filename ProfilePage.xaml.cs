using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Teku
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BuildUI();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildUI();
        }

        private void BuildUI()
        {
            MainStack.Children.Clear();

            if (AuthService.CurrentUser?.Role == UserRole.Guest)
            {
                ShowGuestUI();
            }
            else
            {
                ShowAuthenticatedUI();
            }
        }

        private void ShowGuestUI()
        {
            var titleLabel = new Label
            {
                Text = "Welcome Guest",
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20)
            };

            var buttonStack = new HorizontalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 30)
            };

            var registerButton = new Button
            {
                Text = "Register",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                BorderColor = Colors.Black,
                BorderWidth = 1,
                CornerRadius = 10,
                WidthRequest = 150,
                HeightRequest = 50
            };
            registerButton.Clicked += OnRegisterClicked;

            var loginButton = new Button
            {
                Text = "Sign In",
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 150,
                HeightRequest = 50
            };
            loginButton.Clicked += OnLoginClicked;

            buttonStack.Children.Add(registerButton);
            buttonStack.Children.Add(loginButton);

            MainStack.Children.Add(titleLabel);
            MainStack.Children.Add(buttonStack);
        }

        private void ShowAuthenticatedUI()
        {
            var user = AuthService.CurrentUser;

            var titleLabel = new Label
            {
                Text = $"Welcome, {user.Username}!",
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20)
            };

            var userInfoStack = new VerticalStackLayout
            {
                Spacing = 10,
                Margin = new Thickness(20, 30)
            };

            userInfoStack.Children.Add(new Label
            {
                Text = $"Role: {user.Role}",
                FontSize = 18
            });

            if (!string.IsNullOrEmpty(user.FullName))
            {
                userInfoStack.Children.Add(new Label
                {
                    Text = $"Full Name: {user.FullName}",
                    FontSize = 18
                });
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                userInfoStack.Children.Add(new Label
                {
                    Text = $"Email: {user.Email}",
                    FontSize = 18
                });
            }

            if (user.Role == UserRole.Student && !string.IsNullOrEmpty(user.Group))
            {
                userInfoStack.Children.Add(new Label
                {
                    Text = $"Group: {user.Group}",
                    FontSize = 18
                });
            }

            var logoutButton = new Button
            {
                Text = "Logout",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                CornerRadius = 10,
                WidthRequest = 200,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 40)
            };
            logoutButton.Clicked += OnLogoutClicked;

            MainStack.Children.Add(titleLabel);
            MainStack.Children.Add(userInfoStack);
            MainStack.Children.Add(logoutButton);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new LoginPage());
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushModalAsync(new RegisterPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            AuthService.Logout();
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}