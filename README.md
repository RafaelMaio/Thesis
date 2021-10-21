# Thesis - Augmented Reality Serious Game for Intelligent Wheelchair

## About the project

The quality of life of people with reduced mobility is an important matter and every day technology also evolves in order to help them. Intelligent wheelchairs are a result of this evolution, however, adapting to use new technologies is not always easy. Serious games can make the process of acquiring the skills to use a robotic wheelchair a fun and immersive method. Augmented reality is also a support tool in learning and training how to use several new technologies, including robotics. Allowing the visualization and interaction with virtual objects in the environment which the intelligent wheelchair is inserted, augmented reality make the creation of a serious game possible. Consequently, in this dissertation we developed an augmented reality serious game, taking advantage of ARCore’s tracking capabilities, which were studied and evaluated. A tool for configuring the augmented reality environment where the serious game is played has been developed. The game is supported by a visualization tool that, at the end, allows the visualization of the user’s performance and his errors. An experiment to evaluate the configuration tool and the serious game was carried out with the collaboration of a set of participants. In addition to the development of the serious game and the tools necessary to make it possible, we also developed a library which allows transforming Unity games into augmented reality games.

## DesktopAplication:

Application code for the configuration using the desktop PC and for the performance visualization.

### Requirements:

* Unity 2018.3.2f1

### Usage:

#### Configuration using ARCore cloud anchors

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

...

### Requirements:

* Unity 2018.3.2f1
* Android 7.0 version or higher

### Usage:

...

## Contact

Rafael Maio - rafamaio_98@hotmail.com

## Acknowledgement

I would like to thank my advisor Nuno Lau, co-advisor Paulo Dias and collaborator João Alves.
