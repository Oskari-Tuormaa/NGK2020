import sys
import socket

serverAddress = sys.argv[1]
serverPort = int(sys.argv[2])
msg = sys.argv[3]
bufferSize = 256

# Create and setup socket
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

# Send encoded message through socket
UDPClientSocket.sendto(str.encode(msg), (serverAddress, serverPort))

# Read and print response
s = UDPClientSocket.recv(bufferSize)
print(s.decode())
