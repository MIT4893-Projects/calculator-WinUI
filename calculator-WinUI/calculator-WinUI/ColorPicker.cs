using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace calculator_WinUI
{
    public abstract class ColorPicker
    {
        public static SolidColorBrush BlackOrWhiteTextColorBasedOnBackground(Color backgroundBrush)
        {
            return new SolidColorBrush(
                ((backgroundBrush.R + backgroundBrush.B + backgroundBrush.G) / 3) <= 128 ? Colors.White : Colors.Black
            );
        }
    }
}
