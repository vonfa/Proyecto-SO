#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql/mysql.h>

#define DATABASE "Juego"
#define USER "tu_usuario_db"
#define PASSWORD "tu_contraseña_db"
#define HOST "localhost"

void add_user_to_db(const char* username, const char* password) {
    MYSQL *conn;
    MYSQL_STMT *stmt;
    MYSQL_BIND bind[2];

    conn = mysql_init(NULL);
    if (conn == NULL) {
        fprintf(stderr, "mysql_init() falló\n");
        return;
    }

    if (mysql_real_connect(conn, HOST, USER, PASSWORD, DATABASE, 0, NULL, 0) == NULL) {
        fprintf(stderr, "mysql_real_connect() falló: %s\n", mysql_error(conn));
        mysql_close(conn);
        return;
    }

    const char* query = "INSERT INTO Jugador (Nombre_de_usuario, Contraseña) VALUES (?, ?)";

    // Preparar la consulta para evitar inyecciones SQL
    stmt = mysql_stmt_init(conn);
    if (!stmt) {
        fprintf(stderr, "mysql_stmt_init() falló\n");
        mysql_close(conn);
        return;
    }

    if (mysql_stmt_prepare(stmt, query, strlen(query))) {
        fprintf(stderr, "mysql_stmt_prepare() falló: %s\n", mysql_stmt_error(stmt));
        mysql_stmt_close(stmt);
        mysql_close(conn);
        return;
    }

    // Vinculamos los parámetros
    memset(bind, 0, sizeof(bind));

    // Parámetro 1: username
    bind[0].buffer_type = MYSQL_TYPE_STRING;
    bind[0].buffer = (char *)username;
    bind[0].buffer_length = strlen(username);

    // Parámetro 2: password
    bind[1].buffer_type = MYSQL_TYPE_STRING;
    bind[1].buffer = (char *)password;
    bind[1].buffer_length = strlen(password);

    if (mysql_stmt_bind_param(stmt, bind)) {
        fprintf(stderr, "mysql_stmt_bind_param() falló: %s\n", mysql_stmt_error(stmt));
        mysql_stmt_close(stmt);
        mysql_close(conn);
        return;
    }

    // Ejecutamos la consulta
    if (mysql_stmt_execute(stmt)) {
        fprintf(stderr, "mysql_stmt_execute() falló: %s\n", mysql_stmt_error(stmt));
    } else {
        printf("Usuario %s agregado correctamente.\n", username);
    }

    // Limpiar
    mysql_stmt_close(stmt);
    mysql_close(conn);
}

int main(int argc, char* argv[]) {
    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;
    char peticion[512];

    // INICIALIZACIONES
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        perror("Error creando socket");
        exit(1);
    }

    memset(&serv_adr, 0, sizeof(serv_adr));
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(9050);

    if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0) {
        perror("Error en el bind");
        close(sock_listen);
        exit(1);
    }

    if (listen(sock_listen, 3) < 0) {
        perror("Error en el listen");
        close(sock_listen);
        exit(1);
    }

    for (;;) {
        printf("Escuchando\n");

        sock_conn = accept(sock_listen, NULL, NULL);
        if (sock_conn < 0) {
            perror("Error en el accept");
            continue;
        }
        printf("He recibido conexión\n");

        // Recibir la petición
        int recv_len = recv(sock_conn, peticion, sizeof(peticion) - 1, 0);
        if (recv_len <= 0) {
            perror("Error en recv");
            close(sock_conn);
            continue;
        }

        peticion[recv_len] = '\0';  // Asegurar terminación de la cadena

        // Suponemos que el formato de la petición es "username:password"
        char username[11], password[256];
        if (sscanf(peticion, "%10[^:]:%255s", username, password) != 2) {
            printf("Error en el formato de la petición\n");
            close(sock_conn);
            continue;
        }

        // Agregar el usuario a la base de datos
        add_user_to_db(username, password);

        // Enviar respuesta al cliente
        char respuesta[] = "Usuario agregado correctamente";
        send(sock_conn, respuesta, strlen(respuesta), 0);

        // Se acabó el servicio para este cliente
        close(sock_conn);
    }

    return 0;
}