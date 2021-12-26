using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SerialMIDIBus.converters
{
    /// <summary>
    /// Converter Base
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    public  class BaseDTConverter<T1,T2> :IValueConverter
    {
        protected Func<T1,T2> Convert;
        protected Func<T2, T1> ConvertBack;
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value">obj</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">obj</param>
        /// <param name="culture">culture</param>
        /// <returns></returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Convert!=null)?Convert((T1)value):value;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (ConvertBack != null) ? ConvertBack((T2)value) : value;
        }
    }
}
