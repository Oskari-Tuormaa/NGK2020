#include <stdlib.h>

void error(const char *mesg);

size_t getFileSize (char *fileName);
void receiveFile   (char *fileName, size_t bufferSize, int sockfd);
void sendFile      (size_t bufferSize, int sockfd);

size_t readSizeFromSocket (int sockfd);
void   sendSizeToSocket   (size_t val, int sockfd);
void   readTextFromSocket (char *buf,  size_t length, int sockfd);
void   sendTextToSocket   (char *buf,  int sockfd);
