using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Configuration;
using System.Linq;

namespace SDT.Helpers
{
    class Settings
    {
        /// <summary>
        /// Load settings
        /// </summary>
        public Settings()
        {
            bool isDark = Properties.Settings.Default.Theme;
            string isSwatch = Properties.Settings.Default.Accent;

            var swatchesProvider = new SwatchesProvider();
            var changeSwatch = swatchesProvider.Swatches.Single(
                swatch => swatch.Name == isSwatch);

            new PaletteHelper().ReplacePrimaryColor(changeSwatch);

            new PaletteHelper().SetLightDark(isDark);
        }   
    }
}
