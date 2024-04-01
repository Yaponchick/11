namespace _11
{
    partial class Вклад
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            b = new TextBox();
            c = new TextBox();
            button1 = new Button();
            a = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // b
            // 
            b.Location = new Point(1, 137);
            b.Name = "b";
            b.Size = new Size(125, 27);
            b.TabIndex = 2;
            b.KeyPress += b_KeyPress;
            // 
            // c
            // 
            c.Location = new Point(1, 210);
            c.Name = "c";
            c.Size = new Size(125, 27);
            c.TabIndex = 3;
            c.KeyPress += c_KeyPress;
            // 
            // button1
            // 
            button1.Location = new Point(1, 243);
            button1.Name = "button1";
            button1.Size = new Size(125, 29);
            button1.TabIndex = 4;
            button1.Text = "Вычислить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            button1.KeyPress += button1_KeyPress;
            // 
            // a
            // 
            a.Location = new Point(1, 64);
            a.Name = "a";
            a.Size = new Size(125, 27);
            a.TabIndex = 1;
            a.KeyPress += a_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1, 41);
            label1.Name = "label1";
            label1.Size = new Size(254, 20);
            label1.TabIndex = 5;
            label1.Text = "Сколько гражданин вложил денег?";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1, 94);
            label2.Name = "label2";
            label2.Size = new Size(261, 40);
            label2.TabIndex = 6;
            label2.Text = "Какое значение должно превысить \r\nежемесячное увелечение вклада?";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1, 167);
            label3.Name = "label3";
            label3.Size = new Size(269, 40);
            label3.TabIndex = 7;
            label3.Text = "Какое значение должнен превысить \r\nразмер вклада?";
            // 
            // button2
            // 
            button2.Location = new Point(1, 9);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 5;
            button2.Text = "Задание";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(147, 210);
            button3.Name = "button3";
            button3.Size = new Size(94, 61);
            button3.TabIndex = 6;
            button3.Text = "Очистить";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Вклад
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.InactiveCaption;
            ClientSize = new Size(262, 283);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(a);
            Controls.Add(button1);
            Controls.Add(c);
            Controls.Add(b);
            Name = "Вклад";
            Text = "Вклад";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox b;
        private TextBox c;
        private Button button1;
        private TextBox a;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button button2;
        private Button button3;
    }
}
