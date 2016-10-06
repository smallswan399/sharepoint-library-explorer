using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Core.Libs
{
    public static class EnumHelper
    {

        public static IEnumerable<T> GetEnumListItems<T>(this T _enum)
            where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new TypeAccessException();
            }

            return Enum.GetValues(type).Cast<T>();
        }

        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }
    }
}
