#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>

#define MAX_USERS 100
#define MAX_NAME_LENGTH 20

// Estructura para almacenar los usuarios registrados
char registered_users[MAX_USERS][MAX_NAME_LENGTH];
int user_count = 0;

// Función para verificar si un usuario ya está registrado
int is_user_registered(const char *username) {
    for (int i = 0; i < user_count; i++) {
        if (strcmp(registered_users[i], username) == 0) {
            return 1; // Usuario ya registrado
        }
    }
    return 0; // Usuario no registrado
}

// Función para registrar un nuevo usuario
void register_user(const char *username) {
    if (user_count < MAX_USERS) {
        strcpy(registered_users[user_count], username);
        user_count++;
    }
}

int main(int argc, char *argv[]) {
    int sock_conn, sock_listen, ret;
    struct sockaddr_in serv_adr;
    char peticion[512];
    char respuesta[512];

    // Inicializaciones
    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0) {
        printf("Error creando socket\n");
        return 1; // Salir en caso de error
    }

    memset(&serv_adr, 0, sizeof(serv_adr)); // Inicializar a cero
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(9050);

    if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0) {
        printf("Error al bind\n");
        return 1; // Salir en caso de error
    }

    if (listen(sock_listen, 3) < 0) {
        printf("Error en el Listen\n");
        return 1; // Salir en caso de error
    }

    for (;;) {
        printf("Escuchando\n");
        sock_conn = accept(sock_listen, NULL, NULL);
        printf("He recibido conexión\n");

        int terminar = 0;

        while (terminar == 0) {
            ret = read(sock_conn, peticion, sizeof(peticion));
            printf("Recibido\n");

            peticion[ret] = '\0';
            printf("Petición: %s\n", peticion);

            char *p = strtok(peticion, "/");
            int codigo = atoi(p); // Obtenemos el código de la petición

            char nombre[MAX_NAME_LENGTH];
            if (codigo != 0) {
                p = strtok(NULL, "/");
                strcpy(nombre, p); // Obtenemos el nombre
                printf("Código: %d, Nombre: %s\n", codigo, nombre);
            }

            if (codigo == 0) {
                terminar = 1; // Desconexión
            } else if (codigo == 1) { // Registro
                if (is_user_registered(nombre)) {
                    sprintf(respuesta, "Error: Usuario '%s' ya registrado", nombre);
                } else {
                    register_user(nombre);
                    sprintf(respuesta, "Registro exitoso: Usuario '%s' registrado correctamente", nombre);
                }
            }

            if (codigo != 0) {
                printf("Respuesta: %s\n", respuesta);
                write(sock_conn, respuesta, strlen(respuesta)); // Enviamos respuesta al cliente
            }
        }
        close(sock_conn); // Se acabó el servicio para este cliente
    }
}
