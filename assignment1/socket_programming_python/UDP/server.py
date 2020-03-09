import sys
import socket
import select

localIP = "0.0.0.0" # Bind to every available IP
localPort = int(sys.argv[1])
bufferSize = 256

# Create and setup socket
UDPServerSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

# Bind socket to IP
UDPServerSocket.bind((localIP, localPort))

# Open and get file descriptors for each file
f_l = open("/proc/loadavg", "r")
f_u = open("/proc/uptime", "r")

running = True
print("UDP Server up and listening!")

while(running):
    # Check whether stdin or socket is ready to be read from
    ready, _, _ = select.select([UDPServerSocket, sys.stdin], [], [], 1)

    # If socket has data
    if UDPServerSocket in ready:
        # Read from socket
        bytesAddressPair = UDPServerSocket.recvfrom(bufferSize)
        msg = bytesAddressPair[0]
        address = bytesAddressPair[1]

        # Decode received message
        msg = msg.decode()

        print("Received {} from {}.".format(msg, address))

        if msg == "l" or msg == "L": # Received L or l
            response = f_l.read()
            f_l.seek(0)
            UDPServerSocket.sendto(response[:-1].encode(), address)

        elif msg == "u" or msg == "U": # Received U or u
            response = f_u.read()
            f_u.seek(0)
            UDPServerSocket.sendto(response[:-1].encode(), address)

        elif msg == "q" or msg == "Q": # Received Q or q
            running = False # Shutdown server
            UDPServerSocket.sendto(str.encode("Server shutting down"), address)

        else: # Received unknown command
            UDPServerSocket.sendto(str.encode("Unknown command"), address)

    # If stdin has data
    if sys.stdin in ready:
        s = sys.stdin.readline()
        if s == "q\n" or s == "Q\n": # Received q or Q through stdin
            running = False # Shutdown server
