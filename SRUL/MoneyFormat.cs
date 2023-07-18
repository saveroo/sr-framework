using System;
using System.Globalization;

namespace SRUL
{
public class MoneyFormat: IFormatProvider,  ICustomFormatter
        {
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                else
                    return null;
            }

            public string Format(string fmt, object arg, IFormatProvider formatProvider)
            {
                if (arg.GetType() != typeof(decimal))
                    try
                    {
                        return HandleOtherFormats(fmt, arg);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(string.Format("The format of '{0}' is invalid", fmt), e);
                    }

                string ufmt = fmt.ToUpper(CultureInfo.InvariantCulture);
                if (!(ufmt == "M"))
                    try
                    {
                        return HandleOtherFormats(fmt, arg);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(string.Format("The format of '{0}' is invalid", fmt), e);
                    }

                decimal result;
                if (decimal.TryParse(arg.ToString(), out result))
                {
                    if (result >= 1000000)
                    {
                        decimal d = Math.Round(result / 1000000, 0);

                        CultureInfo clone = (CultureInfo) new CultureInfo("en-US").Clone();
                        string oldCurrSymbol = clone.NumberFormat.CurrencySymbol;
                        clone.NumberFormat.CurrencySymbol = "";

                        return String.Format(clone, oldCurrSymbol + " {0:C0}", d).Trim() + " M";
                    }
                }
                else
                    return string.Format("{0:C0}", result) + " M";

                return string.Format(new CultureInfo("en-US"), "{0:C0}", result);
            }

            private string HandleOtherFormats(string format, object arg)
            {
                if (arg is IFormattable)
                    return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
                else if (arg != null)
                    return arg.ToString();
                else
                    return string.Empty;
            }
    }
}