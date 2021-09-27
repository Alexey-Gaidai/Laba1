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

        private double f(double x)
        {
            Function f = new Function("f(x) = " + textBox1.Text);
            string sklt = "f()";
            string fx = sklt.Insert(2, x.ToString());
            fx = fx.Replace(",", ".");
            Expression fxx = new Expression(fx, f);
            return fxx.calculate();
        }

        private double dichotomy(double a, double b, double e)
        {
            double x;
            while (Math.Abs(b-a) > e)
            {
                x = (a + b) / 2;
                if (f(x) > f(a))
                    b = x;
                else
                    a = x;
            }
            x = (a + b) / 2;
            chart1.Series[1].Points.AddXY(x, f(x));
            return Math.Round(x, 3);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Function func = new Function("f(x)="+textBox1.Text);
            string sklt = "f()";
            string a = sklt.Insert(2, textBox2.Text);
            string b = sklt.Insert(2, textBox3.Text);
            Expression exp = new Expression(a, func);
            Expression exp2 = new Expression(b, func);
            label2.Text = Convert.ToString(exp.calculate());

            

            double Xmin = double.Parse(textBox2.Text);
            double Xmax = double.Parse(textBox3.Text);
            double Step = 1;

            double result = dichotomy(Xmin, Xmax, 0.001);
            
            label2.Text = Math.Round(f(result), 5).ToString();
            

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
                Expression chartt = new Expression(eeee, func);

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
