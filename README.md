# Thesis - Augmented Reality Serious Game for Intelligent Wheelchair

## About the project

The quality of life of people with reduced mobility is an important matter and every day technology also evolves in order to help them. Intelligent wheelchairs are a result of this evolution, however, adapting to use new technologies is not always easy. Serious games can make the process of acquiring the skills to use a robotic wheelchair a fun and immersive method. Augmented reality is also a support tool in learning and training how to use several new technologies, including robotics. Allowing the visualization and interaction with virtual objects in the environment which the intelligent wheelchair is inserted, augmented reality make the creation of a serious game possible. Consequently, in this dissertation we developed an augmented reality serious game, taking advantage of ARCore’s tracking capabilities, which were studied and evaluated. A tool for configuring the augmented reality environment where the serious game is played has been developed. The game is supported by a visualization tool that, at the end, allows the visualization of the user’s performance and his errors. An experiment to evaluate the configuration tool and the serious game was carried out with the collaboration of a set of participants. In addition to the development of the serious game and the tools necessary to make it possible, we also developed a library which allows transforming Unity games into augmented reality games.

## DesktopAplication:

Application code for the configuration using the desktop PC and for the performance visualization.

### Requirements:

* Unity 2018.3.2f1 or other compatible version.

### Usage:

#### Configuration of the AR scenario using ARCore cloud anchors

1. Open the project in Unity.
2. Import the file with the anchors information to the application folder "*AnchorFiles*".
3. Import the architectural plan (if any) to the application folder "*Plan*".
4. Run the application in Unity Editor.
5. Select the file you previously imported.
6. Place the anchors and perform the configuration of the virtual objects.
7. Save the configuration.
8. Transfer the output file to the Android application.

#### Configuration using ARCore motion tracking

1. Open the project in Unity.
2. Import the architectural plan (if any) to the application folder "*Plan*".
3. Run the application in Unity Editor.
4. Select the file you previously imported.
5. Place the desired player initial position and perform the configuration of the virtual objects.
6. Save the configuration.
7. Transfer the output file to the Android application.

#### Performance visualization

1. Open the project in Unity.
2. Import the user's performance file to application folder "*Monitorization*".
3. Run the application in Unity Editor.
4. Watch the user's performance.

## ExperienceResultsAndPlots:

Draws the path traveled by the smartphone. Evaluation of ARCore motion tracking technology.

### Requirements:

* Python 3.0

### Usage:

1. Change the file to read.
2. python trajectory_drawer.py

## PhoneApplication:

Application code for configuration of the AR scenario using the smartphone and for augmented reality serious game.

### Requirements:

* Unity 2018.3.2f1 or other compatible version.
* Android 7.0 version or higher

### Usage:

#### Configuration of the AR scenario

1. Open the project in Unity.
2. Build and Run the application to the smartphone.
3. Create a new scenario.
4. Place the anchor(s) (if using ARCore cloud anchors technology) and the virtual object(s) around the environment.

#### Configuration of the AR scenario

1. Open the project in Unity.
2. Build and Run the application to the smartphone.
3. Select the configured AR scenario.
4. Play the game with static virtual objects or with the moving object.

## Roll-a-ball:

Roll-a-ball game in augmented reality.

### Requirements:

* Unity 2018.3.2f1 or other compatible version.
* Android 7.0 version or higher

### Usage:

1. Open the project in Unity.
2. Build and Run the application to the smartphone.
3. Play the Roll-a-ball game in an immersive augmented reality environment.

## ServerApplication:

Code for the server application. Waits for the smartphone game application connection. Saves the user game performance.
Code for performance visualization using matplotlib -> deprecated.
Code for the usability tests analysis.

### Requirements:

* Python 3.0

### Usage (server):

1. Change the server IP address and Port.
2. python server.py
3. Insert the server IP address and port in the game application.
4. Transfer the file to the desktop application to watch the user game performance.

## Tanks:

Tanks game in augmented reality.

### Requirements:

* Unity 2018.3.2f1 or other compatible version.
* Android 7.0 version or higher

### Usage:

1. Open the project in Unity.
2. Build and Run the application to the smartphone.
3. Observe the tanks game in an immersive augmented reality environment.

## gameLibraryPackage:

Library package to transform any Unity game in an augmented reality game.

### Requirements:

* Unity 2018.3.2f1 or other compatible version.
* Android 7.0 version or higher

### Usage:

1. Import the package “gameLibraryPackage.unitypackage”. Assets -> Import Package -> Custom Package... and select the corresponding package.
2. Tag the intended GameObject player with the tag “Player”, if any (it is possible to be just spectator).
3. Transfer the game to Android platform and associate the player to ARCore and to the phone camera. For this, just use the menu: ARCore game library -> Create game for ARCore.
4. Open the scaling window, if any adjustment to the scene scale must be made to fit in the real world.
5. Choose the scale comparison image, if any and set its width and height.
6. Increase or decrease the scene scale.
7. Change the “arcoreHandler” GameObject position and rotation to the intended starting pose on the real world, relatively to the game scene.
8. Build and Run to the smartphone and enjoy the game in the AR environment.

## Contact

Rafael Maio - rafamaio_98@hotmail.com

## Acknowledgement

I would like to thank my advisor Nuno Lau, co-advisor Paulo Dias and collaborator João Alves.
