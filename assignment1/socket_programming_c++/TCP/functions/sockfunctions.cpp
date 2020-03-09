#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>

#include "sockfunctions.h"

void error(const char *mesg) 
{
  fprintf(stderr, "ERR: %s", mesg);
  exit(-1);
}

size_t getFileSize(char *fileName)
{
  std::streampos begin, end;
  std::ifstream file (fileName, std::ios::in | std::ios::binary);
  begin = file.tellg();
  file.seekg(0, std::ios::end);
  end = file.tellg();
  return end - begin;
}

void sendFile(size_t bufferSize, int sockfd)
{
  int n;
  char buffer[bufferSize];
  int bytesLeft, len;

  memset(buffer, 0, bufferSize);
  readTextFromSocket(buffer, bufferSize - 1, sockfd);
  bytesLeft = getFileSize(buffer);
  sendSizeToSocket(bytesLeft, sockfd);
  if (bytesLeft == 0)
  {
    printf("File %s doesn't exist.\n\r", buffer);
    return;
  }

  std::ifstream file (buffer, std::ios::in | std::ios::binary);
  while (bytesLeft > 0)
  {
    len = bytesLeft < (int)bufferSize ? bytesLeft : bufferSize;
    file.read(buffer, len);
    n = write(sockfd, buffer, len);
    if (n < 0)
      error("Couldn't write to socket\n\r");
    bytesLeft -= bufferSize;
  }
}

void receiveFile(char *fileName, size_t bufferSize, int sockfd)
{
  int n;
  char buffer[bufferSize];
  int bytesLeft, len;

  sendTextToSocket(fileName, sockfd);
  bytesLeft = readSizeFromSocket(sockfd);
  if (bytesLeft == 0)
    error("File doesn't exist\n\r");

  std::ofstream file (fileName, std::ios::out | std::ios::binary | std::ios::trunc);
  while (bytesLeft > 0)
  {
    len = bytesLeft < (int)bufferSize ? bytesLeft : bufferSize;
    printf("BytesLeft: %u\nLen: %u\n\n", bytesLeft, len);
    n = read(sockfd, buffer, len);
    if (n < 0)
      error("Couldn't read from socket\n\r");
    file.write(buffer, len);
    bytesLeft -= bufferSize;
  }
}

size_t readSizeFromSocket(int sockfd)
{
  size_t len;
  int n;
  n = read(sockfd, &len, sizeof(size_t));
  if (n < 0)
    error("Couldn't read size.\n\r");
  return len;
}

void sendSizeToSocket(size_t val, int sockfd)
{
  int n;
  n = write(sockfd, &val, sizeof(size_t));
  if (n < 0)
    error("Couldn't send size.\n\r");
}

void readTextFromSocket(char *buf, size_t length, int sockfd)
{
  char ch;
  int n;
  size_t len;

  /* Recieve byte length of text */
  len = readSizeFromSocket(sockfd);

  /* Loop as many times as amount of bytes to receive */
  for (size_t i = 0; i < len; ++i) {
    /* Read next byte */
    n = read(sockfd, &ch, sizeof(char));
    if (n < 0)
      error("Couldn't read from socket\n\r");
    /* If pos is still within allowable range, write byte to buf */
    if (i < length)
      buf[i] = ch;
  }
}

void sendTextToSocket(char *buf, int sockfd)
{
  int n;

  /* Send byte length of text */
  sendSizeToSocket(strlen(buf), sockfd);

  n = write(sockfd, buf, strlen(buf));
  if (n < 0)
    error("Couldn't write to socket\n\r");
}

