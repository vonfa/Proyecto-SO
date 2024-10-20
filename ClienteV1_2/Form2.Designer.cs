namespace ClienteV1_2
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.username = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Racha = new System.Windows.Forms.RadioButton();
            this.Porcentaje = new System.Windows.Forms.RadioButton();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.groupBox3.Controls.Add(this.username);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.Racha);
            this.groupBox3.Controls.Add(this.Porcentaje);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Location = new System.Drawing.Point(91, 71);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(363, 202);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "CONSULTAS";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(123, 46);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(86, 20);
            this.username.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Username";
            // 
            // Racha
            // 
            this.Racha.AutoSize = true;
            this.Racha.Location = new System.Drawing.Point(81, 118);
            this.Racha.Name = "Racha";
            this.Racha.Size = new System.Drawing.Size(180, 17);
            this.Racha.TabIndex = 7;
            this.Racha.TabStop = true;
            this.Racha.Text = "MEJOR RACHA DE VICTORIAS";
            this.Racha.UseVisualStyleBackColor = true;
            // 
            // Porcentaje
            // 
            this.Porcentaje.AutoSize = true;
            this.Porcentaje.Location = new System.Drawing.Point(81, 80);
            this.Porcentaje.Name = "Porcentaje";
            this.Porcentaje.Size = new System.Drawing.Size(174, 17);
            this.Porcentaje.TabIndex = 8;
            this.Porcentaje.TabStop = true;
            this.Porcentaje.Text = "PORCENTAJE DE VICTORIAS";
            this.Porcentaje.UseVisualStyleBackColor = true;
            this.Porcentaje.CheckedChanged += new System.EventHandler(this.Porcentaje_CheckedChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(133, 173);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Enviar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.groupBox3);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form2";
            this.Text = "Form2";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton Racha;
        private System.Windows.Forms.RadioButton Porcentaje;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox username;
    }
}