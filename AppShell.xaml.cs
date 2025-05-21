using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class AppShell : Shell
    {
        public bool IsLoggedIn => AuthService.CurrentUser?.Role != UserRole.Guest;
        public bool IsTeacher => AuthService.CurrentUser?.Role == UserRole.Teacher;

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            // Обновляем видимость вкладок при изменении пользователя
            AuthService.UserChanged += OnUserChanged;

            // Register routes for modal pages
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(AddConsultationPage), typeof(AddConsultationPage));
        }

        private void OnUserChanged()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsTeacher));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            // Проверка авторизации при переходе на защищенные страницы
            if (args.Target.Location.OriginalString.Contains(nameof(ProfilePage)) &&
                AuthService.CurrentUser?.Role == UserRole.Guest)
            {
                args.Cancel();
                Shell.Current.GoToAsync(nameof(LoginPage));
            }

            base.OnNavigating(args);
        }
    }
}