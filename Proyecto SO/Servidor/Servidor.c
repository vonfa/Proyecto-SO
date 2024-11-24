#include <mysql.h>
#include <sys/types.h> 
#include <sys/socket.h> 
#include <netinet/in.h> 
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>

// Definiciones
#define MAX_JUGADORES 100
#define MAX_NOMBRE 20
#define TAM_BUFFER 512

// Estructuras
typedef struct {
    int sock;
    char nombre[MAX_NOMBRE];
} Jugador;

typedef struct {
    Jugador jugador[MAX_JUGADORES];
    int numero_jugadores;
} Lista_Jugadores;

Lista_Jugadores Jugadores;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
MYSQL *conn;

// Función para conectar a la base de datos
void conectarBD() {
    conn = mysql_init(NULL);
    if (conn == NULL) {
        printf("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
        exit(1);
    }
    
    conn = mysql_real_connect(conn, "localhost", "root", "mysql", "Championship", 0, NULL, 0);
    if (conn == NULL) {
        printf("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
        exit(1);
    }
}

// Función para ejecutar consultas SQL
void ejecutarConsultaSQL(char *sql) {
    if (mysql_query(conn, sql) != 0) {
        fprintf(stderr, "Error en la consulta: %s\n", mysql_error(conn));
        mysql_close(conn);
        exit(EXIT_FAILURE);
    }
}

// Verifica si un jugador ya está conectado
int jugador_ya_conectado(const char *nombre) {
    for (int i = 0; i < Jugadores.numero_jugadores; i++) {
        if (strcmp(Jugadores.jugador[i].nombre, nombre) == 0) {
            return 1; // Jugador ya está conectado
        }
    }
    return 0;
}

// Buscar el socket de un jugador en la lista de jugadores conectados
int BuscarSocketJugador(Lista_Jugadores *ListaJugadoresConectados, const char *NombreaBuscar) {
    for (int i = 0; i < ListaJugadoresConectados->numero_jugadores; i++) {
        if (strcmp(ListaJugadoresConectados->jugador[i].nombre, NombreaBuscar) == 0) {
            return ListaJugadoresConectados->jugador[i].sock;
        }
    }
    return -1; // No encontrado
}
// Buscar el nombre de un jugador por su socket
const char* BuscarNombrePorSocket(Lista_Jugadores *ListaJugadoresConectados, int sock) {
    for (int i = 0; i < ListaJugadoresConectados->numero_jugadores; i++) {
        if (ListaJugadoresConectados->jugador[i].sock == sock) {
            return ListaJugadoresConectados->jugador[i].nombre;
        }
    }
    return NULL; // No encontrado
}


// Manejo de la desconexión
void manejarDesconexion(int sock_conn) {
    pthread_mutex_lock(&mutex);
    for (int j = 0; j < Jugadores.numero_jugadores; j++) {
        if (Jugadores.jugador[j].sock == sock_conn) {
            for (int k = j; k < Jugadores.numero_jugadores - 1; k++) {
                Jugadores.jugador[k] = Jugadores.jugador[k + 1];
            }
            Jugadores.numero_jugadores--;
            break;
        }
    }
    pthread_mutex_unlock(&mutex);
}

// Registro de jugador
void manejarRegistro(int sock_conn, char *nombre, char *contrasenya) {
    char comando[256];
    char respuesta[256];
    sprintf(comando, "SELECT Name FROM Player WHERE Name='%s'", nombre);

    if (mysql_query(conn, comando) == 0) {
        MYSQL_RES *result = mysql_store_result(conn);
        if (mysql_num_rows(result) > 0) {
            sprintf(respuesta, "1/El jugador %s ya existe", nombre);
        } else {
            sprintf(comando, "INSERT INTO Player (Name, Password) VALUES ('%s', '%s')", nombre, contrasenya);
            ejecutarConsultaSQL(comando);
            sprintf(respuesta, "1/Registro exitoso para %s", nombre);
        }
        mysql_free_result(result);
    } else {
        sprintf(respuesta, "1/Error al realizar la consulta.");
    }
    write(sock_conn, respuesta, strlen(respuesta));
}

// Inicio de sesión
void manejarLogin(int sock_conn, char *nombre, char *contrasenya) {
    char comando[256];
    char respuesta[256];
    sprintf(comando, "SELECT Name FROM Player WHERE Name='%s' AND Password='%s'", nombre, contrasenya);

    if (mysql_query(conn, comando) == 0) {
        MYSQL_RES *result = mysql_store_result(conn);
        if (mysql_num_rows(result) > 0) {
            pthread_mutex_lock(&mutex);
            if (jugador_ya_conectado(nombre)) {
                sprintf(respuesta, "2/El jugador %s ya está conectado.", nombre);
            } else {
                sprintf(respuesta, "2/Login exitoso para %s", nombre);
                strcpy(Jugadores.jugador[Jugadores.numero_jugadores].nombre, nombre);
                Jugadores.jugador[Jugadores.numero_jugadores].sock = sock_conn;
                Jugadores.numero_jugadores++;
            }
            pthread_mutex_unlock(&mutex);
        } else {
            sprintf(respuesta, "2/Error: Usuario o contrase�a incorrectos");
        }
        mysql_free_result(result);
    } else {
        sprintf(respuesta, "2/Error al realizar la consulta");
    }

    write(sock_conn, respuesta, strlen(respuesta));
}

// Lista de jugadores conectados
void manejarListaConectados(int sock_conn) {
    char respuesta[512] = "4/Jugadores conectados:";
    pthread_mutex_lock(&mutex);
    for (int j = 0; j < Jugadores.numero_jugadores; j++) {
        strcat(respuesta, " ");
        strcat(respuesta, Jugadores.jugador[j].nombre);
    }
    pthread_mutex_unlock(&mutex);
    write(sock_conn, respuesta, strlen(respuesta));
}

// Función para atender jugadores
void *AtenderJugador(void *socket) {
    int sock_conn = *(int *)socket;
    char peticion[512];
    int ret, terminar = 0;

    while (!terminar) {
        ret = read(sock_conn, peticion, sizeof(peticion));
        peticion[ret] = '\0';
        printf("Peticion: %s\n", peticion);

        char *p = strtok(peticion, "/");
        int codigo = atoi(p);
        char nombre[20], contrasenya[20];

        switch (codigo) {
            case 0: // Desconexión
                terminar = 1;
                manejarDesconexion(sock_conn);
                break;
            case 1: // Registro
                p = strtok(NULL, "/");
                strcpy(nombre, p);
                p = strtok(NULL, "/");
                strcpy(contrasenya, p);
                manejarRegistro(sock_conn, nombre, contrasenya);
                break;
            case 2: // Iniciar sesión
                p = strtok(NULL, "/");
                strcpy(nombre, p);
                p = strtok(NULL, "/");
                strcpy(contrasenya, p);
                manejarLogin(sock_conn, nombre, contrasenya);
                break;
            case 4: // Lista de jugadores conectados
                manejarListaConectados(sock_conn);
                break;
            
			case 7: { // Petición para gestionar inicio de partida
				char Gestion[20];
				char UsuarioContrincante[MAX_NOMBRE];
				char Respuesta[TAM_BUFFER];

				p = strtok(NULL, "/"); // Gestión a realizar
				if (p == NULL) {
					sprintf(Respuesta, "7/ERROR/Faltan parámetros");
					write(sock_conn, Respuesta, strlen(Respuesta));
					break;
				}
				strncpy(Gestion, p, sizeof(Gestion) - 1);
				Gestion[sizeof(Gestion) - 1] = '\0';

				p = strtok(NULL, "/"); // Nombre del contrincante
				if (p == NULL) {
					sprintf(Respuesta, "7/ERROR/Faltan parámetros");
					write(sock_conn, Respuesta, strlen(Respuesta));
					break;
				}
				strncpy(UsuarioContrincante, p, sizeof(UsuarioContrincante) - 1);
				UsuarioContrincante[sizeof(UsuarioContrincante) - 1] = '\0';

				// Buscar el socket del contrincante
				int SocketContrincante = BuscarSocketJugador(&Jugadores, UsuarioContrincante);
				if (SocketContrincante == -1) {
					sprintf(Respuesta, "7/ERROR/Usuario no conectado");
					write(sock_conn, Respuesta, strlen(Respuesta));
					break;
				}

				const char* NombreSolicitante = BuscarNombrePorSocket(&Jugadores, sock_conn);
				if (NombreSolicitante == NULL) {
					sprintf(Respuesta, "7/ERROR/No se pudo identificar al jugador solicitante");
					write(sock_conn, Respuesta, strlen(Respuesta));
					break;
				}

				if (strcmp(Gestion, "ENVIAR") == 0) { // Solicitar un duelo
					pthread_mutex_lock(&mutex);
					sprintf(Respuesta, "7/RECIBIR/%s", NombreSolicitante);
					pthread_mutex_unlock(&mutex);

					write(SocketContrincante, Respuesta, strlen(Respuesta));
					printf("Enviado a %d: %s\n", SocketContrincante, Respuesta);
				}
				break;
}


        }
    }

    close(sock_conn);
    return NULL;
}

// Función principal
int main(int argc, char *argv[]) {
    int sockfd, newsockfd;
    struct sockaddr_in server_addr, client_addr;
    socklen_t clilen;
    pthread_t thread_id;

    // Conectar a la base de datos
    conectarBD();

    // Crear socket
    sockfd = socket(AF_INET, SOCK_STREAM, 0);
    if (sockfd < 0) {
        perror("Error al abrir el socket");
        exit(1);
    }

    // Configurar dirección del servidor
    memset((char *)&server_addr, 0, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = INADDR_ANY;
    server_addr.sin_port = htons(8080);

    // Enlazar el socket a una dirección
    if (bind(sockfd, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0) {
        perror("Error en el bind");
        close(sockfd);
        exit(1);
    }

    // Poner el servidor en modo escucha (hasta 5 conexiones en espera)
    if (listen(sockfd, 5) < 0) {
        perror("Error al poner el servidor a escuchar");
        close(sockfd);
        exit(1);
    }

    printf("Escuchando...\n");

    // Aceptar conexiones entrantes
    clilen = sizeof(client_addr);
    while (1) {
        // Aceptar una nueva conexión
        newsockfd = accept(sockfd, (struct sockaddr *)&client_addr, &clilen);
        if (newsockfd < 0) {
            perror("Error en la aceptacion de la conexion");
            continue; // Si hay un error en la aceptación, continuar escuchando
        }

        // Crear un hilo para manejar la conexión del cliente
        if (pthread_create(&thread_id, NULL, AtenderJugador, (void *)&newsockfd) != 0) {
            perror("Error al crear hilo para el cliente");
            continue; // Si hay un error al crear el hilo, continuar aceptando conexiones
        }

        // Detener el hilo para que no se bloquee
        pthread_detach(thread_id);  // Esto permite que el hilo se limpie automáticamente al finalizar
    }

    // Cerrar el socket del servidor (esto no se alcanza nunca en este flujo de trabajo)
    close(sockfd);
    mysql_close(conn);  // Cerrar la conexión a la base de datos cuando se termine
    return 0;
}
