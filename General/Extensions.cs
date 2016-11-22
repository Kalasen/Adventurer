using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer
{
    public static class Extensions
    {
        //TODO: Consider using ColorTranslator.FromHTML instead. Requires hex prefix, while current code doesn't.
        public static Color ToColorFromHex(this string hex)
        {
            return Color.FromArgb(Convert.ToInt32(int.Parse(hex, System.Globalization.NumberStyles.HexNumber)));
        }
    }
}
