using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente
{
    public partial class InicioSesion : Form
    {
        private SalaEspera salaEspera;
        private Socket server; // Único socket para la comunicación con el servidor
        private List<SalaEspera> ListaVentanasDeEspera = new List<SalaEspera>();

        public InicioSesion()
        {
            InitializeComponent();
            ConectarServidor();
        }

        private void ConectarServidor()
        {
            // Creamos un IPEndPoint con la IP y el puerto del servidor
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 8080);

            // Creamos el socket una única vez
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ipep); // Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado al servidor.");

                // Inicia el hilo para recibir mensajes del servidor
                Task.Run(() => AtenderServidor());
            }
            catch (SocketException)
            {
                MessageBox.Show("No se pudo conectar al servidor.");
                this.BackColor = Color.Red;
            }
        }

        private void AtenderServidor()
        {
            while (true)
            {
                try
                {
                    // Recibimos mensaje del servidor
                    byte[] msg2 = new byte[500];
                    int bytesRecibidos = server.Receive(msg2);
                    if (bytesRecibidos == 0)
                    {
                        MessageBox.Show("Conexión cerrada por el servidor.");
                        break;
                    }

                    string MensajeLimpio = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                    string[] TrozosRespuesta = MensajeLimpio.Split('/');

                    // Verificar que el mensaje contiene al menos el código
                    if (TrozosRespuesta.Length < 2)
                    {
                        MessageBox.Show("Mensaje recibido sin formato válido: " + MensajeLimpio);
                        continue;
                    }

                    int codigo = 0;
                    if (!int.TryParse(TrozosRespuesta[0], out codigo))
                    {
                        MessageBox.Show("Código no válido en el mensaje: " + MensajeLimpio);
                        continue;
                    }

                    // Procesar los códigos según tu lógica
                    switch (codigo)
                    {
                        case 1: // Registro
                            if (TrozosRespuesta.Length > 2)
                            {
                                string RespuestaServidor = TrozosRespuesta[2];
                                
                                // Procesa la respuesta de registro aquí
                            }
                            break;

                        case 2: // Login
                            if (TrozosRespuesta.Length > 2)
                            {
                                string RespuestaServidor = TrozosRespuesta[2];
                                
                                // Procesa la respuesta de inicio de sesión aquí
                            }
                            break;

                        case 4: // Lista de jugadores conectados
                                // Validar que el mensaje tiene al menos un formato esperado
                            if (TrozosRespuesta.Length < 2)
                            {
                                MessageBox.Show("Respuesta del servidor inválida para el código 4.");
                                break;
                            }

                            // La lista de jugadores conectados viene en TrozosRespuesta[1]
                            string[] jugadoresConectados = TrozosRespuesta[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Actualizar el DataGridView de manera segura desde el hilo de la interfaz de usuario
                            this.Invoke(new Action(() =>
                            {
                                salaEspera.dataConectados.Rows.Clear(); // Limpiar la lista actual

                                foreach (string jugador in jugadoresConectados)
                                {
                                    salaEspera.dataConectados.Rows.Add(jugador); // Agregar cada jugador
                                }
                            }));

                            break;


                        case 7: // Respuesta de inicio de partida
                            if (TrozosRespuesta.Length > 2)
                            {
                                string RespuestaServidor = TrozosRespuesta[1];
                                string jugadorRival = TrozosRespuesta[2];
                                // Asumimos que estamos interactuando con la primera ventana de la sala de espera
                            }
                            break;

                        default:
                            MessageBox.Show("Código no reconocido: " + codigo);
                            break;
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Error de conexión: " + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error en AtenderServidor: " + ex.Message);
                }
            }
        }
    


        private async void buttonEnviar_Click(object sender, EventArgs e)
        {
            if (server == null || !server.Connected)
            {
                MessageBox.Show("No hay conexión con el servidor.");
                return;
            }

            if (Registrar.Checked) // Caso de registro
            {
                string mensaje = "1/" + Usuario.Text + "/" + Contraseña.Text; // Código 1 para registro
                await EnviarMensaje(mensaje);
                MessageBox.Show("Registro exitoso para " + Usuario.Text ); // Mostrar mensaje en el cliente
            }
            else if (IniciarSesion.Checked) // Caso de inicio de sesión
            {
                string mensaje = "2/" + Usuario.Text + "/" + Contraseña.Text; // Código 2 para login
                await EnviarMensaje(mensaje);
                salaEspera = new SalaEspera (server, Usuario.Text);
                salaEspera.Show();
                MessageBox.Show("Login exitoso para " + Usuario.Text); // Mostrar mensaje en el cliente

            }
            else if (CerrarSesion.Checked) // Caso de cierre de sesión
            {
                string mensaje = "0/"; // Código 0 para desconexión
                await EnviarMensaje(mensaje);
                Desconectar();
            }
        }

        private async Task EnviarMensaje(string mensaje)
        {
            try
            {
                if (server == null || !server.Connected)
                {
                    MessageBox.Show("La conexión con el servidor se ha perdido.");
                    return;
                }

                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                await Task.Run(() => server.Send(msg));
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error al enviar mensaje al servidor: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }


        private void Desconectar()
        {
            try
            {
                if (server != null && server.Connected)
                {
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                this.BackColor = Color.Gray;

                if (salaEspera != null)
                {
                    salaEspera.Close();
                    salaEspera = null;
                }

                MessageBox.Show("Desconectado del servidor.");
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error al desconectar del servidor: " + ex.Message);
            }
        }

        private void InicioSesion_FormClosing(object sender, FormClosingEventArgs e)
        {
            Desconectar();
        }
    }
}
