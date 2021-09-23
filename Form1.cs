using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        private void dichotomy()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Function f = new Function("f(x)="+textBox1.Text);
            string sklt = "f()";
            string a = sklt.Insert(2, textBox2.Text);
            string b = sklt.Insert(2, textBox3.Text);
            Expression exp = new Expression(a, f);
            Expression exp2 = new Expression(b, f);
            label2.Text = Convert.ToString(exp.calculate());

            double Xmin = double.Parse(textBox2.Text);
            double Xmax = double.Parse(textBox3.Text);
            double Step = 0.1;
            
            // Количество точек графика
            int count = (int)Math.Ceiling((Xmax - Xmin) / Step) + 1;

            // Массив значений X – общий для обоих графиков
            double[] x = new double[count];

            // Два массива Y – по одному для каждого графика
            double[] y1 = new double[count];

            // Расчитываем точки для графиков функции
            for (int i = 0; i < count; i++)
            {
                // Вычисляем значение X
                x[i] = Xmin + Step * i;

                // Вычисляем значение функций в точке X
                string d = sklt.Insert(2, Convert.ToString(x[i]));
                string eeee = d.Replace(",", ".");
                Expression chartt = new Expression(eeee, f);

                y1[i] = chartt.calculate();

            }
            
            // Настраиваем оси графика
            chart1.ChartAreas[0].AxisX.Minimum = Xmin;
            chart1.ChartAreas[0].AxisX.Maximum = Xmax;

            // Определяем шаг сеткn
            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = Step;

            // Добавляем вычисленные значения в графики
            chart1.Series[0].Points.DataBindXY(x, y1);
        }
    }
}
