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

namespace ClientAutoStart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();

            _trayIcon = new NotifyIcon();
            // Get an Hicon for myBitmap.
            IntPtr Hicon =  ClientAutoStart.Resources.call_start.GetHicon();

            // Create a new icon from the handle. 
            System.Drawing.Icon newIcon = System.Drawing.Icon.FromHandle(Hicon);

            _trayIcon.Icon = newIcon;
            _trayIcon.Text = "Client Auto Start";

            _trayIcon.Click += OnTrayIconClick;
            _trayIcon.Visible = true;
        }

        void OnTrayIconClick(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
