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
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;

namespace Laba1
{
    public partial class Form1 : Form
    {
        public int counter;//счетчик для шагов

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string clientsecret = @"C:\Users\Sezion\source\repos\Laba1\client_secret.json";
        static readonly string AppName = "laba3";
        static readonly string SpreadsheetId = "1Kcvpqi-I6wY0HSFGehgdVp_tS70Fk2KQroZT39Z8S5Q";
        const string Range = "'Лист1'!A1:B10";


        public Form1()
        {
            InitializeComponent();
        }

        private UserCredential GetSheetsCredential()
        {
            using (var stream =
               new FileStream(clientsecret, FileMode.Open, FileAccess.Read))
            {
                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetsCreds.json");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var credential = GetSheetsCredential();

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });

            SpreadsheetsResource.ValuesResource.GetRequest request =
                  service.Spreadsheets.Values.Get(SpreadsheetId, Range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();
        }

        private double f(double x)//вынес подставления значения в функцию в отдельный метод
        {
                double result = 0;
                Function f = new Function("f(x) = " + textBox1.Text);
                string sklt = "f()";
                string fx = sklt.Insert(2, x.ToString());
                fx = fx.Replace(",", ".");
                Expression fxx = new Expression(fx, f);
                result = fxx.calculate();   
                return result;
        }
        
        async Task<Double> method(double a, double b, double e)//асинхроним расчеты метода
        {
            var result = await Task.Run(() => mingold(a, b, e));
            return result;
        }

        public static List<point> steps = new List<point>();//список точек-шагов

        private double mingold(double a, double b, double e)//метод золотого сечения
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

        public async Task drawgraph(double min, double max, double step)//асинхронная отрисовка графа, так как именно его отрисовка руинит программу
        {
            await Task.Run(() => graph(min, max, step));
        }

        public void addpoint(double[] x, double[] y1)//метод добавления сплайна на график
        {
            chart1.Series[0].Points.DataBindXY(x, y1);
        }

        public void graph(double min, double max, double step)//сама отрисовка графика
        {
            try
            {
                
                int count = (int)Math.Ceiling((max - min) / step) + 1;

                double[] x = new double[count];
                double[] y1 = new double[count];

                for (int i = 0; i < count; i++)
                {
                    x[i] = min + step * i;
                    y1[i] = f(x[i]);
                }
                Action action = () => addpoint(x, y1);//так как этот метод находится в другом потоке то вызываем через инвоук
                Invoke(action);
                
            }
            catch(System.OverflowException)
            {
                DialogResult err = MessageBox.Show("Границы введены неверно!!!\nУбедитесь в правильности введеных данных и повторите поптыку!", "Ошибка!");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[1].Points.Clear();//очищаем все
            chart1.Series[2].Points.Clear();
            chart1.Update();
            steps.Clear();

            if (Double.IsNaN(f(1)) == true)//проверка на нечисло
            {
                DialogResult err = MessageBox.Show("Функция введена неверно!!!\nНажмите ОЧИСТИТЬ и повторите поптыку!", "Ошибка!");
            }

            if (textBox2.Text.Length >= 5 || textBox3.Text.Length >= 5)
            {
                DialogResult err = MessageBox.Show("Очень большие границы, возможна некорректная работа приложения!", "Внимание!");
            }


            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox4.Text.Length > 9)//проверка на заполненность данных
            {
                DialogResult err = MessageBox.Show("Введите все данные или уменьшите E!!!", "Ошибка!");
            }
            else
            {
                try
                {
                    double Xmin = double.Parse(textBox2.Text);
                    double Xmax = double.Parse(textBox3.Text);
                    double eps = double.Parse(textBox4.Text);

                    chart1.ChartAreas[0].AxisX.Minimum = Xmin;
                    chart1.ChartAreas[0].AxisX.Maximum = Xmax;
                    chart1.ChartAreas[0].AxisX.MajorGrid.Interval = 1;


                    double result = await method(Xmin, Xmax, eps);
                    
                    
                    chart1.Series[1].Points.AddXY(result, f(result));

                    point end = new point(result, f(result));//добавляем конечную точку
                    steps.Add(end);

                    label2.Text = Math.Round(f(result), 5).ToString();

                    await drawgraph(Xmin, Xmax, 1);
                    
                    counter = 0;

                }
                catch (System.FormatException)
                {
                    DialogResult err = MessageBox.Show("Значения введены неверно!\nПроверьте корректность данных", "Ошибка!");
                }
            }
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

        private void button3_Click(object sender, EventArgs e)//шаг вперед
        {
            if (counter < steps.Count)
            {
                chart1.Series[2].Points.Clear();
                chart1.Series[2].Points.AddXY(steps[counter].x, steps[counter].y);
                counter++;
                label7.Text = "Шаг: " + +counter + "/" + steps.Count.ToString();
                
            }
        }

        private void button2_Click(object sender, EventArgs e)//шаг назад
        {
            if (counter >= 1)
            {
                chart1.Series[2].Points.Clear();
                chart1.Series[2].Points.AddXY(steps[counter-1].x, steps[counter-1].y);
                label7.Text = "Шаг: " + +counter + "/" + steps.Count.ToString();
                counter--;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            point xy = new point(Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox6.Text));
            steps.Add(xy);
            dataGridView1.Rows.Add(xy.x, xy.y);
            textBox5.Text = "";
            textBox6.Text = "";
            chart1.Series[1].Points.AddXY(xy.x, xy.y);
        }
    }

    public class point//класс для сохранения точек
    {
        public double x, y;
        public point(double X, double Y)
        {
            this.x = X;
            this.y = Y;
        }
    }
}
//log(5,4*x-3)-ln(2*x)+log10(3.14)
