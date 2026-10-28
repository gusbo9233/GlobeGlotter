
using MudBlazor;

namespace Web.Theme;

public static class GlosifyTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteDark = new PaletteDark
        {
            // Brand colors
            Primary = "#53e076",
            PrimaryContrastText = "#003914",

            Secondary = "#78b7ff",
            SecondaryContrastText = "#041329",

            Success = "#1db954",
            SuccessContrastText = "#003914",

            Info = "#78b7ff",

            // Backgrounds
            Background = "#041329",
            BackgroundGray = "#0d1c32",
            Surface = "#112036",

            // Navigation
            AppbarBackground = "#0d1c32",
            AppbarText = "#d6e3ff",

            DrawerBackground = "#0d1c32",
            DrawerText = "#d6e3ff",
            DrawerIcon = "#bccbb9",

            // Text
            TextPrimary = "#d6e3ff",
            TextSecondary = "#bccbb9",
            TextDisabled = "#869585",

            // Borders
            LinesDefault = "#3d4a3d",
            LinesInputs = "#869585",
            Divider = "#3d4a3d",

            // Icons
            ActionDefault = "#bccbb9"
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily =
                [
                    "Plus Jakarta Sans",
                    "system-ui",
                    "sans-serif"
                ]
            }
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        }
    };
}
