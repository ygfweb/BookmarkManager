using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleWrapper wrapper = new SingleWrapper();
            wrapper.Run(args);
        }
    }
}
