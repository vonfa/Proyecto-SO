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

// Funci?n para conectar a la base de datos
void conectarBD() {
    conn = mysql_init(NULL);
    if (conn == NULL) {
        printf("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
        exit(1);
    }
    
    conn = mysql_real_connect(conn, "shiva2.upc.es", "root", "mysql", "M9_BBDDServidor", 0,NULL, 0);
    if (conn == NULL) {
        printf("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
        exit(1);
    }
}

// Funci?n para ejecutar consultas SQL
void ejecutarConsultaSQL(char *sql) {
    if (mysql_query(conn, sql) != 0) {
        fprintf(stderr, "Error en la consulta: %s\n", mysql_error(conn));
        mysql_close(conn);
        exit(EXIT_FAILURE);
    }
}

// Funci?n para atender clientes
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
            terminar = 1;
            // Eliminar el cliente de la lista
            pthread_mutex_lock(&mutex);
            for (int j = 0; j < Clis.numero_clientes; j++) {
                if (Clis.cliente[j].sock == sock_conn) {
                    // Desplazar todos los clientes a la izquierda
                    for (int k = j; k < Clis.numero_clientes - 1; k++) {
                        Clis.cliente[k] = Clis.cliente[k + 1];
                    }
                    Clis.numero_clientes--;
                    break;
                }
            }
            pthread_mutex_unlock(&mutex);

        } else if (codigo == 1) { // Registro
            char Nombre[20], Contrasenya[20];
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
			

        } else if (codigo == 2) { // Iniciar sesion
            p = strtok(NULL, "/");
            strcpy(Nombre, p);
            p = strtok(NULL, "/");
            strcpy(Contrasenya, p);

            char comando[256];
            sprintf(comando, "SELECT Name FROM Player WHERE Name='%s' AND Password='%s'", Nombre, Contrasenya);
            if (mysql_query(conn, comando) == 0) {
                MYSQL_RES *result = mysql_store_result(conn);
                if (mysql_num_rows(result) > 0) {
                    sprintf(respuesta, "2/Login exitoso para %s", Nombre);
                    // Agregar el cliente a la lista de conectados
                    pthread_mutex_lock(&mutex);
                    strcpy(Clis.cliente[Clis.numero_clientes].Nombre, Nombre);
                    Clis.cliente[Clis.numero_clientes].sock = sock_conn;
                    Clis.numero_clientes++;
                    pthread_mutex_unlock(&mutex);
                } else {
                    sprintf(respuesta, "2/Error: Usuario o contraseña incorrectos");
                }
                mysql_free_result(result);
            } else {
                sprintf(respuesta, "2/Error al realizar la consulta");
            }
            write(sock_conn, respuesta, strlen(respuesta));
	 }else if (codigo ==4)
	{
            pthread_mutex_lock(&mutex);
            strcpy(respuesta, "4/Jugadores conectados:");
            for (int j = 0; j < Clis.numero_clientes; j++) {
                strcat(respuesta, " ");
                strcat(respuesta, Clis.cliente[j].Nombre);
            }
            pthread_mutex_unlock(&mutex);
            write(sock_conn, respuesta, strlen(respuesta));
        }
    

	
	else if (codigo == 5) { // Solicitar porcentaje de victorias
			p = strtok(NULL, "/"); // Obtener el nombre del jugador
    strcpy(Nombre, p);
	char consulta[512];    // Buffer para la consulta SQL
	int err;               // Variable para almacenar errores en las consultas
	MYSQL_RES *resultado;  // Resultado de la consulta
	MYSQL_ROW row;         // Fila de datos recuperada
    // Consulta para contar las participaciones del jugador
	sprintf(consulta, "SELECT COUNT(*) FROM Participation WHERE Player = '%s'", Nombre);
    err = mysql_query(conn, consulta);
    
    if (err == 0) {
        resultado = mysql_store_result(conn);
        row = mysql_fetch_row(resultado);
        
        if (row == NULL || atoi(row[0]) == 0) {
            sprintf(respuesta, "5/El jugador %s no ha participado en ninguna partida.", Nombre);
        } else {
            int total_participaciones = atoi(row[0]); // Total de participaciones
            mysql_free_result(resultado);

            // Consulta para contar las victorias del jugador
			sprintf(consulta, "SELECT COUNT(*) FROM Game WHERE Winner = '%s'", Nombre);
            err = mysql_query(conn, consulta);

            if (err == 0) {
                resultado = mysql_store_result(conn);
                row = mysql_fetch_row(resultado);
                
                int total_victorias = atoi(row[0]); // Total de victorias
                mysql_free_result(resultado);

                // Calcular el porcentaje de victorias
                float porcentaje_victorias = ((float)total_victorias / total_participaciones) * 100;

                // Enviar la respuesta con el porcentaje
                sprintf(respuesta, "5/El jugador %s ha participado en %d partidas y ha ganado %d partidas. Porcentaje de victorias: %.2f%%.", Nombre, total_participaciones, total_victorias, porcentaje_victorias);
            } else {
                sprintf(respuesta, "5/Error al consultar las victorias.");
            }
        }
    } else {
        sprintf(respuesta, "5/Error al consultar las participaciones.");
    }

    // Enviar respuesta al cliente
    write(sock_conn, respuesta, strlen(respuesta));
}
 else if (codigo == 6) { // Mayor racha de victorias consecutivas
    char consulta[512];
    sprintf(consulta,
        "SELECT Winner, MAX(Streak) AS MaxStreak "
        "FROM ( "
        "    SELECT Winner, EndDateTime, "
        "        CASE WHEN @prev_winner = Winner AND DATE(@prev_date) = DATE(EndDateTime - INTERVAL 1 DAY) "
        "        THEN @streak := @streak + 1 "
        "        ELSE @streak := 1 END AS Streak, "
        "        @prev_winner := Winner, @prev_date := EndDateTime "
        "    FROM Game "
        "    CROSS JOIN (SELECT @prev_winner := NULL, @prev_date := NULL, @streak := 0) AS vars "
        "    ORDER BY Winner, EndDateTime "
        ") AS Streaks "
        "GROUP BY Winner "
        "ORDER BY MaxStreak DESC "
        "LIMIT 1;"
    );

    if (mysql_query(conn, consulta) == 0) {
        MYSQL_RES* result = mysql_store_result(conn);
        MYSQL_ROW row;

        if (result && (row = mysql_fetch_row(result))) {
            sprintf(respuesta, "6/El jugador con la mayor racha de victorias es %s con %s victorias consecutivas.", row[0], row[1]);
        }
        else {
            sprintf(respuesta, "6/No se encontraron datos de rachas.");
        }
        mysql_free_result(result);
    }
    else {
        sprintf(respuesta, "6/Error en la consulta.");
    }

    // Enviar respuesta al cliente
    write(sock_conn, respuesta, strlen(respuesta));

}
	if (codigo==2)
	{
		pthread_mutex_lock( &mutex ); //No me interrumpas ahora
		contador = contador +1;
		pthread_mutex_unlock( &mutex); //ya puedes interrumpirme
	}
	sprintf (respuesta,"%d",contador);
	
	
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
    serv_adr.sin_port = htons(9095);

    if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0) {
        printf("Error en el bind");
        exit(1);
    }

    if (listen(sock_listen, 3) < 0) {
        printf("Error en el Listen");
    }

    // Inicializar la lista de clientes
    Clis.numero_clientes = 0;

    // Bucle para atender a los clientes
    for (;;) {
        printf("Escuchando\n");
        
        sock_conn = accept(sock_listen, NULL, NULL);
        printf("He recibido conexion\n");
        
        // Crear thread y decirle lo que tiene que hacer
        pthread_t thread; // Mover la declaración aquí
        pthread_create(&thread, NULL, AtenderCliente, &sock_conn);
    }

    mysql_close(conn);
    return 0;
}

