import json

"""
Handles the game usability test results.
"""

# Game with static objects variables
static_collisions = 0  # Number of collisions
static_time = "00:00"  # Time to finish the game
static_distance = 0  # Distance to the white line
static_stop = 0  # Number of stops succeeded
static_not_stop = 0  # Number of stops failed
static_lookaway = 0  # Number of spotlight deflections succeeded
static_not_lookaway = 0  # Number of spotlight deflections failed
static_out_of_road = 0  # Number of times leaving the road

# Game with the moving object variables
moving_collisions = 0  # Number of collisions
moving_time = "00:00"  # Time to finish the game
moving_distance = 0  # Distance to the moving object
moving_distance04 = 0  # Distance to the moving object minus 40cm
moving_stop = 0  # Number of stops succeeded
moving_not_stop = 0  # Number of stops failed
moving_lookaway = 0  # Number of spotlight deflections succeeded
moving_not_lookaway = 0  # Number of spotlight deflections failed
moving_out_of_road = 0  # Number of times leaving the road

#participants = 10
participants = 20  # Number of participants
runs = 2  # Number of games each partipant played

static = True  # Game with static objects or game with the moving object
end = False  # Game finished
scoreToSub = 0  # Score to subtract

# Sums each partipant performance result
for participant in range(1, participants + 1):
# for participant in range(1, participants + 1):
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
                    static = False
                elif parsed_line["type"] == "STATIC":
                    static = True
                elif parsed_line["type"] == "LOOKAWAY_SUCC":
                    if static:
                        static_lookaway += 1
                    else:
                        moving_lookaway += 1
                elif parsed_line["type"] == "LOOKAWAY_FAIL":
                    if static:
                        static_not_lookaway += 1
                    else:
                        moving_not_lookaway += 1
                    scoreToSub += 100
                elif parsed_line["type"] == "STOP_SUCC":
                    if static:
                        static_stop += 1
                    else:
                        moving_stop += 1
                elif parsed_line["type"] == "STOP_FAIL":
                    if static:
                        static_not_stop += 1
                    else:
                        moving_not_stop += 1
                    scoreToSub += 100
                elif parsed_line["type"] == "DODGE_FAIL":
                    if static:
                        static_collisions += 1
                    else:
                        moving_collisions += 1
                    scoreToSub += 100
                elif parsed_line["type"] == "OUT_OF_ROAD":
                    if static:
                        static_out_of_road += 1
                    else:
                        moving_out_of_road += 1
                    scoreToSub += 50
                elif parsed_line["type"] == "END":
                    scoreToSub += int(parsed_line["gameState"]["minutes"]) * 60
                    scoreToSub += int(parsed_line["gameState"]["seconds"])
                    if static:
                        minutes = int(static_time.split(":")[0])
                        minutes += int(parsed_line["gameState"]["minutes"])
                        seconds = int(static_time.split(":")[1])
                        seconds += int(parsed_line["gameState"]["seconds"])
                        if seconds > 60:
                            minutes += 1
                            seconds -= 60
                        static_time = str(minutes).zfill(2) + ":" + \
                                      str(seconds).zfill(2)
                        static_distance += abs((1 - (int(parsed_line["gameState"]["score"]) + scoreToSub) / 1500))
                    else:
                        minutes = int(moving_time.split(":")[0])
                        minutes += int(parsed_line["gameState"]["minutes"])
                        seconds = int(moving_time.split(":")[1])
                        seconds += int(parsed_line["gameState"]["seconds"])
                        if seconds > 60:
                            minutes += 1
                            seconds -= 60
                        moving_time = str(minutes).zfill(2) + ":" + \
                                      str(seconds).zfill(2)
                        moving_distance += abs((1.4 - (int(parsed_line["gameState"]["score"]) + scoreToSub) / 1500))
                        moving_distance04 += abs((1 - (int(parsed_line["gameState"]["score"]) + scoreToSub) / 1500))
                    scoreToSub = 0
                    end = True
                    break
            if end:
                end = False
                break

# Computes and prints the average results for the game with static objects
print("Static objects average:")
minutes = float(static_time.split(":")[0])
seconds = float(static_time.split(":")[1])
seconds = seconds / participants
seconds += ((minutes / participants) % 1) * 60
minutes = minutes / participants
print("Time: " + str(int(minutes)).zfill(2) + ":" + str(seconds).zfill(2))
print("Collisions: " + str((static_collisions / participants) * 100) + "%")
print("Stop success: " + str(int((static_stop / participants) * 100)) + "%, Stop fail: " + str((static_not_stop / participants) * 100) + "%")
print("Spotlight deflect success: " + str((static_lookaway / participants) * 100) + "%, Spotlight deflect fail: " + str((static_not_lookaway / participants) * 100) + "%")
print("Out of the road: " + str(static_out_of_road / participants))
print("Avg distance to the line: " + str(round(static_distance / participants, 3)) + "(m)\n")

# Computes and prints the average results for the game with the moving object
print("Moving object average:")
minutes = float(moving_time.split(":")[0])
seconds = float(moving_time.split(":")[1])
seconds = seconds / participants
seconds += ((minutes / participants) % 1) * 60
minutes = minutes / participants
print("Time: " + str(int(minutes)).zfill(2) + ":" + str(seconds).zfill(2))
print("Collisions: " + str((moving_collisions / participants) * 100) + "%")
print("Stop success: " + str(int((moving_stop / participants) * 100)) + "%, Stop fail: " + str((moving_not_stop / participants) * 100) + "%")
print("Spotlight deflect success: " + str((moving_lookaway / participants) * 100) + "%, Spotlight deflect fail: " + str((static_not_lookaway / participants) * 100) + "%")
print("Out of the road: " + str(moving_out_of_road / participants))
print("Real avg distance to the car: " + str(round(moving_distance / participants, 3)) + "(m)")
print("Game avg distance to the car: " + str(round(moving_distance04 / participants, 3)) + "(m)")