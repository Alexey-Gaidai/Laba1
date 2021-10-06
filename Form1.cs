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
using System.Threading;

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

        private double f(double x)//вынес подставления значения в функцию в отдельный метод
        {
            Function f = new Function("f(x) = " + textBox1.Text);
            string sklt = "f()";
            string fx = sklt.Insert(2, x.ToString());
            fx = fx.Replace(",", ".");
            Expression fxx = new Expression(fx, f);
            return fxx.calculate();
        }
        
        async Task<Double> method(double a, double b, double e)
        {
             return await Task.Run(() => mingold(a, b, e)); 
            
        }

        public static List<point> steps = new List<point>();

        private double mingold(double a, double b, double e)//метод дихотомии
        {
            double d = (-1 + Math.Sqrt(5)) / 2;
            double x1, x2;
            while (true)
            {
                x1 = b - (b - a) * d;
                x2 = a + (b - a) * d;
                if (f(x1) >= f(x2))
                {
                    a = x1;
                    point p = new point(a, f(a));
                    steps.Add(p);
                }
                else
                {
                    b = x2;
                    point p = new point(b, f(b));
                    steps.Add(p);
                }
                if (Math.Abs(b - a) < e)
                    break;
            }

            return (a + b) / 2;
        }

        private void graph(double min, double max, double step)
        {
            int count = (int)Math.Ceiling((max - min) / step) + 1;

            double[] x = new double[count];
            double[] y1 = new double[count];

            for (int i = 0; i < count; i++)
            {
                x[i] = min + step * i;
                y1[i] = f(x[i]);
            }

            chart1.ChartAreas[0].AxisX.Minimum = min;
            chart1.ChartAreas[0].AxisX.Maximum = max;

            chart1.ChartAreas[0].AxisX.MajorGrid.Interval = step;

            chart1.Series[0].Points.DataBindXY(x, y1);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[1].Points.Clear();
            chart1.Update();
            steps.Clear();
            if (Double.IsNaN(f(1)) == true)
            {
                DialogResult err = MessageBox.Show("Функция введена неверно!!!\nНажмите ОЧИСТИТЬ и повторите поптыку!", "Ошибка!");
            }
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")//проверка на заполненность данных
            {
                DialogResult err = MessageBox.Show("Введите все данные!!!", "Ошибка!");
            }
            else
            {
                try
                {
                    double Xmin = double.Parse(textBox2.Text);
                    double Xmax = double.Parse(textBox3.Text);
                    double eps = double.Parse(textBox4.Text);
                    double Step = 1;
                    double result = 0;

                    result = await method(Xmin, Xmax, eps);
                    
                    chart1.Series[1].Points.AddXY(result, f(result));

                    label2.Text = Math.Round(f(result), 5).ToString();

                    if (Double.IsNaN(result) is true)
                    {
                        DialogResult err = MessageBox.Show("Формула введена неверно!", "Ошибка!");
                    }

                    graph(Xmin, Xmax, Step);

                }
                catch (System.FormatException)
                {
                    DialogResult err = MessageBox.Show("Значения введены неверно!\nПроверьте корректность данных", "Ошибка!");
                }
            }

            counter = 0;
            //chart1.Series[2].Points.AddXY(steps[counter-1].x, steps[counter-1].y);            
            label7.Text = "Шаг: " + +counter+"/"+steps.Count.ToString();
        }

       private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Update();
            label2.Text = "";
        }
        public int counter;

        private void button3_Click(object sender, EventArgs e)
        {
            if (counter < steps.Count)
            {
                if (chart1.Series[2].Points.Count >= 1)
                {
                    chart1.Series[2].Points.RemoveAt(chart1.Series[2].Points.Count - 1);
                    
                }
                else
                {
                    chart1.Series[2].Points.AddXY(steps[counter].x, steps[counter].y);
                    counter++;
                    label7.Text = "Шаг: " + +counter + "/" + steps.Count.ToString();
                }
              
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (counter >= 1)
            {
                
                if (chart1.Series[2].Points.Count >= 1)
                {
                    chart1.Series[2].Points.RemoveAt(chart1.Series[2].Points.Count-1);
                }
                else
                {
                    chart1.Series[2].Points.AddXY(steps[counter-1].x, steps[counter-1].y);
                    label7.Text = "Шаг: " + +counter + "/" + steps.Count.ToString();
                    counter--;
                }
            }
        }
    }

    public class point
    {
        public double x, y;
        public point(double X, double Y)
        {
            this.x = X;
            this.y = Y;
        }
    }
}
