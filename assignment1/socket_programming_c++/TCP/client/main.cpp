//============================================================================
// Name        : file_client.cpp
// Author      : Lars Mortensen
// Version     : 1.0
// Description : file_client in C++, Ansi-style
//============================================================================

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>

#include "../functions/sockfunctions.h"

#define BUFFER_SIZE 1024

int main(int argc, char ** argv)
{
  int sockfd, portno, n;
  struct sockaddr_in serv_addr;
  struct hostent *server;
  char buffer[BUFFER_SIZE];

  // Check proper argument count
  if (argc == 3)
    error("No filename provided\n\r");
  if(argc < 4)
  {
    std::fprintf(stderr, "Usage: %s [hostname] [port] [filename]\n\r", argv[0]);
    exit(0);
  }

  // Create socket
  sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if (sockfd < 0)
    error("Couldn't open socket\n\r");

  // Get server
  server = gethostbyname(argv[1]);
  if (server == NULL)
    error("No such host\n\r");

  // Get port number from arguments
  portno = atoi(argv[2]);

  // Initialize serv_addr with zeroes
  bzero(&serv_addr, sizeof(serv_addr));

  // Setup serv_addr
  serv_addr.sin_family = AF_INET;
  serv_addr.sin_port = htons(portno);
  bcopy(server->h_addr_list[0],
      &serv_addr.sin_addr.s_addr,
      server->h_length);

  // Connect to server
  if (connect(sockfd, (struct sockaddr*) &serv_addr, sizeof(serv_addr)) < 0)
    error("Couldn't connect to server\n\r");

  // Get fourth argument
  sprintf(buffer, "%s", argv[3]);

  sendTextToSocket(buffer, sockfd);

  receiveFile(buffer, BUFFER_SIZE, sockfd);

  close(sockfd);

  return 0;
}

