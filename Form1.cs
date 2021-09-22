using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;

namespace Laba1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        double dichotomy(double a, double b, double eps)
        {
            double x;
            while (b-a > eps)
            {
                x = a + (b - a) / 2;
                if 
            }
            return min;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Function f = new Function("f(x) = log(5, 4*x - 3) - ln(2*x) - log10(3*x)");
            string sklt = "f()";
            string a = sklt.Insert(2, textBox2.Text);
            string b = sklt.Insert(2, textBox3.Text);
            Expression exp = new Expression(a, f);
            Expression exp2 = new Expression(b, f);
            label2.Text = Convert.ToString(exp.calculate());
            double eps = 0.001
        }

    }
}
