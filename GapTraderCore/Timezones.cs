using System;

namespace GapTraderCore
{
    public enum Timezone
    {
        Uk,
        Us
    }

    public static class TimeServices
    {
        public static (TimeSpan, TimeSpan) GetOpenCloseTimes(Timezone timezone)
        {
            switch (timezone)
            {
                case Timezone.Uk:
                    return (new TimeSpan(8, 00, 00), new TimeSpan(16, 30, 00));
                case Timezone.Us:
                    return (new TimeSpan(14, 30, 00), new TimeSpan(21, 00, 00));
                default:
                    throw new ArgumentOutOfRangeException(nameof(timezone), timezone, null);
            }
        }
    }
}