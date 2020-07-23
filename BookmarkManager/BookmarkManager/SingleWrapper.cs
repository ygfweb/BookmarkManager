using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager
{
    class SingleWrapper: WindowsFormsApplicationBase
    {
        private App App { get; set; }
        public SingleWrapper()
        {
            this.IsSingleInstance = true;
        }
        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            this.App = new App();
            this.App.Run();
            return false;
        }
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            if (!App.MainWindow.IsVisible)
            {
                App.MainWindow.Show();
            }
            if (App.MainWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                App.MainWindow.WindowState = System.Windows.WindowState.Normal;
            }
            App.MainWindow.Activate();
        }
    }
}