import math
import matplotlib.pyplot as plt
import pandas as pd
import matplotlib.patches

"""
Handles the configuration usability test results.
"""

conf_phone = open("UsabilityTestConf/testConfPhone.txt", "r")  # File -> Configuration using the smartphone
conf_desktop = open("UsabilityTestConf/testConfDesktop.txt", "r")  # File -> Configuration using the desktop pc

phone_lines = conf_phone.readlines()
desktop_lines = conf_desktop.readlines()

# List item format:
# object, time, precision_x, precision_y, precision_z, precision_straight_line, precision_rotation, precision_scale
sum_object_desktop = []  # desktop
sum_object_phone = [[]]  # phone
all_values_object_desktop = []  # desktop
all_values_object_phone = [[]]  # phone

# Processes the configuration file using the smartphone -> Sum every participant result
for line in phone_lines:
    splitted_line = line.split(",")
    if len(sum_object_phone) == int(splitted_line[1]):
        rot = abs(float(splitted_line[6]))
        if rot >= 45:
            rot -= 180
        rot = abs(rot)

        straigh_line_distance = math.sqrt(
            (math.pow(abs(float(splitted_line[3])), 2) + math.pow(abs(float(splitted_line[5])), 2)))

        sum_object_phone.append([splitted_line[2], abs(float(splitted_line[3])), abs(float(splitted_line[4])),
                                 abs(float(splitted_line[5])), rot, abs(float(splitted_line[7])),
                                 straigh_line_distance])

        all_values_object_phone.append([[splitted_line[2]], [abs(float(splitted_line[3]))], [abs(float(splitted_line[4]))],
                                 [abs(float(splitted_line[5]))], [rot], [abs(float(splitted_line[7]))],
                                 [straigh_line_distance]])
    else:
        hours = float(sum_object_phone[int(splitted_line[1])][0].split(":")[0]) + float(splitted_line[2].split(":")[0])
        minutes = float(sum_object_phone[int(splitted_line[1])][0].split(":")[1]) + float(
            splitted_line[2].split(":")[1])
        if minutes >= 60:
            hours += 1
            minutes -= 60
        seconds = float(sum_object_phone[int(splitted_line[1])][0].split(":")[2]) + float(
            splitted_line[2].split(":")[2])
        if seconds >= 60:
            minutes += 1
            seconds -= 60
        sum_object_phone[int(splitted_line[1])][0] = str(int(hours)).zfill(2) + ":" + str(int(minutes)).zfill(
            2) + ":" + str(seconds)
        sum_object_phone[int(splitted_line[1])][1] += abs(float(splitted_line[3]))
        sum_object_phone[int(splitted_line[1])][2] += abs(float(splitted_line[4]))
        sum_object_phone[int(splitted_line[1])][3] += abs(float(splitted_line[5]))
        rot = abs(float(splitted_line[6]))
        if rot >= 45:
            rot -= 180
        rot = abs(rot)
        sum_object_phone[int(splitted_line[1])][4] += rot
        sum_object_phone[int(splitted_line[1])][5] += abs(float(splitted_line[7]))

        straigh_line_distance = math.sqrt(
            (math.pow(abs(float(splitted_line[3])), 2) + math.pow(abs(float(splitted_line[5])), 2)))
        sum_object_phone[int(splitted_line[1])][6] += straigh_line_distance

        all_values_object_phone[int(splitted_line[1])][0].append(splitted_line[2])
        all_values_object_phone[int(splitted_line[1])][1].append(abs(float(splitted_line[3])))
        all_values_object_phone[int(splitted_line[1])][2].append(abs(float(splitted_line[4])))
        all_values_object_phone[int(splitted_line[1])][3].append(abs(float(splitted_line[5])))
        all_values_object_phone[int(splitted_line[1])][4].append(rot)
        all_values_object_phone[int(splitted_line[1])][5].append(abs(float(splitted_line[7])))
        all_values_object_phone[int(splitted_line[1])][6].append(straigh_line_distance)

# Processes the configuration file using the desktop pc -> Sum every participant result
for line in desktop_lines:
    splitted_line = line.split(",")
    if len(sum_object_desktop) == int(splitted_line[1]):
        rot = abs(float(splitted_line[6]))
        if rot >= 45:
            rot -= 180
        rot = abs(rot)

        straigh_line_distance = math.sqrt(
            (math.pow(abs(float(splitted_line[3])), 2) + math.pow(abs(float(splitted_line[5])), 2)))

        sum_object_desktop.append([splitted_line[2], abs(float(splitted_line[3])), abs(float(splitted_line[4])),
                                 abs(float(splitted_line[5])), rot, abs(float(splitted_line[7])),
                                 straigh_line_distance])

        all_values_object_desktop.append([[splitted_line[2]], [abs(float(splitted_line[3]))], [abs(float(splitted_line[4]))],
                                 [abs(float(splitted_line[5]))], [rot], [abs(float(splitted_line[7]))],
                                 [straigh_line_distance]])
    else:
        hours = float(sum_object_desktop[int(splitted_line[1])][0].split(":")[0]) + float(splitted_line[2].split(":")[0])
        minutes = float(sum_object_desktop[int(splitted_line[1])][0].split(":")[1]) + float(
            splitted_line[2].split(":")[1])
        if minutes >= 60:
            hours += 1
            minutes -= 60
        seconds = float(sum_object_desktop[int(splitted_line[1])][0].split(":")[2]) + float(
            splitted_line[2].split(":")[2])
        if seconds >= 60:
            minutes += 1
            seconds -= 60
        sum_object_desktop[int(splitted_line[1])][0] = str(int(hours)).zfill(2) + ":" + str(int(minutes)).zfill(
            2) + ":" + str(seconds)
        sum_object_desktop[int(splitted_line[1])][1] += abs(float(splitted_line[3]))
        sum_object_desktop[int(splitted_line[1])][2] += abs(float(splitted_line[4]))
        sum_object_desktop[int(splitted_line[1])][3] += abs(float(splitted_line[5]))
        rot = abs(float(splitted_line[6]))
        if rot >= 45:
            rot -= 180
        rot = abs(rot)
        sum_object_desktop[int(splitted_line[1])][4] += rot
        sum_object_desktop[int(splitted_line[1])][5] += abs(float(splitted_line[7]))

        straigh_line_distance = math.sqrt(
            (math.pow(abs(float(splitted_line[3])), 2) + math.pow(abs(float(splitted_line[5])), 2)))
        sum_object_desktop[int(splitted_line[1])][6] += straigh_line_distance

        all_values_object_desktop[int(splitted_line[1])][0].append(splitted_line[2])
        all_values_object_desktop[int(splitted_line[1])][1].append(abs(float(splitted_line[3])))
        all_values_object_desktop[int(splitted_line[1])][2].append(abs(float(splitted_line[4])))
        all_values_object_desktop[int(splitted_line[1])][3].append(abs(float(splitted_line[5])))
        all_values_object_desktop[int(splitted_line[1])][4].append(rot)
        all_values_object_desktop[int(splitted_line[1])][5].append(abs(float(splitted_line[7])))
        all_values_object_desktop[int(splitted_line[1])][6].append(straigh_line_distance)

# Close the files
conf_phone.close()
conf_desktop.close()

# Computes and prints the avarage results for the configuration using the smartphone
object_number = 1
divider = 10
print("Phone results:\n")
for obj in sum_object_phone[1:]:
    print("Object (avg) " + str(object_number) + ":")
    hours = float(obj[0].split(":")[0])
    minutes = float(obj[0].split(":")[1])
    seconds = float(obj[0].split(":")[2])
    seconds = seconds / divider
    seconds += ((minutes / divider) % 1) * 60
    minutes = minutes / divider
    print(
        "Time: " + str(int(hours)).zfill(2) + ":" + str(int(minutes)).zfill(2) + ":" + str(round(seconds, 2)).zfill(2))
    print("X: " + str(round((obj[1] / divider), 3)))
    # print("Y: " + str(round((obj[2] / divider), 3)))
    print("Z: " + str(round((obj[3] / divider), 3)))
    print("Straight line distance: " + str(round((obj[6] / divider), 3)))
    print("Rotation: " + str(round((obj[4] / divider), 3)))
    print("Scale: " + str(round((obj[5] / divider), 3)) + '\n')
    object_number += 1

# Computes and prints the avarage results for the configuration using the desktop pc
object_number = 1
divider = 10
print("Desktop results:\n")
for obj in sum_object_desktop[1:]:
    if object_number > 2:
        divider = 9
    else:
        divider = 10
    print("Object (avg) " + str(object_number) + ":")
    hours = float(obj[0].split(":")[0])
    minutes = float(obj[0].split(":")[1])
    seconds = float(obj[0].split(":")[2])
    seconds = seconds / divider
    seconds += ((minutes / divider) % 1) * 60
    minutes = minutes / divider
    print(
        "Time: " + str(int(hours)).zfill(2) + ":" + str(int(minutes)).zfill(2) + ":" + str(round(seconds, 2)).zfill(2))
    print("X: " + str(round((obj[1] / divider), 3)))
    # print("Y: " + str(round((obj[2] / divider), 3)))
    print("Z: " + str(round((obj[3] / divider), 3)))
    print("Straight line distance: " + str(round((obj[6] / divider), 3)))
    print("Rotation: " + str(round((obj[4] / divider), 3)))
    print("Scale: " + str(round((obj[5] / divider), 3)) + '\n')
    object_number += 1

# Fix the average results using the smartphone for drawing the box plots
time_in_seconds = []
all_values_object_phone.pop(0)
all_values_object_phone_fixed = []
for obj in all_values_object_phone:
    times = obj[0]
    obj_aux = obj
    for time in times:
        minutes = time.split(":")[1]
        seconds = time.split(":")[2]
        time_in_seconds.append(float(minutes) * 60 + float(seconds))
    obj_aux.append(time_in_seconds.copy())
    all_values_object_phone_fixed.append(obj_aux.copy())
    time_in_seconds.clear()

# Fix the average results using the desktop pc for drawing the box plots
time_in_seconds = []
all_values_object_desktop.pop(0)
all_values_object_desktop_fixed = []
for obj in all_values_object_desktop:
    print(obj)
    times = obj[0]
    obj_aux = obj
    for time in times:
        minutes = time.split(":")[1]
        seconds = time.split(":")[2]
        time_in_seconds.append(float(minutes) * 60 + float(seconds))
    obj_aux.append(time_in_seconds.copy())
    all_values_object_desktop_fixed.append(obj_aux.copy())
    time_in_seconds.clear()

# Draw box plots for each result
titles = ["",
          "X error for each object",
          "Y error for each object",
          "Z error for each object",
          "Rotation error for each object",
          "Scale error for each object",
          "Straight line distance error for each object",
          "Time to position each object"]
ylabels = ["",
           "X error (m)",
           "Y error (m)",
           "Z error (m)",
           "Rotation error (º)",
           "Scale error (m)",
           "Distance error (m)",
           "Time (s)"]
for idx in range(1, 8):
    x = [all_values_object_phone_fixed[0][idx],
         all_values_object_desktop_fixed[0][idx],
         all_values_object_phone_fixed[1][idx],
         all_values_object_desktop_fixed[1][idx],
         all_values_object_phone_fixed[2][idx],
         all_values_object_desktop_fixed[2][idx],
         all_values_object_phone_fixed[3][idx],
         all_values_object_desktop_fixed[3][idx]]
    df = pd.DataFrame(x, index=['Obj 1\nphone', 'Obj 1\ndesktop', 'Obj 2\nphone', 'Obj 2\ndesktop',
                                'Obj 3\nphone', 'Obj 3\ndesktop', 'Obj 4\nphone', 'Obj 4\ndesktop'])

    axes = df.T.boxplot(positions=[0.5, 1.5, 3, 4, 5.5, 6.5, 8, 9], grid=False,
                 boxprops=dict(linestyle='-', linewidth=1, color='k'), patch_artist=True,
                 medianprops=dict(linestyle='-', linewidth=1, color='orange'),
                 meanprops=dict(linestyle='--', linewidth=1, color='r'), showmeans=True, meanline=True)
    for i in range(0, 8):
        if i % 2 == 0:
            axes.findobj(matplotlib.patches.Patch)[i].set_edgecolor("black")
            axes.findobj(matplotlib.patches.Patch)[i].set_facecolor("white")
        else:
            axes.findobj(matplotlib.patches.Patch)[i].set_edgecolor("green")
            axes.findobj(matplotlib.patches.Patch)[i].set_facecolor("white")
    plt.plot([], [], '-', linewidth=1, color='black', label='Phone')
    plt.plot([], [], '-', linewidth=1, color='green', label='Desktop')
    plt.plot([], [], '-', linewidth=1, color='orange', label='Median')
    plt.plot([], [], '--', linewidth=1, color='red', label='Mean')
    plt.legend()
    plt.subplots_adjust(bottom=0.25)
    plt.xticks(rotation=25)
    plt.title(titles[idx])
    plt.ylabel(ylabels[idx])
    plt.xlabel("Object number and configuration device")
    plt.show()