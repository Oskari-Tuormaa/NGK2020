#include <sys/socket.h>
#include <netinet/in.h>
#include <iostream>
#include <string.h>

#define BUFFER_SIZE 256

int main(int argc, char *argv[])
{
  int portno, socket_fd;
  unsigned int len;
  char buffer[BUFFER_SIZE];
  struct sockaddr_in server_addr, client_addr;

  if (argc == 2) {
    portno = atoi(argv[1]);
  } else {
    printf("Usage: %s [portno]\n\r", argv[0]);
    return -1;
  }

  len = sizeof(client_addr);

  socket_fd = socket(AF_INET, SOCK_DGRAM, 0);

  memset(&server_addr, 0, sizeof(server_addr));
  memset(&client_addr, 0, sizeof(client_addr));

  server_addr.sin_family = AF_INET;
  server_addr.sin_addr.s_addr = INADDR_ANY;
  server_addr.sin_port = htons(portno);

  bind(socket_fd, (const struct sockaddr *)&server_addr, sizeof(server_addr));

  printf("Server up and running!\n\r");

  bool running = true;
  while (running) {
    recvfrom(socket_fd, buffer, BUFFER_SIZE, 0,
      (struct sockaddr *) &client_addr, &len);

    printf("Got a message: %s", buffer);
  }
  
  return 0;
}
