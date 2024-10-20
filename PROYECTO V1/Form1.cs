using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;

namespace ClienteV1_2
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
            // Configuración del DataGridView
            dataGridJugadores.Columns.Add("Nombre", "Nombre");
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }



       
        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9070);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }


        private async void button4_Click(object sender, EventArgs e)
        {
            if (Register.Checked) // Caso de registro
            {
                string mensaje = "1/" + username.Text + "/" + password.Text; // Código 1 para registro
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);

                // Enviar mensaje de registro de forma asíncrona
                await Task.Run(() => server.Send(msg));

                byte[] msg2 = new byte[512];

                // Recibir respuesta de forma asíncrona
                int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
                string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];

                if (respuesta.Contains("1/Registro exitoso"))
                {
                    MessageBox.Show("Te has registrado correctamente.");
                }
                else
                {
                    MessageBox.Show("Error al registrarte: " + respuesta);
                }
            }
            else if (Login.Checked) // Caso de inicio de sesión
            {
                string mensaje = "2/" + username.Text + "/" + password.Text; // Código 2 para login
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);

                // Enviar mensaje de inicio de sesión de forma asíncrona
                await Task.Run(() => server.Send(msg));

                byte[] msg2 = new byte[512];

                // Recibir respuesta de forma asíncrona
                int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
                string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];



                // Evaluar la respuesta del servidor
                if (respuesta.Contains("2/Login exitoso"))
                {
                    MessageBox.Show("Inicio de sesión exitoso, bienvenido " + username.Text);
                }
                else
                {
                    MessageBox.Show("Error al iniciar sesión: ");
                }
            }

            else if (Queries.Checked) // Verificamos si está seleccionado
            {
                Form2 form2 = new Form2(); // Creamos una instancia del Forms2
                form2.Show(); // Mostramos el Forms2 como una nueva ventana
            }


        }
       





        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private async void button5_Click(object sender, EventArgs e)
        {
            // Enviar solicitud para obtener la lista de jugadores conectados
            string mensaje = "4/"; // Código 4 para solicitar la lista de conectados
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);

            // Enviar mensaje de solicitud de forma asíncrona
            await Task.Run(() => server.Send(msg));

            byte[] msg2 = new byte[512];

            // Recibir respuesta de forma asíncrona
            int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
            string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];

            // Limpiar el DataGridView antes de agregar nuevos datos
            dataGridJugadores.Rows.Clear();

            // Procesar la respuesta para llenar el DataGridView
            string[] jugadores = respuesta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < jugadores.Length; i++) // Empieza desde 1 para ignorar el código
            {
                dataGridJugadores.Rows.Add(jugadores[i]); // Agregar cada jugador a una nueva fila
            }
        }

        private void Queries_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

