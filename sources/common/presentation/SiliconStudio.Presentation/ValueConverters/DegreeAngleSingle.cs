using System;
using System.Globalization;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Presentation.ValueConverters
{
    public class DegreeAngleSingle : ValueConverterBase<DegreeAngleSingle>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var angle = (AngleSingle)value;
            return angle.Degrees;
        }

        /// <inheritdoc/>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var degree = (double)value;
            return new AngleSingle((float)degree, AngleType.Degree);
        }
    }
}
