using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookmarkManager.Libs.Tools
{
    public class ClipBoardManager<T> where T : class
    {
        //public static T GetFromClipboard()
        //{
        //    T retrievedObj = null;
        //    IDataObject dataObj = Clipboard.GetDataObject();
        //    string format = typeof(T).FullName;
        //    if (dataObj.GetDataPresent(format))
        //    {
        //        retrievedObj = dataObj.GetData(format) as T;
        //    }
        //    return retrievedObj;
        //}

        //public static void CopyToClipboard(T objectToCopy)
        //{
        //    // register my custom data format with Windows or get it if it's already registered
        //    DataFormats..Format format = DataFormats.GetFormat(typeof(T).FullName);

        //    // now copy to clipboard
        //    IDataObject dataObj = new DataObject();
        //    dataObj.SetData(format.Name, false, objectToCopy);
        //    Clipboard.SetDataObject(dataObj, false);
        //}

        //public static void Clear()
        //{
        //    Clipboard.Clear();
        //}
    }
}
