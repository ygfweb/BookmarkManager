using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookmarkManager.Libs.Commands
{
    /// <summary>
    /// 异步命令基类
    /// </summary>
    public abstract class BaseAsyncCommand : ICommand
    {
        /// <summary>
        /// 是否正在运行中
        /// </summary>
        public bool IsExecuting { get; private set; }
        /// <summary>
        /// 之前运行，一般用于显示进度条、操作确认对话框，返回值为true表示继续操作，否则取消操作
        /// </summary>
        protected virtual bool DoBefore() { return true; }

        /// <summary>
        /// 之后运行，一般用于关闭进度条
        /// </summary>
        protected virtual void DoAfter() { }

        /// <summary>
        /// 执行实际运行的代码
        /// </summary>
        protected abstract Task DoExecute();
        /// <summary>
        /// 执行完成的异常处理
        /// </summary>
        protected virtual void DoError(Exception ex) 
        {
            MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// 执行完成的正常处理
        /// </summary>
        protected virtual void DoSuccess() { }
        /// <summary>
        /// 检查命令是否可执行
        /// </summary>
        protected virtual bool DoCanExecute(object param)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return (!this.IsExecuting && this.DoCanExecute(parameter));
        }

        public async void Execute(object parameter)
        {
            try
            {              
                if (this.DoBefore())
                {
                    this.IsExecuting = true;
                    try
                    {
                        await this.DoExecute();
                        this.IsExecuting = false;
                        this.DoSuccess();
                    }
                    catch (Exception ex)
                    {
                        this.IsExecuting = false;
                        this.DoError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this.DoError(ex);
            }
        }
    }
}
