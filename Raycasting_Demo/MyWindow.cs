using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raycasting_Demo
{
    public partial class MyWindow : Form
    {
        Raycaster rc;
        public MyWindow()
        {
            InitializeComponent();
            rc = new Raycaster();
        }

        private void Panel_OnPaint(object sender, PaintEventArgs e)
        {
            var g = panel.CreateGraphics();
            g.DrawImage(rc.NewFrame(panel.Width, panel.Height), 0, 0);
        }
    }
}
