import sys
from socket import *
from lib import Lib

HOST = '0.0.0.0'
PORT = 9000
BUFSIZE = 1000
def main(argv):
	servSock = socket(AF_INET, SOCK_STREAM)
	servSock.bind((HOST, PORT))
	servSock.listen(1)
	print("The server is ready to recieve\n")
	while True:
		conSock, addr = servSock.accept()
		print("client connected\n")
		msg = Lib.readTextTCP(conSock)
		while msg != "exit":
			print("Command received: %s\n" % msg)
		
			fileSize = Lib.check_File_Exists(msg)
	
			if fileSize > 0:
				
				sendFile(msg, fileSize, conSock)
			else:
				Lib.writeTextTCP(str(fileSize), conSock)
			msg = Lib.readTextTCP(conSock)
		conSock.close()
		print("Connection closed")
	
	

def sendFile(fileName,  fileSize,  conn):
	Lib.writeTextTCP(str(fileSize), conn)
	file = open(fileName, 'rb')
	data = file.read(BUFSIZE)
	while data:
		conn.send(data)
		data = file.read(BUFSIZE)
    
if __name__ == "__main__":
   main(sys.argv[1:])
