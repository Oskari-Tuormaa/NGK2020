import sys
import socket
import select

localIP = "127.0.0.1"
localPort = int(sys.argv[1])
bufferSize = 256

UDPServerSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

UDPServerSocket.bind((localIP, localPort))

f_l = open("/proc/loadavg", "r")
f_u = open("/proc/uptime", "r")

running = True
print("UDP Server up and listening!")

while(running):
    ready, _, _ = select.select([UDPServerSocket, sys.stdin], [], [], 1)

    if UDPServerSocket in ready:
        bytesAddressPair = UDPServerSocket.recvfrom(bufferSize)
        msg = bytesAddressPair[0]
        address = bytesAddressPair[1]

        msg = msg.decode()

        if msg == "l" or msg == 'L':
            response = f_l.read()
            f_l.seek(0)
            UDPServerSocket.sendto(response[:-1].encode(), address)

        elif msg == "u" or msg == "U":
            response = f_u.read()
            f_u.seek(0)
            UDPServerSocket.sendto(response[:-1].encode(), address)

        elif msg == "q" or msg == "Q":
            running = False
            print("Received q from {}, shutting down".format(address))
            UDPServerSocket.sendto(str.encode("Server shutting down"), address)

        else:
            UDPServerSocket.sendto(str.encode("Unknown command"), address)

    if sys.stdin in ready:
        s = sys.stdin.readline()
        if s == "q\n" or s == "Q\n":
            running = False
