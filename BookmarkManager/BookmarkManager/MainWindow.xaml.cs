using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using SiHan.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookmarkManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindowViewModel MainWindowViewModel = new MainWindowViewModel();
        private NotifyIcon notifyIcon = new NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            this.StateChanged += MainWindow_StateChanged;
            this.tb_file.Text = "文件路径：" + GlobalVariables.DbFile;
            this.MainWindowViewModel.DashboardViewModel = this.dashboard.ViewModel;
            this.MainWindowViewModel.View = this;
            this.DataContext = this.MainWindowViewModel;

            //图标
            // https://blog.csdn.net/u014234260/article/details/73648649
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.Visible = true;
            notifyIcon.Text = GlobalVariables.AppName;
            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new[]
            {
                new System.Windows.Forms.MenuItem("打开窗体", (sender, args) =>
                {
                    Visibility = System.Windows.Visibility.Visible;
                    ShowInTaskbar = true;
                    Activate();
                }),
                new System.Windows.Forms.MenuItem("退出程序", (sender, args) =>
                {
                    System.Windows.Application.Current.Shutdown();
                })
            });
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.ShowInTaskbar = true;
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.dashboard.LoadData();
        }

        private void RibbonWindow_ContentRendered(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
