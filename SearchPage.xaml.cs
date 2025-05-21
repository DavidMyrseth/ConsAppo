 using Microsoft.Maui.Controls;

namespace Teku
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            var mainStack = new VerticalStackLayout
            {
                Padding = new Thickness(10)
            };

            var titleLabel = new Label
            {
                Text = "Search",
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var searchBarContainer = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };

            var searchEntry = new Entry
            {
                Placeholder = "Search",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromArgb("#f0f0f0"),
                TextColor = Colors.Black,
                Margin = new Thickness(0, 0, 0, 20),
                HeightRequest = 40
            };

            var searchIcon = new Image
            {
                Source = "search_icon.png",
                WidthRequest = 25,
                HeightRequest = 25,
                VerticalOptions = LayoutOptions.Center
            };

            searchBarContainer.Children.Add(searchEntry);
            Grid.SetColumn(searchEntry, 0);
            Grid.SetRow(searchEntry, 0);

            searchBarContainer.Children.Add(searchIcon);
            Grid.SetColumn(searchIcon, 1);
            Grid.SetRow(searchIcon, 0);

            var tabStack = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 20,
                Margin = new Thickness(0, 0, 0, 20)
            };
        }

        private Label CreateTabLabel(string text, bool isSelected)
        {
            return new Label
            {
                Text = text,
                FontSize = 16,
                FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None,
                TextColor = isSelected ? Colors.Black : Colors.Gray
            };
        }

        private Grid CreateCategoryRow(string categoryName, bool isSale)
        {
            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                Padding = new Thickness(0, 5)
            };

            var categoryLabel = new Label
            {
                Text = categoryName,
                FontSize = 16,
                TextColor = isSale ? Colors.Red : Colors.Black,
                VerticalOptions = LayoutOptions.Center
            };

            var arrowIcon = new Label
            {
                Text = ">",
                FontSize = 16,
                TextColor = Colors.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            grid.Children.Add(categoryLabel);
            Grid.SetColumn(categoryLabel, 0);

            grid.Children.Add(arrowIcon);
            Grid.SetColumn(arrowIcon, 1);

            return grid;
        }
    }
}