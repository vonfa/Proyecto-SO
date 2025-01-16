using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClienteV1_2
{
    public partial class Form2 : Form
    {
        Socket server;

        public Form2()
        {
            InitializeComponent();
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect("192.168.56.101", 9070); // Conecta al servidor al iniciar
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servidor: " + ex.Message);
            }
        }



        private async void button4_Click(object sender, EventArgs e)
        {


            if (Porcentaje.Checked) // Caso de porcentaje de victorias
            {
                if (string.IsNullOrEmpty(username.Text))
                {
                    MessageBox.Show("Por favor, introduce el nombre de un jugador.");
                    return; // Detener si el nombre no está presente
                }
                string mensaje = "5/" + username.Text; // Código 5 para porcentaje de victorias
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[512];

                // Recibir respuesta de forma asíncrona
                int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
                string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];

                // Mostrar respuesta en un MessageBox
                MessageBox.Show(respuesta);
            }
            else if (Racha.Checked)
            {
                string mensaje = "6/"; // Código 6 para la mayor racha de victorias
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[512];

                // Recibir respuesta de forma asíncrona
                int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
                string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];

                // Mostrar respuesta en un MessageBox
                MessageBox.Show(respuesta);
            }
        }

        private void Porcentaje_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
