import json
import matplotlib.pyplot as plt
import numpy as np
import math

"""
Draws the trajectories of each participant
Allows to compare the trajectories using ARCore motion tracking and ARCore cloud anchors technlogies 
"""

participants = 10  # Number of participantes
runs = 2  # Number of game each participant played
static = True  # Static or moving object game
end = False  # End of the game
lastpoint = (99.00, 99.00, 99.00)  # Last coordinate

# Camera coordinates using ARCore cloud anchors technology
anchors_static_camera_coordinates = []  # For the game with static objects
anchors_moving_camera_coordinates = []  # For the game with the moving object
np_anchors_static_camera_coordinates = []  # For the game with static objects (numpy)
np_anchors_moving_camera_coordinates = []  # For the game with the moving object (numpy)

# Camera coordinates using ARCore motion tracking technology
mt_static_camera_coordinates = []  # For the game with static objects
mt_moving_camera_coordinates = []  # For the game with the moving object
np_mt_static_camera_coordinates = []  # For the game with static objects (numpy)
np_mt_moving_camera_coordinates = []  # For the game with the moving object (numpy)

# Road line coordinates
anchors_static_line_coordinates = []  # Game with static objects and ARCore cloud anchors technology
anchors_moving_line_coordinates = []  # Game with the moving object and ARCore cloud anchors technology
np_anchors_static_line_coordinates = []  # Game with static objects and ARCore cloud anchors technology (numpy)
np_anchors_moving_line_coordinates = []  # Game with the moving object and ARCore cloud anchors technology (numpy)
mt_static_line_coordinates = []  # Game with static objects and ARCore motion tracking technology
mt_moving_line_coordinates = []  # Game with the moving object and ARCore motion tracking technology
np_mt_static_line_coordinates = []  # Game with static objects and ARCore motion tracking technology (numpy)
np_mt_moving_line_coordinates = []  # Game with the moving object and ARCore motion tracking technology (numpy)

# Moving object coordinates
anchors_movObj_coordinates = []  # using ARCore cloud anchors technology
np_anchors_movObj_coordinates = []  # using ARCore motion tracking technology
mt_movObj_coordinates = []  # using ARCore cloud anchors technology (numpy)
np_mt_movObj_coordinates = []  # using ARCore motion tracking technology (numpy)

file_count = 0

# Reads every participant monitorization file and writes the required information in the list variables
# Participants using ARCore cloud anchors technology
for participant in range(1, participants + 1):
    for run in range(0, runs):
        f = open("Monitorization/" + str(participant) + "/" + str(run) + ".txt", "r")
        lines = f.readlines()
        for line_aux in lines:
            for line in line_aux.split("}{"):
                if len(line_aux.split("}{")) > 1:
                    if line_aux.split("}{")[len(line_aux.split("}{")) - 1] == line:
                        line = "{" + line
                    else:
                        line = line + "}"
                parsed_line = json.loads(line)
                if parsed_line["type"] == "MOVING":
                    anchors_moving_camera_coordinates.append([])
                    anchors_moving_line_coordinates.append([])
                    anchors_movObj_coordinates.append([])
                    static = False
                elif parsed_line["type"] == "STATIC":
                    anchors_static_camera_coordinates.append([])
                    anchors_static_line_coordinates.append([])
                    static = True
                elif parsed_line["type"] == "END":
                    end = True
                    file_count += 1
                    break
                elif parsed_line["type"] == "CAMERA":
                    if static:
                        anchors_static_camera_coordinates[math.floor(file_count / 2)].append(
                            [float(parsed_line["obj"]["position_x"]),
                             float(parsed_line["obj"]["position_z"]),
                             float(parsed_line["obj"]["position_y"])])
                    else:
                        anchors_moving_camera_coordinates[math.floor(file_count / 2)].append(
                            [float(parsed_line["obj"]["position_x"]),
                             float(parsed_line["obj"]["position_z"]),
                             float(parsed_line["obj"]["position_y"])])
                    lastpoint = (float(parsed_line["obj"]["position_x"]), float(parsed_line["obj"]["position_z"]),
                                 float(parsed_line["obj"]["position_y"]))
                elif parsed_line["type"] == "MOVING_OBJECT":
                    anchors_movObj_coordinates[math.floor(file_count / 2)].append(
                        [float(parsed_line["obj"]["position_x"]),
                         float(parsed_line["obj"]["position_z"]),
                         float(parsed_line["obj"]["position_y"])])
                elif parsed_line["type"] == "LINE":
                    if static:
                        anchors_static_line_coordinates[math.floor(file_count / 2)].clear()
                        for point in parsed_line["line"]:
                            anchors_static_line_coordinates[math.floor(file_count / 2)].append([float(point["x"]),
                                                                                                float(point["z"]),
                                                                                                float(point["y"])])
                    else:
                        anchors_moving_line_coordinates[math.floor(file_count / 2)].clear()
                        for point in parsed_line["line"]:
                            anchors_moving_line_coordinates[math.floor(file_count / 2)].append([float(point["x"]),
                                                                                                float(point["z"]),
                                                                                                float(point["y"])])
            if end:
                f.close()
                end = False
                break

file_count = 0

# Reads every participant monitorization file and writes the required information in the list variables
# Participants using ARCore motion tracking technology
for participant in range(11, participants + 11):
    for run in range(0, runs):
        print(file_count)
        f = open("Monitorization/" + str(participant) + "/" + str(run) + ".txt", "r")
        lines = f.readlines()
        for line_aux in lines:
            for line in line_aux.split("}{"):
                if len(line_aux.split("}{")) > 1:
                    if line_aux.split("}{")[len(line_aux.split("}{")) - 1] == line:
                        line = "{" + line
                    else:
                        line = line + "}"
                parsed_line = json.loads(line)
                if parsed_line["type"] == "MOVING":
                    mt_moving_camera_coordinates.append([])
                    mt_moving_line_coordinates.append([])
                    mt_movObj_coordinates.append([])
                    static = False
                elif parsed_line["type"] == "STATIC":
                    mt_static_camera_coordinates.append([])
                    mt_static_line_coordinates.append([])
                    static = True
                elif parsed_line["type"] == "END":
                    end = True
                    file_count += 1
                    break
                elif parsed_line["type"] == "CAMERA":
                    if static:
                        mt_static_camera_coordinates[math.floor(file_count / 2)].append(
                            [float(parsed_line["obj"]["position_x"]),
                             float(parsed_line["obj"]["position_z"]),
                             float(parsed_line["obj"]["position_y"])])
                    else:
                        mt_moving_camera_coordinates[math.floor(file_count / 2)].append(
                            [float(parsed_line["obj"]["position_x"]),
                             float(parsed_line["obj"]["position_z"]),
                             float(parsed_line["obj"]["position_y"])])
                    lastpoint = (float(parsed_line["obj"]["position_x"]), float(parsed_line["obj"]["position_z"]),
                                 float(parsed_line["obj"]["position_y"]))
                elif parsed_line["type"] == "MOVING_OBJECT":
                    mt_movObj_coordinates[math.floor(file_count / 2)].append(
                        [float(parsed_line["obj"]["position_x"]),
                         float(parsed_line["obj"]["position_z"]),
                         float(parsed_line["obj"]["position_y"])])
                elif parsed_line["type"] == "LINE":
                    if static:
                        mt_static_line_coordinates[math.floor(file_count / 2)].clear()
                        for point in parsed_line["line"]:
                            mt_static_line_coordinates[math.floor(file_count / 2)].append([float(point["x"]),
                                                                                           float(point["z"]),
                                                                                           float(point["y"])])
                    else:
                        mt_moving_line_coordinates[math.floor(file_count / 2)].clear()
                        for point in parsed_line["line"]:
                            mt_moving_line_coordinates[math.floor(file_count / 2)].append([float(point["x"]),
                                                                                           float(point["z"]),
                                                                                           float(point["y"])])
            if end:
                f.close()
                end = False
                break

# Use the np.array() function on every list
for anchors_static_list in anchors_static_camera_coordinates:
    np_anchors_static_camera_coordinates.append(np.array(anchors_static_list).T)
for anchors_moving_list in anchors_moving_camera_coordinates:
    np_anchors_moving_camera_coordinates.append(np.array(anchors_moving_list).T)
for mt_static_list in mt_static_camera_coordinates:
    np_mt_static_camera_coordinates.append(np.array(mt_static_list).T)
for mt_moving_list in mt_moving_camera_coordinates:
    np_mt_moving_camera_coordinates.append(np.array(mt_moving_list).T)
for anchors_static_list in anchors_static_line_coordinates:
    np_anchors_static_line_coordinates.append(np.array(anchors_static_list).T)
for anchors_moving_list in anchors_moving_line_coordinates:
    np_anchors_moving_line_coordinates.append(np.array(anchors_moving_list).T)
for mt_static_list in mt_static_line_coordinates:
    np_mt_static_line_coordinates.append(np.array(mt_static_list).T)
for mt_moving_list in mt_moving_line_coordinates:
    np_mt_moving_line_coordinates.append(np.array(mt_moving_list).T)
for anchors_movObj_list in anchors_movObj_coordinates:
    np_anchors_movObj_coordinates.append(np.array(anchors_movObj_list).T)
for mt_movObj_list in mt_movObj_coordinates:
    np_mt_movObj_coordinates.append(np.array(mt_movObj_list).T)

# Draw the trajectory done by each user during the static objects game.
figXZStatic = plt.figure(0)
aXZStatic = figXZStatic.add_subplot(111)
for np_anchors_static in np_anchors_static_camera_coordinates:
    black_lines, = aXZStatic.plot(np_anchors_static[0], np_anchors_static[1], color="black")
for np_mt_static in np_mt_static_camera_coordinates:
    red_lines, = aXZStatic.plot(np_mt_static[0], np_mt_static[1], color="red")
aXZStatic.set_title("Path traveled by each user during the static objects game.")
aXZStatic.set_xlabel('X distance (m)')
aXZStatic.set_ylabel('Z distance (m)')
aXZStatic.legend([black_lines, red_lines], ["Users using cloud anchors mode", "Users using motion tracking mode"])

# Draw the trajectory done by each user during the moving object game.
figXZMoving = plt.figure(1)
aXZMoving = figXZMoving.add_subplot(111)
for np_anchors_moving in np_anchors_moving_camera_coordinates:
    black_lines, = aXZMoving.plot(np_anchors_moving[0], np_anchors_moving[1], color="black")
for np_mt_moving in np_mt_moving_camera_coordinates:
    red_lines, = aXZMoving.plot(np_mt_moving[0], np_mt_moving[1], color="red")
aXZMoving.set_title("Path traveled by each user during the moving object game.")
aXZMoving.set_xlabel('X distance (m)')
aXZMoving.set_ylabel('Z distance (m)')
aXZMoving.legend([black_lines, red_lines], ["Users using cloud anchors mode", "Users using motion tracking mode"])

# Draw the 3D trajectory done by each user during the static objects game.
fig3DStatic = plt.figure(2)
a3DStatic = fig3DStatic.add_subplot(111, projection='3d')
for np_anchors_static in np_anchors_static_camera_coordinates:
    black_lines, = a3DStatic.plot(np_anchors_static[0], np_anchors_static[1], np_anchors_static[2], color="black")
for np_mt_static in np_mt_static_camera_coordinates:
    red_lines, = a3DStatic.plot(np_mt_static[0], np_mt_static[1], np_mt_static[2], color="red")
a3DStatic.set_title("Path traveled (3D) by each user during the static objects game.")
a3DStatic.set_xlabel('X distance (m)')
a3DStatic.set_ylabel('Z distance (m)')
a3DStatic.set_zlabel('Y distance (m)')
aXZStatic.legend([black_lines, red_lines], ["Users using cloud anchors mode", "Users using motion tracking mode"])

# Draw the 3D trajectory done by each user during the moving object game.
fig3DMoving = plt.figure(3)
a3DMoving = fig3DMoving.add_subplot(111, projection='3d')
for np_anchors_moving in np_anchors_moving_camera_coordinates:
    black_lines, = a3DMoving.plot(np_anchors_moving[0], np_anchors_moving[1], np_anchors_moving[2], color="black")
for np_mt_moving in np_mt_moving_camera_coordinates:
    red_lines, = a3DMoving.plot(np_mt_moving[0], np_mt_moving[1], np_mt_moving[2], color="red")
a3DMoving.set_title("Path traveled (3D) by each user during the moving object game.")
a3DMoving.set_xlabel('X distance (m)')
a3DMoving.set_ylabel('Z distance (m)')
a3DMoving.set_zlabel('Y distance (m)')
aXZStatic.legend([black_lines, red_lines], ["Users using cloud anchors mode", "Users using motion tracking mode"])

# Draw the trajectory done by user 4 during the static objects game.
figXZComp = plt.figure(4)
aXZComp = figXZComp.add_subplot(111)
black_lines, = aXZComp.plot(np_anchors_static_camera_coordinates[3][0], np_anchors_static_camera_coordinates[3][1],
                            color="black")
red_lines, = aXZComp.plot(np_anchors_static_line_coordinates[3][0], np_anchors_static_line_coordinates[3][1], color="red")
aXZComp.set_title("Path traveled by user 4 during the static objects game.")
aXZComp.set_xlabel('X distance (m)')
aXZComp.set_ylabel('Z distance (m)')
aXZStatic.legend([black_lines, red_lines], ["User path", "Road path"])

# Draw the trajectory done by user 4 during the moving object game.
figXZComp = plt.figure(5)
aXZComp = figXZComp.add_subplot(111)
black_lines, = aXZComp.plot(np_anchors_moving_camera_coordinates[3][0], np_anchors_moving_camera_coordinates[3][1],
                            color="black")
red_lines, = aXZComp.plot(np_anchors_movObj_coordinates[3][0], np_anchors_movObj_coordinates[3][1],
                          color="red")
aXZComp.set_title("Path traveled by user 4 during the moving object game.")
aXZComp.set_xlabel('X distance (m)')
aXZComp.set_ylabel('Z distance (m)')
aXZStatic.legend([black_lines, red_lines], ["User path", "Moving object path"])

# Draw the trajectory done by user 13 during the static objects game.
figXZComp = plt.figure(6)
aXZComp = figXZComp.add_subplot(111)
black_lines, = aXZComp.plot(np_mt_static_camera_coordinates[2][0], np_mt_static_camera_coordinates[2][1], color="black")
red_lines, = aXZComp.plot(np_mt_static_line_coordinates[2][0], np_mt_static_line_coordinates[2][1], color="red")
aXZComp.set_title("Path traveled by user 13 during the static objects game.")
aXZComp.set_xlabel('X distance (m)')
aXZComp.set_ylabel('Z distance (m)')
aXZStatic.legend([black_lines, red_lines], ["User path", "Road path"])

# Draw the trajectory done by user 13 during the moving object game.
figXZComp = plt.figure(7)
aXZComp = figXZComp.add_subplot(111)
black_lines, = aXZComp.plot(np_mt_static_camera_coordinates[2][0], np_mt_static_camera_coordinates[2][1], color="black")
red_lines, = aXZComp.plot(np_mt_movObj_coordinates[2][0], np_mt_movObj_coordinates[2][1], color="red")
aXZComp.set_title("Path traveled by user 13 during the moving object game.")
aXZComp.set_xlabel('X distance (m)')
aXZComp.set_ylabel('Z distance (m)')
aXZStatic.legend([black_lines, red_lines], ["User path", "Moving object path"])

plt.show()