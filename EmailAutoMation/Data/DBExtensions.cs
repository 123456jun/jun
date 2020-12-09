using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RobotForeignWorkflow
{
    public static class DBExtensions
    {
        /// <summary>
        /// 将IDataReader的数据转换为目标的实体类集合
        /// </summary>
        /// <typeparam name="T">目标的实体类型</typeparam>
        /// <param name="dr">数据源</param>
        /// <returns>目标的实体类对象集合</returns>
        public static List<T> ToList<T>(this IDataReader dr) where T : new()
        {
            // 定义集合  
            List<T> ts = new List<T>();
            using (dr)
            {

                //遍历IDataReader中所有的数据行  
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        T t = newT<T>(dr);
                        //对象添加到泛型集合中  
                        ts.Add(t);
                    }
                }
            }
            return ts;
        }
        /// <summary>
        /// 将IDataReader的数据转换为目标的实体类
        /// </summary>
        /// <typeparam name="T">目标的实体类型</typeparam>
        /// <param name="dr">数据源</param>
        /// <returns>目标的实体类对象</returns>
        public static T ToInstance<T>(this IDataReader dr) where T : new()
        {
            using (dr)
            {
                
                //遍历IDataReader中所有的数据行  
                if (dr != null)
                {
                    if (dr.Read())
                    {
                      
                        return newT<T>(dr);
                    }
                }
            }
            return default(T);
        }
        /// <summary>
        /// 将IDataReader的数据转换为目标的实体类
        /// </summary>
        /// <typeparam name="T">目标的实体类型</typeparam>
        /// <param name="dr">数据源</param>
        /// <returns>目标的实体类对象</returns>
        private static T newT<T>(IDataReader dr) where T : new()
        {
            T t = new T();

            // 获得此模型的所有属性  
            PropertyInfo[] properties = t.GetType().GetProperties();
            PropertyInfo prop;

            // 获得此模型的所有字段  
            FieldInfo[] fields = t.GetType().GetFields();
            FieldInfo field;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                //取值  
                object value = dr.GetValue(i);
                //找到字段对应的的属性  
                if (properties.Any(p => string.Equals(p.Name, dr.GetName(i), StringComparison.OrdinalIgnoreCase)))
                {
                    prop = properties.First(p => string.Equals(p.Name, dr.GetName(i), StringComparison.OrdinalIgnoreCase));
                    if (!prop.CanWrite)//该属性不可写，直接跳出  
                    {
                        continue;
                    }

                    //如果非空，则赋给对象的属性  
                    if (value != DBNull.Value)
                    {
                        setPropertyValue(prop, t, value);
                    }
                }
                else if (fields.Any(f => string.Equals(f.Name, dr.GetName(i), StringComparison.OrdinalIgnoreCase)))
                {
                    field = fields.First(f => string.Equals(f.Name, dr.GetName(i), StringComparison.OrdinalIgnoreCase));

                    //如果非空，则赋给对象的属性  
                    if (value != DBNull.Value)
                    {
                        setFieldValue(field, t, value);
                    }
                }
            }
            return t;
        }
        /// <summary>
        /// 给属性赋值
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        private static void setPropertyValue(PropertyInfo prop, object obj, object value)
        {
            if (prop == null)
            {
                return;
            }
            if (prop.PropertyType.IsEnum) //转换为枚举，支持数字和文字
            {
                prop.SetValue(obj, Enum.Parse(prop.PropertyType, value.ToString(), true), null);
                return;
            }
            switch (prop.PropertyType.FullName)
            {
                case "System.Int64":
                    prop.SetValue(obj, Convert.ToInt64(value), null);
                    break;
                case "System.Int32":
                    prop.SetValue(obj, Convert.ToInt32(value), null);
                    break;
                case "System.Int16":
                    prop.SetValue(obj, Convert.ToInt16(value), null);
                    break;
                case "System.Boolean":
                    prop.SetValue(obj, ValueAsBoolean(value, false), null);
                    break;
                case "System.String":
                    prop.SetValue(obj, value.ToString(), null);
                    break;
                default:
                    prop.SetValue(obj, value, null);
                    break;
            }
        }
        /// <summary>
        /// 给字段赋值
        /// </summary>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        private static void setFieldValue(FieldInfo field, object obj, object value)
        {
            if (field == null)
            {
                return;
            }

            if (field.FieldType.IsEnum) //转换为枚举，支持数字和文字
            {
                field.SetValue(obj, Enum.Parse(field.FieldType, value.ToString(), true));
                return;
            }

            switch (field.FieldType.FullName)
            {
                case "System.Int64":
                    field.SetValue(obj, Convert.ToInt64(value));
                    break;
                case "System.Int32":
                    field.SetValue(obj, Convert.ToInt32(value));
                    break;
                case "System.ToInt16":
                    field.SetValue(obj, Convert.ToInt16(value));
                    break;
                case "System.Boolean":
                    field.SetValue(obj, ValueAsBoolean(value, false));
                    break;
                case "System.String":
                    field.SetValue(obj, value.ToString());
                    break;
                default:
                    field.SetValue(obj, value);
                    break;
            }
        }

        /// <summary>
        /// 将值转化为布尔值
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        private static bool ValueAsBoolean(object value, bool defaultValue)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            //先用默认的规则
            switch (value.ToString().ToLower())
            {
                case "是":
                case "1":
                case "true":
                case "yes":
                case "y":
                case "+":
                    return true;
                case "否":
                case "0":
                case "false":
                case "no":
                case "n":
                case "-":
                    return false;
            }
            //默认的规则不包含则使用系统的规则
            if (bool.TryParse(value.ToString(), out bool i))
            {
                return i;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
