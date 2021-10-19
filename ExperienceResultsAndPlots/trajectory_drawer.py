import matplotlib.pyplot as plt
import numpy as np

"""
Plot drawer based on each run result
"""

def set_axes_equal(ax: plt.Axes):
    """Set 3D plot axes to equal scale.

    Make axes of 3D plot have equal scale so that spheres appear as
    spheres and cubes as cubes.  Required since `ax.axis('equal')`
    and `ax.set_aspect('equal')` don't work on 3D.
    """
    limits = np.array([
        ax.get_xlim3d(),
        ax.get_ylim3d(),
        ax.get_zlim3d(),
    ])
    origin = np.mean(limits, axis=1)
    radius = 0.5 * np.max(np.abs(limits[:, 1] - limits[:, 0]))
    _set_axes_radius(ax, origin, radius)

def _set_axes_radius(ax, origin, radius):
    x, y, z = origin
    ax.set_xlim3d([x - radius, x + radius])
    ax.set_ylim3d([y - radius, y + radius])
    ax.set_zlim3d([z - radius, z + radius])

pointsAux_1st = []  # Poses for the first half of the run
pointsAux_2nd = []  # Poses for the second half of the run
points = []

fileName = "Results_iris/results3"  # File to draw from
f = open(fileName + ".csv", "r")

lastpoint = (99.00, 99.00, 99.00)  # Last camera position
lastrot = (370.00, 370.00, 370.00)  # Last camera rotation

counter = 0  # Number of repeated points
repeatedPoints = {}  # Repeated points

lines = f.read().split("\n")  # File line

# Process every camera coordinate (written on the file)
for line in lines:
    if line == '':
        break
    coords = line.split(",")
    if (not float(coords[0]) + 0.2 > lastpoint[0] > float(coords[0]) - 0.2 or
        not float(coords[1]) + 0.2 > lastpoint[2] > float(coords[1]) - 0.2 or
        not float(coords[2]) + 0.2 > lastpoint[1] > float(coords[2]) - 0.2) and \
            (lastpoint != (99.00, 99.00, 99.00)):
        print("Big difference -> Current coordinates: (" + coords[0] + "," + coords[1] + "," + coords[
            2] + "), last coordinates: (" + str(
            lastpoint[0]) + "," + str(lastpoint[2]) + "," + str(lastpoint[1]) + ") at " + coords[7])
    if float(coords[0]) == lastpoint[0] and float(coords[1]) == lastpoint[2] and float(coords[2]) == lastpoint[1] \
            and float(coords[3]) == lastrot[0] and float(coords[4]) == lastrot[1] and float(coords[5]) == lastrot[2]:
        counter += 1
        if counter > 1:
            # print("Nothing changed ("+str(counter)+") -> Current coordinates: (" + coords[0] + "," + coords[1] + "," + coords[2] + "), last coordinates: (" + str(
            #    lastpoint[0]) + "," + str(lastpoint[2]) + "," + str(lastpoint[1]) + ")")
            repeatedPoints[coords[0], coords[1], coords[2], coords[3], coords[4], coords[5]] = counter
    else:
        counter = 0
    points.append([float(coords[0]), float(coords[2]), float(coords[1])])
    lastpoint = (float(coords[0]), float(coords[2]), float(coords[1]))
    lastrot = (float(coords[3]), float(coords[4]), float(coords[5]))

# Dividir coordenadas pelo meio (iris)
#pointsAux_1st = points[:len(points) // 2]
#pointsAux_2nd = points[len(points) // 2:]

# Dividir coordenadas pelo ponto mais à frente (house)
bigger = (-99, -99)
cont = 0
maxCont = 0
for point in points:
    cont += 1
    if point[0] >= bigger[0] and point[1] >= bigger[1]:
        bigger = (point[0], point[1])
        maxCont = cont
pointsAux_1st = points[:maxCont]
pointsAux_2nd = points[maxCont:]

# Numpy.array function
points_1st = np.array(pointsAux_1st).T
points_2nd = np.array(pointsAux_2nd).T

# X Z coordinates markers House
'''
markerX = [0.3, 0.3, 2.4, 0.0]
markerY = [-0.4, -0.4, -0.4, -0.4]
markerZ = [0.0, 2.3, 6.7, -0.4]
'''

# X Z coordinates markers IRIS

markerX = [0.6, 0.6, -2, 0.0]
markerY = [-0.6, -0.6, -0.6, -0.6]
markerZ = [0.0, 4, 8.6, -0.6]

# Markers coordinates where the results were taken
markerResultsX = []
markerResultsY = []
markerResultsZ = []
for result in range(0, 6):
    f = open(fileName + "_" + str(result) + ".csv", "r")
    lines = f.read().split("\n")
    coords = lines[0].split(",")
    print(coords)
    if not markerResultsX:
        markerResultsX.append(float(coords[0]))
        markerResultsY.append(float(coords[1]))
        markerResultsZ.append(float(coords[2]))
    else:
        markerResultsX.append(float(coords[0]) + markerResultsX[0])
        markerResultsY.append(float(coords[1]) + markerResultsY[0])
        markerResultsZ.append(float(coords[2]) + markerResultsZ[0])

# Colors for the markers
colors = ["blue", "gold", "green", "red"]
colorsResults = ["blue", "gold", "yellow", "green", "red", "lightblue"]

# Number of poses
n1 = range(len(points_1st[0]))
n2 = range(len(points_1st[0]), len(points_1st[0]) + len(points_2nd[0]))

# Draws the X axis in each cycle
figX = plt.figure(1)
aX = figX.add_subplot(111)
aX.plot(n1, points_1st[0], color="black")
aX.plot(n2, points_2nd[0], color="grey")
aX.set_title("X axis in each cycle")
aX.set_xlabel('Cycle')
aX.set_ylabel('X distance (m)')

# Draws the Z axis in each cycle
figZ = plt.figure(2)
aZ = figZ.add_subplot(111)
aZ.plot(n1, points_1st[1], color="black")
aZ.plot(n2, points_2nd[1], color="grey")
aZ.set_title("Z axis in each cycle")
aZ.set_xlabel('Cycle')
aZ.set_ylabel('Z distance (m)')

# Draws the path trajectory and the markers positions (real and obtained from ARCore)
fig3D = plt.figure(3)
a3D = fig3D.add_subplot(111, projection='3d')
black_line, = a3D.plot(points_1st[0], points_1st[1], points_1st[2], color="black")
#brown_line, = a3D.plot(points_2nd[0], points_2nd[1], points_2nd[2], color="saddlebrown") # (house)
a3D.plot(points_2nd[0], points_2nd[1], points_2nd[2], color="black") # (iris)
a3D.set_title("Path Walked (3D)")
a3D.set_xlabel('X distance (m)')
a3D.set_zlabel('Y distance (m)')
a3D.set_ylabel('Z distance (m)')
i = 0
for c in colors:
    a3D.scatter(markerX[i], markerZ[i],markerY[i], color=c, zorder=3, alpha=0.7)
    i += 1
i = 0
for c in colorsResults:
    a3D.scatter(markerResultsX[i],  markerResultsZ[i],markerResultsY[i], color=c, marker="x", zorder=6)
    i += 1

circles = a3D.scatter(markerX[0], markerZ[0], markerY[0], color="darkgrey", alpha=0.7, zorder=0)
crosses = a3D.scatter(markerResultsX[0], markerResultsZ[0], markerResultsY[0], color="darkgrey", alpha=1, zorder=0, marker='x')
#a3D.legend([black_line, brown_line, circles, crosses], ["Way forward", "Way back", "Markers physical\nposition", "Markers position \ngiven by ARCore"],
#           loc='upper left', bbox_to_anchor=(0.85, 1.12)) # (house)
a3D.legend([black_line, circles, crosses], ["Path", "Markers physical\nposition", "Markers position \ngiven by ARCore"], loc='upper left', bbox_to_anchor=(1.05, 1)) # (iris)
#a3D.set_aspect('equal')
#set_axes_equal(a3D)


# Draws the path 3D trajectory and the markers positions (real and obtained from ARCore)
figXZ = plt.figure(4)
aXZ = figXZ.add_subplot(111)
black_line, = aXZ.plot(points_1st[0], points_1st[1], color="black")
#brown_line, = aXZ.plot(points_2nd[0], points_2nd[1], color="saddlebrown") # (house)
aXZ.plot(points_2nd[0], points_2nd[1], color="black") # (iris)
aXZ.set_title("Path walked")
aXZ.set_xlabel('X distance (m)')
aXZ.set_ylabel('Z distance (m)')
i = 0
for c in colors:
    aXZ.scatter(markerX[i], markerZ[i], color=c, zorder=3, alpha=0.7)
    i += 1
i = 0
for c in colorsResults:
    aXZ.scatter(markerResultsX[i], markerResultsZ[i], color=c, marker="x", zorder=6)
    i += 1
circles = aXZ.scatter(x=markerX[0], y=markerZ[0], color="darkgrey", alpha=0.7, zorder=0)
crosses = aXZ.scatter(x=markerResultsX[0], y=markerResultsZ[0], color="darkgrey", alpha=1, zorder=0, marker='x')
plt.axis('scaled')
#aXZ.legend([black_line, brown_line, circles, crosses], ["Way forward", "Way back", "Markers physical\nposition", "Markers position \ngiven by ARCore"], loc='upper left', bbox_to_anchor=(1.05, 1)) # (house)
aXZ.legend([black_line, circles, crosses], ["Path", "Markers physical\nposition", "Markers position \ngiven by ARCore"], loc='upper left', bbox_to_anchor=(1.05, 1)) # (iris)

# Draws the histogram plot for the repeated poses during the run
figHist = plt.figure(5)
hist = figHist.add_subplot(111)
histX = []
for x in range(0, len(list(repeatedPoints.keys()))):
    print("P" + str(x) + ": " + str(list(repeatedPoints.keys())[x]))
    histX.append("P" + str(x))
hist.set_title("Obstruction method analysis")
hist.set_ylabel("Number of repeated frames")
hist.set_xlabel("Repeated poses")
hist.bar(histX, repeatedPoints.values(), color='g')

# Figure deprecated
figBoxPlot = plt.figure(6)
boxPlotData = np.array([-0.03,0,-0.08,-0.05,-0.02,-0.07,0.03,-0.01,-0.03,0.02,0.29,0.03,0.03,0.52,0.03])
ax6 = figBoxPlot.add_subplot(111)
ax6.set_title('Box Plot')
ax6.boxplot(boxPlotData)

plt.show()