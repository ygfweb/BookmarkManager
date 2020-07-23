using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookmarkManager.Libs.Commands
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class BaseCommand: ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected virtual void DoExecute() { }

        protected virtual bool DoCanExecute() { return true; }

        public bool CanExecute(object parameter)
        {
            return this.DoCanExecute();
        }

        public void Execute(object parameter)
        {
            this.DoExecute();
        }
    }
}
