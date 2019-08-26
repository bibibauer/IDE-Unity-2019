using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// This script gets values from CSVReader script
// It instantiates points and particles according to values read

public class PointRenderer : MonoBehaviour {

    //************************************************************************ PUBLIC VARIABLES ************************************************************************

    // Bools for editor options
    public bool renderstaticPointPrefabs = true;
    public bool renderPrefabsWithColor = true;

    public string age = "32";
    public string gender;
    public string location;
    public string political;
    public string worldview;

    // Name of the input file, no extension
    [Space (20)]
    public string inputFile = "Data-U_Dummy";

    // Indices for columns to be assigned
    [Space (20)]
    public int dimensionX = 2;
    public int dimensionY = 3;
    public int dimensionZ = 4;
    public int clusterVariable = 1;
    public int interviewVariable = 5;

    // Full column names from CSV (as Dictionary Keys)
    [Space (20)]
    public string xDimensionName;
    public string yDimensionName;
    public string zDimensionName;
    public string clusterVariableName;
    public string interviewVariableName;

    // Colors to be assigned to each cluster grouping in the data
    [Space (20)]
    public Material cluster1Color;
    public Material cluster2Color;
    public Material cluster3Color;
    public Material cluster4Color;
    public Material highlightColor;

    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
    private float plotScale = 5;

    // Scale of the prefab particlePoints
    [Space (20)]
    [Range (0.0f, 0.5f)]
    public float pointScale = 0.07f;
    [Range (0.0f, 0.5f)]
    public float highlightScale = 0.2f;

    // The prefab for the data particlePoints that will be instantiated
    public GameObject pointPrefab;
    public GameObject highlightPrefab;
    // public GameObject staticPointPrefab;
    // public GameObject interactivePointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    public GameObject metadata;

    //******************************************************************************** PRIVATE VARIABLES ********************************************************************************
    // Minimum and maximum values of columns
    private float xMin;
    private float yMin;
    private float zMin;

    private float xMax;
    private float yMax;
    private float zMax;

    private GameObject dataPoint;

    // Number of rows
    private int rowCount;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    //**************************************************************************************** METHODS ************************************************************************************************

    void Awake () {
        // Run CSV Reader
        pointList = CSVReader.Read (inputFile);

        // Load prefabs
        pointPrefab = Resources.Load ("DataCube", typeof (GameObject)) as GameObject;
        highlightPrefab = Resources.Load ("DataBall", typeof (GameObject)) as GameObject;

        // Load materials
        cluster1Color = Resources.Load ("material1", typeof (Material)) as Material;
        cluster2Color = Resources.Load ("material2", typeof (Material)) as Material;
        cluster3Color = Resources.Load ("material3", typeof (Material)) as Material;
        cluster4Color = Resources.Load ("material4", typeof (Material)) as Material;
        highlightColor = Resources.Load ("DataMaterial", typeof (Material)) as Material;
    }

    // Use this for initialization
    void Start () {

        // Store dictionary keys (column names in CSV) in a list
        List<string> columnList = new List<string> (pointList[1].Keys);

        foreach (string key in columnList)
            //Debug.Log ("Column name is " + key);

            // Assign column names according to index indicated in columnList
            xDimensionName = columnList[dimensionX];
        yDimensionName = columnList[dimensionY];
        zDimensionName = columnList[dimensionZ];
        clusterVariableName = columnList[clusterVariable];
        interviewVariableName = columnList[interviewVariable];

        // Get maxes of each axis, using FindMaxValue method defined below
        xMax = FindMaxValue (xDimensionName);
        yMax = FindMaxValue (yDimensionName);
        zMax = FindMaxValue (zDimensionName);

        // Get minimums of each axis, using FindMinValue method defined below
        xMin = FindMinValue (xDimensionName);
        yMin = FindMinValue (yDimensionName);
        zMin = FindMinValue (zDimensionName);

        // Place labels on graph axes
        AssignLabels ();

        // Call PlacePoint methods defined below
        PlacePrefabPoints ();

        // Call HighlightPoints method defined below
        HighlightPoints ();

    }

    // Places the prefabs according to values read in
    private void PlacePrefabPoints () {

        // Get count (number of rows in table)
        rowCount = pointList.Count;

        // int clusterGroup;

        for (var i = 0; i < pointList.Count; i++) {

            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle (pointList[i][xDimensionName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle (pointList[i][yDimensionName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle (pointList[i][zDimensionName]) - zMin) / (zMax - zMin);

            // Get cluster group number
            int clusterGroup = (Convert.ToInt32 (pointList[i][clusterVariableName]));

            // Create vector 3 for positioning particlePoints
            Vector3 position = new Vector3 (x, y, z) * plotScale;

            //instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate (pointPrefab, Vector3.zero, Quaternion.identity);

            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position point at relative to parent
            dataPoint.transform.localPosition = position;
            dataPoint.transform.localScale = new Vector3 (pointScale, pointScale, pointScale);

            // Converts index to string to name the point the index number
            string dataPointName = i.ToString ();

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            // Set color according to cluster value
            if (clusterGroup == 1) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster1Color;
            } else if (clusterGroup == 2) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster2Color;
            } else if (clusterGroup == 3) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster3Color;
            } else if (clusterGroup == 4) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster4Color;
            }

        }

        metadata = GameObject.Find ("Text");
        metadata.transform.rotate(0, 270, 0, relativeTo = Space.Self);
        Debug.Log ("Text object found");
        metadata.GetComponent<TextMesh> ().text = "Age: 32 \n Location: CA";

    }

    private void HighlightPoints () {

        // Get count (number of rows in table)
        rowCount = pointList.Count;

        // int clusterGroup;

        for (var i = 0; i < pointList.Count; i++) {

            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle (pointList[i][xDimensionName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle (pointList[i][yDimensionName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle (pointList[i][zDimensionName]) - zMin) / (zMax - zMin);

            // Get cluster group number
            int clusterGroup = (Convert.ToInt32 (pointList[i][clusterVariableName]));

            // Get value for interview participants 1 = interview, null = no interview
            int interview = (Convert.ToInt32 (pointList[i][interviewVariableName]));

            // Make prefab white if the data point is an interview participant
            if (interview == 1) {
                // Place prefabas in the right spots blah blah blah same as in the PlacePointPrefabs method
                Vector3 position = new Vector3 (x, y, z) * plotScale;
                GameObject highlightPoint = Instantiate (highlightPrefab, Vector3.zero, Quaternion.identity);

                highlightPoint.transform.parent = PointHolder.transform;
                highlightPoint.transform.localPosition = position;
                highlightPoint.transform.localScale = new Vector3 (highlightScale, highlightScale, highlightScale);
                string highlightPointName = i.ToString ();
                highlightPoint.transform.name = highlightPointName;

                highlightPoint.GetComponent<Renderer> ().sharedMaterial = highlightColor;

                // // Set color according to cluster value
                // if (clusterGroup == 1) {
                //     highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster1Color;
                // } else if (clusterGroup == 2) {
                //     highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster2Color;
                // } else if (clusterGroup == 3) {
                //     highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster3Color;
                // } else if (clusterGroup == 4) {
                //     highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster4Color;
                // }
            }

        }
    }

    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels () {
        // Update point counter
        GameObject.Find ("Point_Count").GetComponent<TextMesh> ().text = pointList.Count.ToString ("0");

        // Update title according to inputFile name
        GameObject.Find ("Dataset_Label").GetComponent<TextMesh> ().text = inputFile;

        // Update axis titles to ColumnNames
        GameObject.Find ("X_Title").GetComponent<TextMesh> ().text = xDimensionName;
        GameObject.Find ("Y_Title").GetComponent<TextMesh> ().text = yDimensionName;
        GameObject.Find ("Z_Title").GetComponent<TextMesh> ().text = zDimensionName;

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("X_Min_Lab").GetComponent<TextMesh> ().text = xMin.ToString ("0.0");
        GameObject.Find ("X_Mid_Lab").GetComponent<TextMesh> ().text = (xMin + (xMax - xMin) / 2f).ToString ("0.0");
        GameObject.Find ("X_Max_Lab").GetComponent<TextMesh> ().text = xMax.ToString ("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("Y_Min_Lab").GetComponent<TextMesh> ().text = yMin.ToString ("0.0");
        GameObject.Find ("Y_Mid_Lab").GetComponent<TextMesh> ().text = (yMin + (yMax - yMin) / 2f).ToString ("0.0");
        GameObject.Find ("Y_Max_Lab").GetComponent<TextMesh> ().text = yMax.ToString ("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("Z_Min_Lab").GetComponent<TextMesh> ().text = zMin.ToString ("0.0");
        GameObject.Find ("Z_Mid_Lab").GetComponent<TextMesh> ().text = (zMin + (zMax - zMin) / 2f).ToString ("0.0");
        GameObject.Find ("Z_Max_Lab").GetComponent<TextMesh> ().text = zMax.ToString ("0.0");

    }

    //Method for finding max value, assumes PointList is generated
    private float FindMaxValue (string columnName) {
        //set initial value to first value
        float maxValue = Convert.ToSingle (pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++) {
            if (maxValue < Convert.ToSingle (pointList[i][columnName]))
                maxValue = Convert.ToSingle (pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    //Method for finding minimum value, assumes PointList is generated
    private float FindMinValue (string columnName) {
        //set initial value to first value
        float minValue = Convert.ToSingle (pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++) {
            if (Convert.ToSingle (pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle (pointList[i][columnName]);
        }

        return minValue;
    }

}