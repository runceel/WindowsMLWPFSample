using Microsoft.Toolkit.Win32.UI.XamlHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsMLWPFSample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (new XamlApplication())
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
