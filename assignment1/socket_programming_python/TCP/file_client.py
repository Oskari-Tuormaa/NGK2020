import sys
from socket import *
from lib import Lib

PORT = 9000
BUFSIZE = 1000

def main(argv):
	if len(argv) > 0:
		SERVER = argv[0]
	else:
		print("Benyt formatet 'python file_client.py <IP>'\n")
		sys.exit()
	sock = socket(AF_INET, SOCK_STREAM)
	sock.connect((SERVER,PORT))
	print("Forbundet til server\n")
	cmd = input("Indtast filnavn. Ved afslutning skriv 'exit'\n")
	while cmd != "exit":
		Lib.writeTextTCP(cmd,sock)

		fileSize = Lib.getFileSizeTCP(sock)
		print(fileSize)
	
		if fileSize > 0:
			receiveFile(cmd, sock, fileSize)
		else:
			print("Fil ikke fundet\n")
		cmd = input("Indtast filnavn. Ved afslutning skriv 'exit'\n")
	Lib.writeTextTCP(cmd,sock)
	sock.close()
	sys.exit()
		
    
def receiveFile(nm,  conn, siz):
	bytesRead = 0
	fileName = Lib.extractFilename(nm)
	file = open(fileName, 'wb')

	while bytesRead < siz:
		buff = conn.recv(BUFSIZE)
		file.write(buff)
		bytesRead = bytesRead+len(buff)
	print("Transfer complete\n")

if __name__ == "__main__":
   main(sys.argv[1:])
