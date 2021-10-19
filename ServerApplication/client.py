import socket

"""
Test script: Used to verify the connection to the server. 
Also verifies the message sending.
"""

HOST = '192.168.1.102'  # The server's hostname or IP address
PORT = 4444      # The port used by the server

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))
    s.sendall(b'Hello, world')
    data = s.recv(1024)

print('Received', repr(data))