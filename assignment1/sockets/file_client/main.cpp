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
#include <iknlib.h>

#define BUFFER_SIZE 1024

void receiveFile(std::string fileName, int sockfd);

int main(int argc, char ** argv)
{
  int sockfd, portno, n;
  struct sockaddr_in serv_addr;
  struct hostent *server;
  char buffer[BUFFER_SIZE];

  // Check proper argument count
  if (argc == 2)
  {
    std::fprintf(stderr, "ERROR: No port provided\n\r");
    exit(1);
  }
  else if (argc < 3)
  {
    std::fprintf(stderr, "Usage: %s hostname port\n\r", argv[0]);
    exit(0);
  }

  // Create socket
  sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if (sockfd < 0)
    error("ERROR: Couldn't open socket\n\r");

  // Get server
  server = gethostbyname(argv[1]);
  if (server == NULL)
    error("ERROR: No such host\n\r");

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
    error("ERROR: Couldn't connect to server\n\r");

  while (true)
  {
    printf("Enter filename:\n\r");
    bzero(buffer, BUFFER_SIZE);
    std::cin >> buffer;

    n = write(sockfd, buffer, strlen(buffer));
    if (n < 0)
      error("ERROR: Couldn't write to socket\n\r");

    if (strcmp(buffer, "exit") == 0)
      break;
  }

  close(sockfd);

  return 0;
}

/**
 * Modtager filstørrelsen og udskriver meddelelsen: "Filen findes ikke" hvis størrelsen = 0
 * ellers
 * Åbnes filen for at skrive de bytes som senere modtages fra serveren (HUSK kun selve filnavn)
 * Modtag og gem filen i blokke af 1000 bytes indtil alle bytes er modtaget.
 * Luk filen, samt input output streams
 *
 * @param fileName Det fulde filnavn incl. evt. stinavn
 * @param sockfd Stream for at skrive til/læse fra serveren
 */
void receiveFile(std::string fileName, int sockfd)
{
  // TO DO Your own code
}

