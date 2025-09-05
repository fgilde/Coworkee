using System.Drawing;
using System.Linq;

namespace lib.Coworkee.Client.Utils
{
    public static class ColorUtils
    {
        public static string Random(string str)
        {
            var hash = str.Aggregate(0, (current, t) => t + ((current << 5) - current));
            return ToHex(Color.FromArgb(hash));
        }

        public static string ToHex(this System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static string ToRGB(this System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }
    }
}