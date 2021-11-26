using CleanArchitectureBase.Client.Enums;
using MudBlazor;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Theming
{

    public class ClientTheme : MudTheme
    {
        public new LayoutPropertiesEx LayoutProperties { get; set; }

        #region Statics

        #region Default Typography and Layout

        private static Typography DefaultTypography => new()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "6rem",
                FontWeight = 300,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "2.125rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em"
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 400,
                LineHeight = 1.6,
                LetterSpacing = ".0075em"
            },
            Button = new Button()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em"
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em"
            },
            Subtitle2 = new Subtitle2()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            }
        };

        private static LayoutPropertiesEx DefaultLayoutProperties => new()
        {
            DefaultBorderRadius = "3px"
        };

        #endregion

        public static ClientTheme LastUsedTheme { get; set; }

        public static ClientTheme DefaultTheme = new ClientTheme()
        {
            Palette = new Palette()
            {
                Primary = "#1E88E5",
                AppbarBackground = "#1E88E5",
                Background = Colors.Grey.Lighten5,
                DrawerBackground = "#FFF",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#007E33"
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        }.SetProperties(
            t => t.LayoutProperties.DrawerClipMode = DrawerClipMode.Always,
            t => t.LayoutProperties.DrawerVariant = DrawerVariant.Responsive);


        public static ClientTheme CodeBlue = new ClientTheme()
        {
            Palette = new Palette()
            {
                Primary = "#0082bb",
                AppbarBackground = "#0082bb",
                Secondary = "#ff8300",
                Background = Colors.Grey.Lighten5,
                DrawerBackground = "#FFF",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#128a00",
                Warning = "#ffdd00",
                Error = "#df1642"
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        }.SetProperties(
            t => t.LayoutProperties.DrawerClipMode = DrawerClipMode.Never,
            t => t.LayoutProperties.DrawerVariant = DrawerVariant.Temporary,
            t => t.LayoutProperties.ShowUserCardInNavigation = false,
            t => t.LayoutProperties.ShowLogoInAppBar = false,
            t => t.LayoutProperties.ShowLogoInNavMenu = true,
            t => t.LayoutProperties.AppBarTitleBehaviour = AppBarTitleBehaviour.TitleOnly,
            t => t.LayoutProperties.MenuTogglePosition = MenuTogglePosition.Start,
            t => t.LayoutProperties.NavMenuExpandMode = ExpandMode.SingleExpand);


        public static ClientTheme DarkTheme = new ClientTheme() {
            Palette = new Palette()
            {
                Primary = "#1E88E5",
                Success = "#007E33",
                Black = "#27272f",
                Background = "#32333d",
                BackgroundGrey = "#27272f",
                Surface = "#373740",
                DrawerBackground = "#27272f",
                DrawerText = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#373740",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DrawerIcon = "rgba(255,255,255, 0.50)"
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        }.SetProperties(
            t => t.LayoutProperties.DrawerClipMode = DrawerClipMode.Always,
            t => t.LayoutProperties.DrawerVariant = DrawerVariant.Responsive);


        #endregion
    }
}