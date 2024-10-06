#include <mysql.h>
#include <sys/types.h> 
#include <sys/socket.h> 
#include <netinet/in.h> 
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>

// Estructuras
typedef struct {
    int *sock;
    char Nombre[20];
} Cliente;

typedef struct {
    Cliente cliente[100];
    int numero_clientes;
} Cliente_Lista;

Cliente_Lista Clis;
int contador = 0;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
MYSQL *conn;
int i = 0;
char peticion[512];
char respuesta[512];

// Función para conectar a la base de datos
void conectarBD() {
    conn = mysql_init(NULL);
    if (conn == NULL) {
        printf("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
        exit(1);
    }
    
    conn = mysql_real_connect(conn, "localhost", "root", "mysql", NULL, 0, Championship, 0);
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

// Función para atender clientes
void *AtenderCliente(void *socket) {
    int sock_conn;
    int *s;
    s = (int *) socket;
    sock_conn = *s;    
    int ret;

    int terminar = 0;
    while (terminar == 0) {
        ret = read(sock_conn, peticion, sizeof(peticion));
        printf("Recibido\n");
        peticion[ret] = '\0';
        printf("Peticion: %s\n", peticion);
        
        char *p = strtok(peticion, "/");
        int codigo = atoi(p);

        char Nombre[20];
        char Contrasenya[20];

        if (codigo == 0) { // Desconexión
            p = strtok(NULL, "/");
            strcpy(Nombre, p);

            int k;
            for (k = 0; k < Clis.numero_clientes; k++) {
                if (strcmp(Clis.cliente[k].Nombre, Nombre) == 0) {
                    int x;
                    for (x = k; x < Clis.numero_clientes - 1; x++) {
                        strcpy(Clis.cliente[x].Nombre, Clis.cliente[x + 1].Nombre);
                        Clis.cliente[x].sock = Clis.cliente[x + 1].sock;
                    }
                    Clis.numero_clientes--;
                    break;
                }
            }

            contador--;
            sprintf(respuesta, "0/Desconexion correcta de %s", Nombre);
            write(sock_conn, respuesta, strlen(respuesta));
            terminar = 1;

        } else if (codigo == 1) { // Registro
            p = strtok(NULL, "/");
            strcpy(Nombre, p);
            p = strtok(NULL, "/");
            strcpy(Contrasenya, p);

            char comando[256];
            sprintf(comando, "SELECT Name FROM Player WHERE Name='%s'", Nombre);

            if (mysql_query(conn, comando) == 0) {
                MYSQL_RES *result = mysql_store_result(conn);
                if (mysql_num_rows(result) > 0) {
                    sprintf(respuesta, "1/El usuario %s ya existe", Nombre);
                } else {
                    sprintf(comando, "INSERT INTO Player (Name, Password) VALUES ('%s', '%s')", Nombre, Contrasenya);
                    ejecutarConsultaSQL(comando);
                    sprintf(respuesta, "1/Registro exitoso para %s", Nombre);
                }
                mysql_free_result(result);
            }
            write(sock_conn, respuesta, strlen(respuesta));

        } else if (codigo == 2) { // Iniciar sesión
            p = strtok(NULL, "/");
            strcpy(Nombre, p);
            p = strtok(NULL, "/");
            strcpy(Contrasenya, p);

            char comando[256];
            sprintf(comando, "SELECT Name FROM Player WHERE Name='%s' AND Password='%s'", Nombre, Contrasenya);

            if (mysql_query(conn, comando) == 0) {
                MYSQL_RES *result = mysql_store_result(conn);
                if (mysql_num_rows(result) > 0) {
                    sprintf(respuesta, "2/Inicio de sesión exitoso para %s", Nombre);
                } else {
                    sprintf(respuesta, "2/Error: Usuario o contraseña incorrectos");
                }
                mysql_free_result(result);
            } else {
                sprintf(respuesta, "2/Error al realizar la consultami");
            }

            write(sock_conn, respuesta, strlen(respuesta));
        }
    }
    close(sock_conn);
    return NULL;
}

int main(int argc, char *argv[]) {
    conectarBD();

    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;

    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        printf("Error creando socket");
        exit(1);
    }

    memset(&serv_adr, 0, sizeof(serv_adr));
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(9050);

    if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0) {
        printf("Error en el bind");
        exit(1);
    }

    if (listen(sock_listen, 3) < 0) {
        printf("Error en el listen");
        exit(1);
    }

    pthread_t thread;
    for (;;) {
        printf("Escuchando\n");
        sock_conn = accept(sock_listen, NULL, NULL);
        printf("He recibido conexión\n");
        Clis.cliente[i].sock = sock_conn;
        pthread_create(&thread, NULL, AtenderCliente, &Clis.cliente[i].sock);
        i++;
    }

    mysql_close(conn);
    return 0;
}
