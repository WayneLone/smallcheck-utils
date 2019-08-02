using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Smallcheck.Utils.Common
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        #region 获取枚举类描述信息 +static string GetDescription(Enum enumValue)
        /// <summary>
        /// 获取枚举类描述信息
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(Enum enumValue)
        {
            try
            {
                Type enumType = enumValue.GetType();
                FieldInfo enumFieldInfo = enumType.GetField(Enum.GetName(enumType, enumValue));
                if (Attribute.GetCustomAttribute(enumFieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute descAttribute)
                {
                    return descAttribute.Description;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获得枚举描述信息和对应值 +static Dictionary<string, int> GetEnumDic(Type enumType)
        /// <summary>
        /// 获得枚举描述信息和对应值
        /// 获取枚举的Description特性作为描述信息
        /// </summary>
        /// <param name="enumType">枚举类型 k-枚举描述 v-枚举值</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetEnumDic(Type enumType)
        {
            try
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (var item in Enum.GetValues(enumType))
                {
                    FieldInfo enumFieldInfo = enumType.GetField(Enum.GetName(enumType, item));
                    if (Attribute.GetCustomAttribute(enumFieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute descAttribute)
                    {
                        dic.Add(descAttribute.Description, Convert.ToInt32(item));
                    }
                }
                return dic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取特定值的多个标志位枚举 +static Dictionary<int, string> GetFlagEnumDic(Enum enumValue)
        /// <summary>
        /// 获取特定值的多个标志位枚举
        /// </summary>
        /// <param name="enumValue">Flag枚举值</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetFlagEnumDic(Enum enumValue)
        {
            try
            {
                Dictionary<int, string> resDic = new Dictionary<int, string>();
                Type enumType = enumValue.GetType();
                if (enumType.IsDefined(typeof(FlagsAttribute)))
                {
                    foreach (var item in Enum.GetValues(enumType))
                    {
                        if (enumValue.HasFlag((Enum)item))
                        {
                            FieldInfo enumFieldInfo = enumType.GetField(Enum.GetName(enumType, item));
                            if (Attribute.GetCustomAttribute(enumFieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute descAttribute)
                            {
                                resDic.Add(Convert.ToInt32(item), descAttribute.Description);
                            }
                            else
                            {
                                resDic.Add(Convert.ToInt32(item), "未知");
                            }
                        }
                    }
                }
                return resDic;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
