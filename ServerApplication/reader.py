import json
import math
import time

import matplotlib.pyplot as plt
import numpy as np
from matplotlib import animation

"""
First perfomance visualization module -> Adapted to be used in Unity.
"""

f = open("Monitorization/5/0.txt", "r")  # File to read

lines = f.readlines()  # Real all line from the file

animator = False  # Animate the perfomance visualization

white_line = []  # Stores the white line coordinates
red_line = []  # Stores the red line coordinates
camera_scat = []  # Camera scatter
moving_scat = []  # Moving object scatter
goal_scat = []  # Passage point object scatters
goal_coords = []  # Passage point objects coordinates
lookaway_scat = []  # Spotlight object scatters
lookaway_coords = []  # Spotlight objects coordinates
stop_scat = []  # Stop object scatters
stop_coords = []  # Stop objects coordinates
dodge_scat = []  # Obstacle scatters
dodge_coords = []  # Obstacles coordinates

moving_or_static = []  # Moving or static game

# Draw figure
fig = plt.figure(0)
a = fig.add_subplot(111, projection='3d')
a.set_xlim([-10, 10])
a.set_ylim([-10, 10])
a.set_zlim([-10, 10])

start_time = [time.time()]  # Initial timestamp

# Message types: REGISTRATION, CAMERA, LOOKAWAY_SUCC, LOOKAWAY_FAIL, SPOTLIGHT_OBJ, STOP_SUCC, STOP_FAIL, STOP_OBJ,
#               DODGE_OBJ, DODGE_FAIL, MOVING_OBJECT, STATIC, MOVING, LINE, END, OUT_OF_ROAD, HINT, GOAL, CHECKPOINT


# Write game information
info_text = a.text(x=-10, y=10, z=15, s='Score: 0, Num_Checkpoints: 0, Time: 00:00')
more_text = a.text(x=-5, y=9, z=15, s='', c="green")
less_text = a.text(x=-5, y=5, z=15, s='', c="red")
counter = 0


def animation_frame3D(i):
    """
    Draw in figure based on the current file line.
    :param i: file line.
    """
    line_aux = lines[i]
    for line in line_aux.split("}{"):
        if len(line_aux.split("}{")) > 1:
            if line_aux.split("}{")[len(line_aux.split("}{")) - 1] == line:
                line = "{" + line
            else:
                line = line + "}"
        parsed_line = json.loads(line)
        if parsed_line["type"] == "CAMERA":  # Processing the CAMERA message
            if i % 10 == 0:  # Draw scatter every 10 lines
                if len(camera_scat) > 0:  # Deleter previous scatter
                    camera_scat[0].remove()
                    camera_scat.clear()
                camera_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                             parsed_line["obj"]["position_y"], color="blue"))
            if len(parsed_line["line"]) != 0:  # Verify if the game started
                red_line.append(
                    (parsed_line["obj"]["position_x"], parsed_line["line"][0]["y"], parsed_line["obj"]["position_z"]))
                if i % 30 == 0:  # Draw red line every 30 lines
                    np_red_line = np.array(red_line).T
                    a.plot(np_red_line[0], np_red_line[2], np_red_line[1], color="red", linewidth=0.5)
                    red_line.clear()
        elif parsed_line["type"] == "LINE":  # Processing the LINE message
            continue_here = len(white_line)
            white_line.clear()
            i = 0
            for point in parsed_line["line"]:
                if i >= continue_here:
                    white_line.append((point["x"], point["y"], point["z"]))
                i += 1
            np_white_line = np.array(white_line).T
            a.plot(np_white_line[0], np_white_line[2], np_white_line[1], color="black", linewidth=10)  # Draw road
            if moving_or_static[0] == "STATIC":  # Draw white line if game with static objects
                a.plot(np_white_line[0], np_white_line[2], np_white_line[1], color="white", linewidth=0.5)
        elif parsed_line["type"] == "GOAL":  # Processing the GOAL message: Draws the passage point object
            goal_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                       parsed_line["obj"]["position_y"],
                                       color="green", alpha=0.9, zorder=4))
            goal_coords.append((parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                parsed_line["obj"]["position_y"]))
        elif parsed_line["type"] == "CHECKPOINT":  # Processing the CHECKPOINT message:
            # Erases the passage point object. Score feedback.
            index = check_closest(goal_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                parsed_line["obj"]["position_y"]))
            goal_scat[index].remove()
            goal_scat.remove(goal_scat[index])
            goal_coords.remove(goal_coords[index])
            more_text.set_text("+" + str(float(parsed_line["gameState"]["score"]) -
                                         float(info_text.get_text().split(',')[0].replace("Score: ", ""))))
        elif parsed_line["type"] == "SPOTLIGH_OBJ":  # Processing the SPOTLIGH_OBJ message: Draws the spotlight object
            lookaway_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                           parsed_line["obj"]["position_y"],
                                           color="yellow", alpha=0.9, zorder=4))
            lookaway_coords.append((parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                    parsed_line["obj"]["position_y"]))
        elif parsed_line["type"] == "STOP_OBJ":  # Processing the STOP_OBJ message: Draws the stop sign object
            stop_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                       parsed_line["obj"]["position_y"],
                                       color="yellow", alpha=0.9, zorder=4))
            stop_coords.append((parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                parsed_line["obj"]["position_y"]))
        elif parsed_line["type"] == "DODGE_OBJ":  # Processing the DODGE_OBJ message: Draws a obstacle object
            dodge_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                        parsed_line["obj"]["position_y"],
                                        color="gray", alpha=0.9, zorder=4))
            dodge_coords.append((parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                 parsed_line["obj"]["position_y"]))
        elif parsed_line["type"] == "LOOKAWAY_SUCC":  # Processing the LOOKAWAY_SUCC message:
            # Erases the spotlight object. Score feedback.
            index = check_closest(lookaway_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                    parsed_line["obj"]["position_y"]))
            lookaway_scat[index].remove()
            lookaway_scat.remove(lookaway_scat[index])
            lookaway_coords.remove(lookaway_coords[index])
            more_text.set_text('+0')
        elif parsed_line["type"] == "LOOKAWAY_FAIL":  # Processing the LOOKAWAY_FAIL message:
            # Erases the spotlight object. Score feedback.
            index = check_closest(lookaway_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                    parsed_line["obj"]["position_y"]))
            lookaway_scat[index].remove()
            lookaway_scat.remove(lookaway_scat[index])
            lookaway_coords.remove(lookaway_coords[index])
            less_text.set_text('-100')
        elif parsed_line["type"] == "STOP_SUCC":  # Processing the STOP_SUCC message:
            # Erases the stop object. Score feedback.
            index = check_closest(stop_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                parsed_line["obj"]["position_y"]))
            stop_scat[index].remove()
            stop_scat.remove(stop_scat[index])
            stop_coords.remove(stop_coords[index])
            more_text.set_text('+0')
        elif parsed_line["type"] == "STOP_FAIL":  # Processing the STOP_FAIL message:
            # Erases the stop object. Score feedback.
            index = check_closest(stop_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                parsed_line["obj"]["position_y"]))
            stop_scat[index].remove()
            stop_scat.remove(stop_scat[index])
            stop_coords.remove(stop_coords[index])
            less_text.set_text('-100')
        elif parsed_line["type"] == "DODGE_FAIL":  # Processing the DODGE_FAIL message:
            # Erases the obstacle object. Score feedback.
            index = check_closest(dodge_coords, (parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                                 parsed_line["obj"]["position_y"]))
            dodge_scat[index].remove()
            dodge_scat.remove(dodge_scat[index])
            dodge_coords.remove(dodge_coords[index])
            less_text.set_text('-100')
        elif parsed_line["type"] == "MOVING_OBJECT":  # Processing the MOVING_OBJECT message:
            if i % 10 == 0:  # Draws the moving object every 10 file lines
                if len(moving_scat) > 0:  # Erases the scatter if already exists
                    moving_scat[0].remove()
                    moving_scat.clear()
                moving_scat.append(a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                                             parsed_line["obj"]["position_y"], color="orange"))
        elif parsed_line["type"] == "OUT_OF_ROAD":  # Processing the OUT_OF_ROAD message: Score feedback.
            less_text.set_text('-50')
        elif parsed_line["type"] == "HINT":  # Processing the HINT message: Draws the arrow if game with static objects
            if moving_or_static[0] == "STATIC":
                a.scatter(parsed_line["obj"]["position_x"], parsed_line["obj"]["position_z"],
                          parsed_line["obj"]["position_y"],
                          color="saddlebrown", alpha=0.9, zorder=4)
        elif parsed_line["type"] == "END":  # Processing the END message: Save the game score and closes the file
            if moving_or_static[0] == "STATIC":
                fScore = open("Scoreboard/score_static.txt", 'a')
                fScore.write(parsed_line["user_name"] + ":" + str(parsed_line["gameState"]["score"]) + '\n')
            else:
                fScore = open("Scoreboard/score_moving.txt", 'a')
                fScore.write(parsed_line["user_name"] + ":" + str(parsed_line["gameState"]["score"]) + '\n')
            fScore.close()
        elif parsed_line["type"] == "STATIC" or parsed_line["type"] == "MOVING":  # Processing the message which
            # indicate the type of game.
            moving_or_static.append(parsed_line["type"])

        if i % 33 == 0:  # Changes the game information every 33 file lines
            info_text.set_text('Score: ' + str(parsed_line["gameState"]["score"]) +
                               ', Num_Checkpoints: ' + str(parsed_line["gameState"]["num_checkpoints"]) + ', Time: ' +
                               str(parsed_line["gameState"]["minutes"]).split('.')[0].zfill(2) + ":" +
                               str(parsed_line["gameState"]["seconds"]).split('.')[0].zfill(2))
            less_text.set_text('')
            more_text.set_text('')

        if i == len(lines) - 2:
            white_line.clear()


def check_closest(gos_list, coordinates):
    """
    Verifies the closest object to the camera.
    :param gos_list: list of game objects
    :param coordinates: camera coordinates
    :return: index of the closest object
    """
    count = 0
    closest_distance = 999999
    closest_count = 0
    for item in gos_list:
        aux_distance = calculate2dDistance(item, coordinates)
        if closest_distance > aux_distance:
            closest_distance = aux_distance
            closest_count = count
        count += 1
    return closest_count


def calculate2dDistance(point1, point2):
    """
    Calculates the 2D distance between two points.
    :param point1: First point
    :param point2: Second point
    :return: 2D distance between the two points
    """
    return math.sqrt(math.pow(point2[0] - point1[0], 2) +
                     math.pow(point2[1] - point1[1], 2) +
                     math.pow(point2[2] - point1[2], 2))


for x in range(0, len(lines)):  # Draw game perforamance
    animation_frame3D(x)

if animator:  # Animate the game performance drawing
    white_line = []
    red_line = []
    camera_scat = []
    moving_scat = []
    goal_scat = []
    goal_coords = []
    lookaway_scat = []
    lookaway_coords = []
    stop_scat = []
    stop_coords = []
    dodge_scat = []
    dodge_coords = []

    fig = plt.figure(1)
    a = fig.add_subplot(111, projection='3d')
    fig.canvas.copy_from_bbox(a.bbox)
    a.set_xlim([-10, 10])
    a.set_ylim([-10, 10])
    a.set_zlim([-10, 10])
    info_text = a.text(x=-10, y=10, z=15, s='Score: 0, Num_Checkpoints: 0, Time: 00:00')
    more_text = a.text(x=-5, y=9, z=15, s='', c="green")
    less_text = a.text(x=-5, y=5, z=15, s='', c="red")
    counter = 0

    anim = animation.FuncAnimation(fig, frames=len(lines) - 1, func=animation_frame3D, interval=0.0001, repeat=False)

    # writergif = animation.PillowWriter(fps=30)
    # anim.save('yo2.gif', writer=writergif)

plt.show()