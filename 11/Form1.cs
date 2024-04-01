namespace _11
{
    public partial class ����� : Form
    {
        public �����()
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
                // �������� �������� A, B � C �� ��������� �����
                double A = double.Parse(a.Text);
                double B = double.Parse(b.Text);
                double C = double.Parse(c.Text);

                //  �������� ��������� �������� � ���������
                Properties.Settings.Default.A = A;
                Properties.Settings.Default.B = B;
                Properties.Settings.Default.C = C;
                Properties.Settings.Default.Save(); 

                // ��������� ����������
                int CheckB = Logic.CountingB(A, B);
                int CheckC = Logic.CountingC(A, C);

                // ��������� ������ ��� ������
                string resultMessage = string.Format("�������� ������������ ���������� ������ \n�������� {0} ������ ����� {1} �������\n", B, CheckB);
                resultMessage += string.Format("\n������ ������ �������� {0} ������ ����� \n{1} �������", C, CheckC);

                // ������� ���������� � ����� MessageBox
                MessageBox.Show(resultMessage);
            }
            catch (FormatException)
            {
                MessageBox.Show("������");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("��������� 1 ����� ������ ���� � �����, ������ A ���.����� ������ ����� ������ ������ ������������� �� 2 % �� ��������� �����. ����������: �) �� ����� ����� �������� ������������ ���������� ������ �������� B ���.; �) ����� ������� ������� ������ ������ �������� C ���");
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
        /brief ������� ��� ���������� ������, ����� ����������� ���������� ������ ��������� �������� �����.
        /param A - �������������� �����, B - �����, ������� ���������� ���������.
        /return monthsB - ���������� �������.
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
        /brief ������� ��� ���������� ������, ����� ����� ��������� �������� �����.
        /param A - �������������� �����, � - �����, ������� ���������� ���������.
        /return months� - ���������� �������.
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
