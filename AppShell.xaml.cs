using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class AppShell : Shell
    {
        public bool IsLoggedIn => AuthService.CurrentUser?.Role != UserRole.Guest;
        public bool IsTeacher => AuthService.CurrentUser?.Role == UserRole.Teacher;
        public bool IsStudent => IsLoggedIn && !IsTeacher;

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            RegisterRoutes();

            AuthService.UserChanged += OnUserChanged;
            UpdateTabsVisibility();
        }

        private void UpdateTabsVisibility()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsTeacher));
            OnPropertyChanged(nameof(IsStudent));
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("ProfilePage", typeof(ProfilePage));
            Routing.RegisterRoute("TeacherConsultationsPage", typeof(TeacherConsultationsPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("AddConsultationPage", typeof(AddConsultationPage));
        }

        private void OnUserChanged()
        {
            MainThread.BeginInvokeOnMainThread(UpdateTabsVisibility);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AuthService.UserChanged -= OnUserChanged;
        }
    }
}