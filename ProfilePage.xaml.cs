using Microsoft.Maui.Controls;

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

            if (AuthService.CurrentUser.Role == UserRole.Guest)
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
                Margin = new Thickness(10, 20, 10, 5)
            };

            var buttonStack = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0)
            };

            var registerButton = new Button
            {
                Text = "Register",
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                BorderColor = Colors.Black,
                BorderWidth = 1,
                CornerRadius = 5,
                WidthRequest = 120
            };
            registerButton.Clicked += OnRegisterClicked;

            var signInButton = new Button
            {
                Text = "Sign In",
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                CornerRadius = 5,
                WidthRequest = 120
            };
            signInButton.Clicked += OnSignInClicked;

            buttonStack.Children.Add(registerButton);
            buttonStack.Children.Add(signInButton);

            MainStack.Children.Add(titleLabel);
            MainStack.Children.Add(buttonStack);
        }

        private void ShowAuthenticatedUI()
        {
            var titleLabel = new Label
            {
                Text = $"Welcome {AuthService.CurrentUser.Username}",
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(10, 20, 10, 5)
            };

            var roleLabel = new Label
            {
                Text = $"Role: {AuthService.CurrentUser.Role}",
                FontSize = 16,
                Margin = new Thickness(10, 0, 10, 20)
            };

            var logoutButton = new Button
            {
                Text = "Logout",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                CornerRadius = 5,
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20)
            };
            logoutButton.Clicked += OnLogoutClicked;

            MainStack.Children.Add(titleLabel);
            MainStack.Children.Add(roleLabel);
            MainStack.Children.Add(logoutButton);

            // Add role-specific UI
            if (AuthService.CurrentUser.Role == UserRole.Student)
            {
                MainStack.Children.Add(new Label
                {
                    Text = $"Group: {AuthService.CurrentUser.Group}",
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 20)
                });
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegisterPage());
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            AuthService.Logout();
            BuildUI();
        }
    }
}