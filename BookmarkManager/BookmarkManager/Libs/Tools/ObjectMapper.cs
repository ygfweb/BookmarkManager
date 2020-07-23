using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BookmarkManager.Libs.Tools
{
    /// <summary>
    /// 对象映射
    /// </summary>
    public static class ObjectMapper
    {
        private static void _SetValue(Type sourceType, Type targetType, object sourceObj, object targetObject)
        {
            List<PropertyInfo> targetPropertyList = targetType.GetProperties().Where(x => x.PropertyType.IsPublic && x.CanWrite).ToList();
            foreach (PropertyInfo targetProperty in targetPropertyList)
            {
                PropertyInfo sourceProperty = sourceType.GetProperty(targetProperty.Name, BindingFlags.Public | BindingFlags.Instance);
                if (sourceProperty != null && sourceProperty.CanRead && sourceProperty.PropertyType == targetProperty.PropertyType)
                {
                    object value = sourceProperty.GetValue(sourceObj, null);
                    targetProperty.SetValue(targetObject, value, null);
                }
            }
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        public static TTarget Map<TSource, TTarget>(TSource source) where TSource : class where TTarget : class, new()
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                Type sourceType = typeof(TSource);
                Type targetType = typeof(TTarget);
                TTarget target = new TTarget();
                _SetValue(sourceType, targetType, source, target);
                return target;
            }
        }

        /// <summary>
        /// 映射列表
        /// </summary>
        public static List<TTarget> MapList<TSource, TTarget>(List<TSource> sourceList) where TSource : class where TTarget : class, new()
        {
            List<TTarget> targets = new List<TTarget>();
            if (sourceList != null && sourceList.Count > 0)
            {
                Type sourceType = typeof(TSource);
                Type targetType = typeof(TTarget);
                foreach (TSource source in sourceList)
                {
                    TTarget target = new TTarget();
                    _SetValue(sourceType, targetType, source, target);
                    targets.Add(target);
                }
            }
            return targets;
        }

        /// <summary>
        /// 对象之间拷贝值
        /// </summary>
        public static void CopyValues<TSource, TTarget>(TSource source, TTarget target) where TSource : class where TTarget : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            _SetValue(sourceType, targetType, source, target);
        }
    }
}
