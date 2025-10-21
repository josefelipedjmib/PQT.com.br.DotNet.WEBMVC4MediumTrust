using System;
using System.ComponentModel;
using System.Reflection;
using Auxiliar.DataAnnotationCustoms;

namespace Auxiliar.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo[] memInfo = type.GetMember(value.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return value.ToString();
        }

        public static bool GetEnumAtivoValue(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo[] memInfo = type.GetMember(value.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(AtivoAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((AtivoAttribute)attrs[0]).Ativo;
                }
            }
            return false; 
        }
    }
}
