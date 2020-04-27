using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Of_Life
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        // Property to change the time
        public decimal Time
        {
           
            get
            {
                return numericUpDownTime.Value;
            }

            set
            {
                numericUpDownTime.Value = value;
            }
        }

        // Property to set Height
        public decimal GridHeight
        {
            get
            {
                return numericUpDownHeight.Value;
            }

            set
            {
                numericUpDownHeight.Value = value;
            }
        }

        // Property to set the width
        public decimal GridWidth
        {
            get
            {
                return numericUpDownWidth.Value;
            }

            set
            {
                numericUpDownWidth.Value = value;
            }
        }
    }
}
