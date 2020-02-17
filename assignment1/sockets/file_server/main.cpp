//============================================================================
// Name        : file_server.cpp
// Author      : Lars Mortensen
// Version     : 1.0
// Description : file_server in C++, Ansi-style
//============================================================================

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

#include "../functions/sockfunctions.h"

#define BUFFER_SIZE 1024

int main(int argc, char *argv[])
{
  int sockfd, newsockfd, portno, n;
  char buffer[BUFFER_SIZE];
  struct sockaddr_in serv_addr, cli_addr;
  socklen_t clilen = sizeof(cli_addr);

  // Check argument count
  if (argc < 2) {
    fprintf(stderr,"Usage: %s [portno]\n\r", argv[0]);
    exit(1);
  }

  // Create socket
  sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if (sockfd < 0) 
    error("ERROR opening socket");

  // Get portno
  portno = atoi(argv[1]);

  // Setup serv_addr
  memset(&serv_addr, 0, sizeof(serv_addr));
  serv_addr.sin_family = AF_INET;
  serv_addr.sin_addr.s_addr = INADDR_ANY;
  serv_addr.sin_port = htons(portno);

  // Bind socket to address
  if (bind(sockfd, (struct sockaddr*) &serv_addr, sizeof(serv_addr)) < 0)
    error("ERROR: Coulnd't bind socket to address\n\r");

  // Start listening on socket
  listen(sockfd, 5);

  bool running = true;

  // Main server loop
  while (running)
  {
    // Block until new connection is established
    newsockfd = accept(sockfd, (struct sockaddr*) &cli_addr, &clilen);

    // Clear buffer and read filename
    bzero(buffer, BUFFER_SIZE);
    readTextFromSocket(buffer, BUFFER_SIZE - 1, newsockfd);

    if (strcmp(buffer, "exit") == 0)
      running = false;

    // Output recieved string
    std::printf("%d: Message recieved:\n\t%s\n\r", portno, buffer);

    if (running)
      sendFile(BUFFER_SIZE, newsockfd);

    close(newsockfd);
  }

  close(sockfd);

  return 0;
}

