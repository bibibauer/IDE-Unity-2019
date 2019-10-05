# IDE-Unity-2019
A Unity project that creates a 3D scatterplot generated from CSV data file. 

This is a Unity project originally based off an example project from [Penn State](https://sites.psu.edu/bdssblog/2017/04/06/basic-data-visualization-in-unity-scatterplot-creation/). 
From there it was set up for the purposes of an academic project called "Data-U" created by a group of students and faculty 
from the Interaction Design program at [California College of the Arts](cca.edu).

The final output of the project was a multi-player mixed reality experience, hosted through [Enklu](enklu.com) 
and ultimately expereienced via Hololens 1 devices.

## What It Does
The system takes a CSV file, reads the file, and then plots it across three dimensions/axees (X, Y, Z) using the 
columns/index numbers for data variables defined by the user. All dimensions values are mapped to a scaled range of -3 to +3. 
The system has also been set up to support the use up to 9 additional (but optional) variables. The first looks for assigned cluster 
group numbers, the second looks for points to be highlighted, and the remaining 7 can metedata values displayed as text.

## How It Works
The two main functions of the system are achieved through the "CSVReader" and "PointRenderer" C# scripts.
Any CSV data file located within the /Resources folder of the project can be referenced by these scripts and plotted.
There are multiple variables exposed in the Pointrenderer script component for making changes without editing the script.
Editable variables are as follows:
- Toggles for displaying data, highlighted data, metadata text, and graph labels
- Name of CSV file to be plotted
- Data columns in CSV to be used for all 12  variables (indexed by column number)
- Prefix labels for metadata text display
- Prefabs to be used for different types of data points (regular vs highlighted)
- Scale of prefabs to be displayed
- Materials to be use for different cluster groups and highlighted data points
