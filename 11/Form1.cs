namespace _11
{
    public partial class Вклад : Form
    {
        public Вклад()
        {
            InitializeComponent();
            a.Text = Properties.Settings.Default.A.ToString();
            b.Text = Properties.Settings.Default.B.ToString();
            c.Text = Properties.Settings.Default.C.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем значения A, B и C из текстовых полей
                double A = double.Parse(a.Text);
                double B = double.Parse(b.Text);
                double C = double.Parse(c.Text);

                //  передаем введенные значения в параметры
                Properties.Settings.Default.A = A;
                Properties.Settings.Default.B = B;
                Properties.Settings.Default.C = C;
                Properties.Settings.Default.Save(); 

                // Вычисляем результаты
                int CheckB = Logic.CountingB(A, B);
                int CheckC = Logic.CountingC(A, C);

                // Формируем строку для вывода
                string resultMessage = string.Format("Величина ежемесячного увеличения вклада \nпревысит {0} рублей через {1} месяцев\n", B, CheckB);
                resultMessage += string.Format("\nРазмер вклада превысит {0} рублей через \n{1} месяцев", C, CheckC);

                // Выводим результаты в одном MessageBox
                MessageBox.Show(resultMessage);
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Гражданин 1 марта открыл счет в банке, вложив A руб.Через каждый месяц размер вклада увеличивается на 2 % от имеющейся суммы. Определить: а) за какой месяц величина ежемесячного увеличения вклада превысит B руб.; б) через сколько месяцев размер вклада превысит C руб");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            a.Text = "";
            b.Text = "";
            c.Text = "";
        }

        private void a_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                b.Focus();
            }
        }

        private void b_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                c.Focus();
            }
        }

        private void button1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void c_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                button1.Focus();
            }
        }
    }
    public class Logic
    {   
        
        /*
        /brief Функция для нахождения месяца, когда ежемесечное увелечение вклада превышает заданное число.
        /param A - Первоначальный вклад, B - число, которое необходимо превысить.
        /return monthsB - Количество месяцев.
        */
        public static int CountingB(double A, double B)
        {
            double Percent = A * 0.02;
            double Balance = A;
            int monthsB = 0;

            for (int i = 0; ; i++)
            {
                if (Percent > B)
                {
                    monthsB = i;
                    break;
                }

                Balance += Percent;
                Percent = Balance * 0.02;
            }
            return monthsB;
        }
        /*
        /brief Функция для нахождения месяца, когда вклад превышает заданное число.
        /param A - Первоначальный вклад, С - число, которое необходимо превысить.
        /return monthsС - Количество месяцев.
        */
        public static int CountingC(double A, double C)
        {
            double Percent = A * 0.02;
            double Balance = A;
            int monthsC = 0;

            for (int i = 0; ; i++)
            {
                if (Balance > C)
                {
                    monthsC = i;
                    break;
                }
                Balance += Percent;
                Percent = Balance * 0.02;
            }
            return monthsC;
        }
    }
}
