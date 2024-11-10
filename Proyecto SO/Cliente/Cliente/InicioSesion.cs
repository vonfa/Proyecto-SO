using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace Cliente
{
    public partial class InicioSesion : Form
    {
        private SalaEspera salaEspera;
        Socket server;

        public InicioSesion()
        {
            InitializeComponent();
            

        }

        private async void buttonEnviar_Click(object sender, EventArgs e)
        {

            if (Registrar.Checked) // Caso de registro
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc, 9095);


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
                string mensaje = "1/" + Usuario.Text + "/" + Contraseña.Text; // Código 1 para registro
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
            else if (IniciarSesion.Checked) // Caso de inicio de sesión
            {
                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("10.4.119.5");
                IPEndPoint ipep = new IPEndPoint(direc, 9095);


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
                string mensaje = "2/" + Usuario.Text + "/" + Contraseña.Text; // Código 2 para login
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
                    MessageBox.Show("Inicio de sesión exitoso, bienvenido " + Usuario.Text);
                    // Abre la SalaEspera asociada con esta sesión
                    salaEspera = new SalaEspera();
                    salaEspera.Show();
                }
                else
                {
                    MessageBox.Show("Error al iniciar sesión: ");
                }
                
                InicioSesion form3 = new InicioSesion(); // Creamos una instancia del Forms3
                form3.Show(); // Mostramos el Forms3 como una nueva ventana


            }
            else if (CerrarSesion.Checked) // Caso de inicio de sesión
            {
                //Mensaje de desconexión
                string mensaje = "0/";

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                
                server.Close();
                // Cierra la instancia de SalaEspera asociada a esta sesión si está abierta
                if (salaEspera != null)
                {
                    salaEspera.Close();
                    salaEspera = null; // Libera la referencia
                }
            }




        }
    }
}
