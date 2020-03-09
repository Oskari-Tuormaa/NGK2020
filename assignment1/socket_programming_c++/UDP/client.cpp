#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <unistd.h>
#include <iostream>
#include <string.h>

#define BUFFER_SIZE 256

int main(int argc, char *argv[])
{
  int socket_fd;
  char buffer[BUFFER_SIZE], *host, *portno;
  socklen_t server_addr_len;
  struct sockaddr_in server_addr;
  struct addrinfo hints, *res, *rp;

  if (argc != 4) {
    printf("Usage: %s [IP] [port] [msg]\n\r", argv[0]);
    return -1;
  }

  host = argv[1];
  portno = argv[2];
  sprintf(buffer, "%s", argv[3]);

  bzero(&hints, sizeof(hints));
  hints.ai_family = AF_INET;
  hints.ai_socktype = SOCK_DGRAM;
  hints.ai_protocol = IPPROTO_UDP;

  getaddrinfo(host, portno, &hints, &res);

  for (rp = res; rp != NULL; rp = rp->ai_next) {
    socket_fd = socket(res->ai_family, res->ai_socktype, res->ai_protocol);
    if (socket_fd < 0)
      continue;

    if (connect(socket_fd, rp->ai_addr, rp->ai_addrlen) != -1)
      break;

    close(socket_fd);
  }

  if (rp == NULL) {
    fprintf(stderr, "Couldn't connect\n\r");
    exit(EXIT_FAILURE);
  }

  memcpy(&server_addr, res->ai_addr, res->ai_addrlen);
  server_addr_len = res->ai_addrlen;

  freeaddrinfo(rp);

  sendto(socket_fd, buffer, strlen(buffer), 0, (const sockaddr*)&server_addr, server_addr_len);

  return 0;
}
