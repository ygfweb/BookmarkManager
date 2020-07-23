using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookmarkManager.Libs.Commands
{
    public static class AppCommands
    {
        /// <summary>
        /// 增加书签命令
        /// </summary>
        public static RoutedUICommand CreateBookmark { get; private set; }
        static AppCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl+A"));
            CreateBookmark = new RoutedUICommand("增加书签", "CreateBookmark", typeof(AppCommands), inputs);
        }
    }
}
