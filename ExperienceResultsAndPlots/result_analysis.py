import matplotlib.pyplot as plt
import numpy as np

"""
Plot drawer for the drift analysis
"""

markerResultsDistance = [] # Stores the distance errors of each run in each marker

# Saves the distance error from the results obtained from ARCore to the markers in each run
for run in range(0, 18):
    fileName = "Results_iris/results"+str(run)
    markerResultsDistanceAux = []
    for result in range(1, 6):
        f = open(fileName + "_" + str(result) + ".csv", "r")
        lines = f.read().split("\n")
        coords = lines[1].split(",")
        markerResultsDistanceAux.append(float(coords[6]))
    markerResultsDistance.append(markerResultsDistanceAux)

# Numpy array function
markerResultsDistance_nparray = []
for aux in markerResultsDistance:
    markerResultsDistance_nparray.append(np.array(aux))

# Draws plots for each method and walking speed containing the distance error for each marker in each run
titles = ["Drift - Slow and Steady",
          "Drift - Slow and Side",
          "Drift - Slow and Obstruction",
          "Drift - Fast and Steady",
          "Drift - Fast and Side",
          "Drift - Fast and Obstruction"]
for fig in range(0, 6):
    figDriftS = plt.figure(fig+1)
    drift = figDriftS.add_subplot(111)
    drift.plot(markerResultsDistance_nparray[0+fig*3], linestyle='--', marker='o', color='r')
    drift.plot(markerResultsDistance_nparray[1+fig*3], linestyle='--', marker='o', color='g')
    drift.plot(markerResultsDistance_nparray[2+fig*3], linestyle='--', marker='o', color='b')
    drift.set_title(titles[fig])
    drift.set_ylim([-0.95, 0.7]) # (house)
    #drift.set_ylim([-4.05, 5.05]) # (iris)
    drift.set_xlabel("Marker")
    drift.set_ylabel("Distance error")
    drift.legend(["Run nº1", "Run nº2", "Run nº3"],loc='lower right')
    plt.xticks(list(range(len(markerResultsDistance_nparray[0]))), ["2_A", "2_B", "3", "4", "5"])

plt.show()