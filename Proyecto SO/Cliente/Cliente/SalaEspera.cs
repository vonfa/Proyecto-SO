using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente
{
    public partial class SalaEspera : Form
    {
        private Socket server;
        private string usuario;
        private string jugadorRival;

        public SalaEspera(Socket server, string usuario)
        {
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;

            if (server != null && server.Connected)
            {
                MessageBox.Show("Conexión con el servidor establecida correctamente.");
                EscucharMensajes();
            }
            else
            {
                MessageBox.Show("No hay conexión con el servidor.");
            }

            InicializarDataGridView();

        }

        private void InicializarDataGridView()
        {
            dataConectados.Columns.Clear();
            dataConectados.Columns.Add("Jugador", "Jugador");
            dataConectados.Rows.Clear();
        }

        private async void buttonEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                string mensaje;
                if (PorcentajeVictorias.Checked)
                {
                    if (string.IsNullOrEmpty(Jugador.Text))
                    {
                        MessageBox.Show("Por favor, introduce el nombre de un jugador.");
                        return;
                    }

                    mensaje = $"5/{Jugador.Text}"; // Código para porcentaje de victorias
                }
                else if (RachaVictorias.Checked)
                {
                    mensaje = "6/"; // Código para racha de victorias
                }
                else
                {
                    MessageBox.Show("Selecciona una opción antes de enviar.");
                    return;
                }

                await EnviarMensajeYMostrarRespuesta(mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar datos al servidor: " + ex.Message);
            }
        }

        private async void Conectados_Click(object sender, EventArgs e)
        {
            try
            {
                await EnviarMensaje("4/"); // Solicitar lista de conectados
                byte[] respuestaBytes = await RecibirRespuesta();
                string respuesta = Encoding.ASCII.GetString(respuestaBytes).Trim();

                // Limpiar y actualizar el DataGridView con los jugadores conectados
                InicializarDataGridView();
                string[] jugadores = respuesta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string jugador in jugadores.Skip(1)) // Ignorar el código en el índice 0
                {
                    dataConectados.Rows.Add(jugador);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recibir la lista de conectados: " + ex.Message);
            }
        }

        private void Invitacion_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataConectados.CurrentRow == null || dataConectados.CurrentRow.Cells[0].Value == null)
                {
                    MessageBox.Show("Por favor, selecciona un jugador en la lista.");
                    return;
                }

                string jugadorSeleccionado = dataConectados.CurrentRow.Cells[0].Value.ToString();

                if (string.IsNullOrEmpty(jugadorSeleccionado) || jugadorSeleccionado == usuario)
                {
                    MessageBox.Show("Selecciona un jugador válido diferente a tu usuario.");
                    return;
                }

                string mensaje = $"7/ENVIAR/{jugadorSeleccionado}";
                MessageBox.Show($"Mensaje enviado: {mensaje}"); // Para depuración
                EnviarMensaje(mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar la invitación: " + ex.Message);
            }
        }

        public void ProcesarInvitacion(string gestion, string jugadorRival)
        {
            this.jugadorRival = jugadorRival;

            if (gestion == "RECIBIR")
            {
                if (InvokeRequired)
                {
                    // Usamos Invoke para asegurar que la llamada se realiza en el hilo de la interfaz gráfica
                    Invoke(new Action(() => MostrarMessageBoxInvitacion(jugadorRival)));
                }
                else
                {
                    MostrarMessageBoxInvitacion(jugadorRival);
                }
            }
        }

        private void MostrarMessageBoxInvitacion(string jugadorRival)
        {
            // Mostrar el MessageBox con la invitación y las opciones
            DialogResult resultado = MessageBox.Show(
                $"Has recibido una solicitud de partida por parte de {jugadorRival}. ¿Deseas aceptar su petición?",
                "Invitación recibida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            string mensaje;

            if (resultado == DialogResult.Yes)
            {
                // Enviar mensaje de aceptación al servidor
                mensaje = $"7/ACEPTAR/{jugadorRival}";
                MessageBox.Show("Has aceptado la invitación.", "Invitación aceptada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Enviar mensaje de rechazo al servidor
                mensaje = $"7/RECHAZAR/{jugadorRival}";
                MessageBox.Show("Has rechazado la invitación.", "Invitación rechazada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Enviar el mensaje correspondiente al servidor
            EnviarMensajeAlServidor(mensaje);
        }

        private void EnviarMensajeAlServidor(string mensaje)
        {
            try
            {
                // Convertir el mensaje a bytes y enviarlo al servidor
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar mensaje al servidor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void ProcesarMensaje(string mensaje)
        {
            string[] partes = mensaje.Split('/');
            if (partes.Length < 3)
            {
                MessageBox.Show("Mensaje inválido recibido del servidor.");
                return;
            }

            int codigo = int.TryParse(partes[0], out var cod) ? cod : -1;
            if (codigo == 7 && partes[1] == "RECIBIR")
            {
                ProcesarInvitacion("RECIBIR", partes[2]);
            }
        }

        private async Task EnviarMensaje(string mensaje)
        {
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            await Task.Run(() => server.Send(msg));
        }

        private async Task<byte[]> RecibirRespuesta()
        {
            byte[] buffer = new byte[512];
            int bytesRecibidos = await Task.Run(() => server.Receive(buffer));
            byte[] mensajeCompleto = buffer.Take(bytesRecibidos).ToArray();

            string mensaje = Encoding.ASCII.GetString(mensajeCompleto).Trim();
            Console.WriteLine($"Mensaje recibido del servidor: {mensaje}"); // Log para debug

            // Procesar mensaje recibido
            ProcesarMensaje(mensaje);

            return mensajeCompleto;
        }


        private async Task EnviarMensajeYMostrarRespuesta(string mensaje)
        {
            await EnviarMensaje(mensaje);

            byte[] respuestaBytes = await RecibirRespuesta();
            string respuesta = Encoding.ASCII.GetString(respuestaBytes).Trim();
            MessageBox.Show(respuesta);
        }

        private void dataConectados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Método para eventos de clic en celdas del DataGridView (si es necesario)
        }
        private async void EscucharMensajes()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[512];
                    int bytesRecibidos = await Task.Run(() => server.Receive(buffer));
                    if (bytesRecibidos > 0)
                    {
                        string mensaje = Encoding.ASCII.GetString(buffer, 0, bytesRecibidos).Trim();
                        Console.WriteLine($"Mensaje recibido: {mensaje}"); 
                        ProcesarMensaje(mensaje);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al recibir mensajes: " + ex.Message);
                    break;
                }
            }
        }

    }
}
