using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace Givt.Donations.Persistence.Converters
{
    internal class AmountsConverter
    {
        public static ValueConverter<decimal[], string> GetConverter()
        {
            return new ValueConverter<decimal[], string>(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.None).Select(str => decimal.Parse(str, CultureInfo.InvariantCulture)).ToArray()
            );
        }

        public static ValueComparer<decimal[]> GetComparer()
        {
            return new ValueComparer<decimal[]>(true);
        }
    }
}
