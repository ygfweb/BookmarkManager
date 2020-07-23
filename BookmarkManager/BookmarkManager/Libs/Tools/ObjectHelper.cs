using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BookmarkManager.Libs.Tools
{
    public static class ObjectHelper
    {
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        public static bool IsNull(object obj)
        {
            return obj == null || obj == DBNull.Value || StringHelper.IsNullOrWhiteSpace(obj.ToString());
        }
        /// <summary>
        /// 将图片转换二进制数据
        /// </summary>
        public static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 判断对象是否可以被序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsSerializable(object obj)
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            BinaryFormatter bin = new BinaryFormatter();
            try
            {
                bin.Serialize(mem, obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
