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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Cliente
{
    
    public partial class SalaEspera : Form
    {
        Socket server;
        public SalaEspera()
        {

            InitializeComponent();
           
        Conectados.Visible = false; // Oculta el botón Conectados

            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect("192.168.56.101", 9070); // Conecta al servidor al iniciar

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servidor: " + ex.Message);
            }
            // Configura el temporizador para actualizar la lista de conectados
            timerConectados = new Timer();
            timerConectados.Interval = 5000; // Intervalo en milisegundos (5000ms = 5s)
            timerConectados.Tick += timerConectados_Tick;
            timerConectados.Start(); // Inicia el temporizador
        }

        private async void buttonEnviar_Click(object sender, EventArgs e)
        {
            if (PorcentajeVictorias.Checked) // Caso de porcentaje de victorias
            {
                if (string.IsNullOrEmpty(Jugador.Text))
                {
                    MessageBox.Show("Por favor, introduce el nombre de un jugador.");
                    return; // Detener si el nombre no está presente
                }
                string mensaje = "5/" + Jugador.Text; // Código 5 para porcentaje de victorias
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                byte[] msg2 = new byte[512];

                // Recibir respuesta de forma asíncrona
                int bytesRecibidos = await Task.Run(() => server.Receive(msg2));
                string respuesta = Encoding.ASCII.GetString(msg2, 0, bytesRecibidos).Split('\0')[0];

                // Mostrar respuesta en un MessageBox
                MessageBox.Show(respuesta);
            }
            else if (RachaVictorias.Checked)
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

        private async void Conectados_Click(object sender, EventArgs e)
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

            // Asegurarse de que el DataGridView tiene una columna antes de agregar filas
            if (dataConectados.Columns.Count == 0)
            {
                dataConectados.Columns.Add("Jugador", "Jugador"); // Agregar una columna llamada "Jugador"
            }
            

            // Limpiar el DataGridView antes de agregar nuevos datos
            dataConectados.Rows.Clear();

            // Procesar la respuesta para llenar el DataGridView
            string[] jugadores = respuesta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < jugadores.Length; i++) // Empieza desde 1 para ignorar el código
            {
                dataConectados.Rows.Add(jugadores[i]); // Agregar cada jugador a una nueva fila
            }

        }
        private async void timerConectados_Tick(object sender, EventArgs e)
        {
            await ActualizarListaConectados(); // Llama al método para actualizar la lista de conectados periódicamente
        }

        private async Task ActualizarListaConectados()
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

            // Asegurarse de que el DataGridView tiene una columna antes de agregar filas
            if (dataConectados.Columns.Count == 0)
            {
                dataConectados.Columns.Add("Jugador", "Jugador"); // Agregar una columna llamada "Jugador"
            }

            // Limpiar el DataGridView antes de agregar nuevos datos
            dataConectados.Rows.Clear();

            // Procesar la respuesta para llenar el DataGridView
            string[] jugadores = respuesta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < jugadores.Length; i++) // Empieza desde 1 para ignorar el código
            {
                dataConectados.Rows.Add(jugadores[i]); // Agregar cada jugador a una nueva fila
            }
        }

    }
}



