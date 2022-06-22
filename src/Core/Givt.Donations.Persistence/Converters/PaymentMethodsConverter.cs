using Givt.Donations.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Givt.Donations.Persistence.Converters
{
    internal class PaymentMethodsConverter
    {
        public static ValueConverter<IEnumerable<PaymentMethod>, UInt64> GetConverter()
        {
            return new ValueConverter<IEnumerable<PaymentMethod>, UInt64>(
                list => GetBitmappedPaymentMethods(list),
                bitmap => GetListPaymentMethods(bitmap)
            );
        }

        public static ValueComparer<IEnumerable<PaymentMethod>> GetComparer()
        {
            return new ValueComparer<IEnumerable<PaymentMethod>>(
                (c1, c2) => c1.OrderBy(pm => pm).SequenceEqual(c2.OrderBy(pm => pm)),
                c => c.OrderBy(pm => pm).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.OrderBy(pm => pm).ToList());
        }

        private static UInt64 GetBitmappedPaymentMethods(IEnumerable<PaymentMethod> list)
        {
            UInt64 result = 0;
            foreach (var paymentMethod in list)
                result |= (UInt64)0x1 << (byte)paymentMethod;

            return result;
        }

        private static IEnumerable<PaymentMethod> GetListPaymentMethods(UInt64 bitmap)
        {
            var result = new List<PaymentMethod>();
            UInt64 mask = 0x1;
            for (int i = 0; i < sizeof(UInt64) * 8; i++)
            {
                if ((bitmap & mask) != 0) { result.Add((PaymentMethod)i); }
                mask <<= 1;
            }
            return result;
        }

    }
}
