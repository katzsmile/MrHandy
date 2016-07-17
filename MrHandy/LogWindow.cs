using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MrHandy
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public void Log(string LogText)
        {
            txtLog.AppendText(LogText + "\n");
        }
        public void Clear()
        {
            txtLog.Clear();
        }
    }
}
