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
    public partial class Randomize : Form
    {
        public Randomize()
        {
            InitializeComponent();
        }

        // Property to change seed
         public decimal Seed
        {
            get
            {
                return numericUpDownSeed.Value;
            }

            set
            {
                numericUpDownSeed.Value = value;
            }
        }
    }
}
