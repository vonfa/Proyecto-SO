namespace Cliente
{
    partial class SalaEspera
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
            this.dataConectados = new System.Windows.Forms.DataGridView();
            this.Jugador = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonEnviar = new System.Windows.Forms.Button();
            this.PorcentajeVictorias = new System.Windows.Forms.RadioButton();
            this.RachaVictorias = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.EmpezarPartida = new System.Windows.Forms.Button();
            this.Conectados = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataConectados)).BeginInit();
            this.SuspendLayout();
            // 
            // dataConectados
            // 
            this.dataConectados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataConectados.Location = new System.Drawing.Point(409, 90);
            this.dataConectados.Name = "dataConectados";
            this.dataConectados.Size = new System.Drawing.Size(265, 266);
            this.dataConectados.TabIndex = 0;
            // 
            // Jugador
            // 
            this.Jugador.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Jugador.Location = new System.Drawing.Point(184, 51);
            this.Jugador.Name = "Jugador";
            this.Jugador.Size = new System.Drawing.Size(172, 26);
            this.Jugador.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(57, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Jugador";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(57, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "Consultas";
            // 
            // buttonEnviar
            // 
            this.buttonEnviar.BackColor = System.Drawing.SystemColors.Window;
            this.buttonEnviar.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnviar.Location = new System.Drawing.Point(218, 259);
            this.buttonEnviar.Name = "buttonEnviar";
            this.buttonEnviar.Size = new System.Drawing.Size(84, 37);
            this.buttonEnviar.TabIndex = 12;
            this.buttonEnviar.Text = "Enviar";
            this.buttonEnviar.UseVisualStyleBackColor = false;
            this.buttonEnviar.Click += new System.EventHandler(this.buttonEnviar_Click);
            // 
            // PorcentajeVictorias
            // 
            this.PorcentajeVictorias.AutoSize = true;
            this.PorcentajeVictorias.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PorcentajeVictorias.Location = new System.Drawing.Point(184, 144);
            this.PorcentajeVictorias.Name = "PorcentajeVictorias";
            this.PorcentajeVictorias.Size = new System.Drawing.Size(202, 27);
            this.PorcentajeVictorias.TabIndex = 13;
            this.PorcentajeVictorias.TabStop = true;
            this.PorcentajeVictorias.Text = "Porcentaje de Victorias";
            this.PorcentajeVictorias.UseVisualStyleBackColor = true;
            // 
            // RachaVictorias
            // 
            this.RachaVictorias.AutoSize = true;
            this.RachaVictorias.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RachaVictorias.Location = new System.Drawing.Point(184, 190);
            this.RachaVictorias.Name = "RachaVictorias";
            this.RachaVictorias.Size = new System.Drawing.Size(215, 27);
            this.RachaVictorias.TabIndex = 14;
            this.RachaVictorias.TabStop = true;
            this.RachaVictorias.Text = "Mejor Racha de Victorias";
            this.RachaVictorias.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(57, 333);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 23);
            this.label3.TabIndex = 15;
            // 
            // EmpezarPartida
            // 
            this.EmpezarPartida.BackColor = System.Drawing.SystemColors.Window;
            this.EmpezarPartida.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmpezarPartida.Location = new System.Drawing.Point(462, 374);
            this.EmpezarPartida.Name = "EmpezarPartida";
            this.EmpezarPartida.Size = new System.Drawing.Size(160, 37);
            this.EmpezarPartida.TabIndex = 16;
            this.EmpezarPartida.Text = "Empezar Partida";
            this.EmpezarPartida.UseVisualStyleBackColor = false;
            // 
            // Conectados
            // 
            this.Conectados.BackColor = System.Drawing.SystemColors.Window;
            this.Conectados.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Conectados.Location = new System.Drawing.Point(487, 44);
            this.Conectados.Name = "Conectados";
            this.Conectados.Size = new System.Drawing.Size(112, 37);
            this.Conectados.TabIndex = 18;
            this.Conectados.Text = "Conectados";
            this.Conectados.UseVisualStyleBackColor = false;
            this.Conectados.Click += new System.EventHandler(this.Conectados_Click);
            // 
            // SalaEspera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(686, 423);
            this.Controls.Add(this.Conectados);
            this.Controls.Add(this.EmpezarPartida);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.RachaVictorias);
            this.Controls.Add(this.PorcentajeVictorias);
            this.Controls.Add(this.buttonEnviar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Jugador);
            this.Controls.Add(this.dataConectados);
            this.Name = "SalaEspera";
            this.Text = "Sala de Espera";
            ((System.ComponentModel.ISupportInitialize)(this.dataConectados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataConectados;
        private System.Windows.Forms.TextBox Jugador;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonEnviar;
        private System.Windows.Forms.RadioButton PorcentajeVictorias;
        private System.Windows.Forms.RadioButton RachaVictorias;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button EmpezarPartida;
        private System.Windows.Forms.Button Conectados;
    }
}