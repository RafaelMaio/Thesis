#!/usr/bin/env python3

import socket
import os
import json

"""
Server application: Waits for the game application connection
"""

#HOST = '192.168.1.103'  # Standard loopback interface address (localhost)
HOST = '192.168.43.113'
PORT = 4444    # Port to listen on (non-privileged ports are > 1023)

data_appender = ""  # Saves the message start if too long to read
end_of_message = "\"messageEnd\":\"split_by_this_message_end\"}"  # Message boundary

while True:
    print("Waiting for a connection.")
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:  # Waits for the a connection
        s.bind((HOST, PORT))
        s.listen()
        conn, addr = s.accept()
        with conn:  # Connection succeeded
            print('Connected by', addr)
            while True:  # Runs while the client does not close the connection
                data = conn.recv(8192).decode('utf-8')    # this returns a JSON-formatted String
                if not data:
                    break
                # Creates folders or/and files to store the players game performance
                if data.__contains__("REGISTRATION"):
                    parsed_data = json.loads(data)
                    try:
                        os.mkdir("Monitorization/"+parsed_data["user_name"])
                        os.mkdir("C:/Users/rafam/OneDrive/Ambiente de Trabalho/Cenas_Univ/UnityTry/auxiliarProj/Assets/Monitorization/" + parsed_data["user_name"])
                    except OSError:
                        print("Creation of the directory %s failed, already exists")
                    i = 0
                    while os.path.exists("Monitorization/" + parsed_data["user_name"] + "/" + str(i) + ".txt"):
                        i += 1
                    f1 = open("Monitorization/" + parsed_data["user_name"] + "/" + str(i) + ".txt", 'a')
                    f2 = open("C:/Users/rafam/OneDrive/Ambiente de Trabalho/Cenas_Univ/UnityTry/auxiliarProj/Assets/Monitorization/" + parsed_data["user_name"] + "/" + str(i) + ".txt", 'a')
                #  Write the message in files
                more_data = data.split(end_of_message)
                for d in more_data:
                    if data_appender != "":
                        if d != more_data[len(more_data) - 1]:
                            f1.write(data_appender + d + end_of_message + "\n")
                            f2.write(data_appender + d + end_of_message + "\n")
                            data_appender = ""
                        else:
                            if d != '':
                                data_appender += d
                    else:
                        if d != more_data[len(more_data) - 1]:
                            f1.write(d + end_of_message + "\n")
                            f2.write(d + end_of_message + "\n")
                        else:
                            if d != '':
                                data_appender = d
            # Closes the files
            f1.close()
            f2.close()