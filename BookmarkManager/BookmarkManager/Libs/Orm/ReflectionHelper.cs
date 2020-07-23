using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BookmarkManager.Libs.Orm
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        public static string GenerateInsertql<T>() where T:class,new()
        {
            Type type = typeof(T);
            StringBuilder sb = new StringBuilder();
            List<PropertyInfo> pList = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            sb.Append($"INSERT INTO {type.Name} ");
            sb.Append("(");
            StringBuilder cols = new StringBuilder();
            foreach (var item in pList)
            {
                cols.Append($"[{item.Name}],");
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(") ");
            sb.Append("VALUES (");
            cols = new StringBuilder();
            foreach (var item in pList)
            {
                cols.Append($"@{item.Name},");
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(");");
            return sb.ToString();
        }

        public static string GenerateUpdateSql<T>() where T : class, new()
        {
            Type type = typeof(T);
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE {type.Name} SET ");
            List<PropertyInfo> infos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            StringBuilder cols = new StringBuilder();
            foreach (var item in infos)
            {
                if (!string.Equals(item.Name,"id",StringComparison.OrdinalIgnoreCase))
                {
                    cols.Append($"[{item.Name}]=@{item.Name},");
                }               
            }
            sb.Append(cols.ToString().Trim(','));
            sb.Append(" WHERE ");
            sb.Append("Id = @Id;");
            return sb.ToString();
        }

        public static string GenerateDeleteSql<T>() where T : class, new()
        {
            Type type = typeof(T);
            return $"DELETE FROM [{type.Name}] WHERE [Id] = @Id; ";
        }

        /// <summary>
        /// 为命令生成参数
        /// </summary>
        public static void GenerateParameters<T>(DbCommand command,T obj) where T : class, new()
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.Parameters.Clear();
            Type type = typeof(T);
            foreach (var item in type.GetProperties())
            {
                DbParameter parameter = command.CreateParameter();
                parameter.ParameterName = "@" + item.Name;
                object value = item.GetValue(obj, null);
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.Value = value;
                }
                command.Parameters.Add(parameter);
            }
        }
        /// <summary>
        /// 为命令生成参数
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="paramObject">匿名对象</param>
        public static void GenerateParameters(DbCommand command,object paramObject)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            if (paramObject != null)
            {
                command.Parameters.Clear();
                foreach (PropertyInfo pInfo in paramObject.GetType().GetProperties())
                {
                    var parameter = command.CreateParameter();
                    string name = pInfo.Name;
                    object value = pInfo.GetValue(paramObject, null);
                    parameter.ParameterName = "@" + name;
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = value;
                    }
                    command.Parameters.Add(parameter);
                }
            }           
        }

        /// <summary>
        /// 将DbDataReader转换成List
        /// </summary>
        public static List<T> ConvertToList<T>(DbDataReader reader) where T : class, new()
        {          
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            List<T> list = new List<T>();
            Type type = typeof(T);
            List<PropertyInfo> properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo p in properties)
                {
                    object value = reader[p.Name];
                    if (value == DBNull.Value)
                    {
                        p.SetValue(obj, null, null);
                    }
                    else
                    {
                        p.SetValue(obj, value, null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}

