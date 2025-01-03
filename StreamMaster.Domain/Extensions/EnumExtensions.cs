namespace StreamMaster.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static SMFileTypes GetSMFileTypEnumByValue(this int value, SMFileTypes defaultValue = SMFileTypes.Logo)
        {
            return Enum.IsDefined(typeof(SMFileTypes), value)
                ? (SMFileTypes)value
                : defaultValue;
        }
    }
}
