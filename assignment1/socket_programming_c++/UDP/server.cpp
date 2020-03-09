#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <iostream>
#include <string.h>
#include <unistd.h>

#define BUFFER_SIZE 256

int main(int argc, char *argv[])
{
  int socket_fd, s;
  ssize_t nread;
  socklen_t len;
  char buffer[BUFFER_SIZE];
  struct sockaddr_storage peer_addr;
  struct addrinfo hints, *res, *rp;

  if (argc != 2) {
    printf("Usage: %s [portno]\n\r", argv[0]);
    return -1;
  }

  memset(&hints, 0, sizeof(hints));
  hints.ai_family = AF_INET;
  hints.ai_socktype = SOCK_DGRAM;
  hints.ai_flags = AI_PASSIVE;
  hints.ai_protocol = IPPROTO_UDP;
  hints.ai_canonname = NULL;
  hints.ai_addr = NULL;
  hints.ai_next = NULL;

  s = getaddrinfo(NULL, argv[1], &hints, &res);

  for (rp = res; rp != NULL; rp = rp->ai_next) {
    socket_fd = socket(rp->ai_family, rp->ai_socktype, rp->ai_protocol);
    if (socket_fd < 0)
      continue;

    if (bind(socket_fd, rp->ai_addr, rp->ai_addrlen) == 0)
      break;

    close(socket_fd);
  }

  if (rp == NULL) {
    fprintf(stderr, "Couldn't bind to address\n\r");
    exit(EXIT_FAILURE);
  }

  freeaddrinfo(res);

  printf("Server up and running!\n\r");

  bool running = true;
  while (running) {
    nread = recvfrom(socket_fd, buffer, BUFFER_SIZE, 0,
      (struct sockaddr *) &peer_addr, &len);

    char host[NI_MAXHOST], service[NI_MAXSERV];

    s = getnameinfo((struct sockaddr *) &peer_addr, len,
        host, NI_MAXHOST,
        service, NI_MAXSERV,
        NI_NUMERICSERV);

    if (s == 0)
      printf("Received %zd bytes from %s:%s\n\r", nread, host, service);
    else
      fprintf(stderr, "getnameinfo: %s\n\r", gai_strerror(s));
  }
  
  return 0;
}
