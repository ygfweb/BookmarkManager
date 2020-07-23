using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BookmarkManager.Libs.Tools
{
    /// <summary>
    ///     Xml helper class
    /// </summary>
    public static class XmlHelper
    {
        #region 序列化

        /// <summary>
        ///  XML Serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, Encoding encoding = null)
        {
            return Serialize(obj, typeof(T), encoding);
        }

        /// <summary>
        ///     XML Serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Serialize(object obj, Type type, Encoding encoding = null)
        {
            return Serialize(obj, type, encoding, null);
        }

        /// <summary>
        ///     XML Serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static string Serialize(object obj, Type type, Encoding encoding, params Type[] types)
        {
            var serializer = new XmlSerializer(type, types);
            var sb = new StringBuilder();

            using (var writer = new EncodingStringWriter(sb, encoding))
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                serializer.Serialize(writer, obj, namespaces);
                return sb.ToString();
            }
        }

        /// <summary>
        ///     XML Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
        {
            return Deserialize<T>(xml, typeof(T));
        }

        /// <summary>
        ///     XML Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, Type type)
        {
            return Deserialize<T>(xml, type, null);
        }

        /// <summary>
        ///     XML Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, Type type, params Type[] types)
        {
            var serializer = new XmlSerializer(type, types);
            using (TextReader reader = new StringReader(xml))
            {
                var obj = (T)serializer.Deserialize(reader);
                return obj;
            }
        }

        #endregion 序列化
    }
}
