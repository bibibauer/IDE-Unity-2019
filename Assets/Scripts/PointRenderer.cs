using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// This script gets values from CSVReader script
// It instantiates points and labels according to values read

public class PointRenderer : MonoBehaviour {

    /* PUBLIC VARIABLES */

    // Name of the input file, no extension
    public bool dataPoints = true;
    public bool highlightPoints = true;
    public bool metadataLabels = true;
    public bool dimensionLabels = true;

    // Name of CSV file to be referenced
    [Space (20)]
    public string inputFile = "DVX_ENKLU_190907";

    // Columns to be referenced for different variables in CSV file
    [Space (20)]
    public int dimensionX = 0;
    public int dimensionY = 1;
    public int dimensionZ = 2;
    public int clusterVariable = 3;
    public int interviewVariable = 4;
    public int metadataVariable1 = 5;
    public int metadataVariable2 = 6;
    public int metadataVariable3 = 8;
    public int metadataVariable4 = 9;
    public int metadataVariable5 = 10;
    public int metadataVariable6 = 11;
    public int metadataVariable7 = 12;

    // Prefix labels for the metadata variables
    [Space (20)]
    public string meta1Label;
    public string meta2Label;
    public string meta3Label;
    public string meta4Label;
    public string meta5Label;
    public string meta6Label;
    public string meta7Label;

    // Materials to be used for each type of data point (cluster group or highlight)
    [Space (20)]
    public Material cluster1Material;
    public Material cluster2Material;
    public Material cluster3Material;
    public Material cluster4Material;
    public Material highlightMaterial;

    // Prefabs and scale for the normal data points
    [Space (20)]
    public GameObject pointPrefab;
    [Range (0.0f, 0.5f)]
    public float dataPointScale = 0.07f;

    // Prefabs and scale for the normal data points
    [Space (20)]
    public GameObject highlightPrefab;
    [Range (0.0f, 0.5f)]
    public float highlightPointScale = 0.2f;

    /* PRIVATE VARIABLES */

    // Full column names from CSV (as Dictionary Keys)
    private string xDimensionName;
    private string yDimensionName;
    private string zDimensionName;
    private string clusterVariableName;
    private string interviewVariableName;
    private string metadata1;
    private string metadata2;
    private string metadata3;
    private string metadata4;
    private string metadata5;
    private string metadata6;
    private string metadata7;

    // Scale of particlePoints within graph, WARNING: Does not scale with graph frame
    private float plotScale = 5; // <--- Honestly pretty pointless, need to figure out a way to get rid of this if possible

    // Minimum and maximum values of columns
    private float xMin = -3;
    private float yMin = -3;
    private float zMin = -3;

    private float xMax = 3;
    private float yMax = 3;
    private float zMax = 3;

    // Object which will become a data point
    private GameObject dataPoint;

    // Object which will contain instantiated prefabs in hiearchy
    private GameObject PointHolder;

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    // Number of rows in the CSV file
    private int rowCount;

    //**************************************************************************************** METHODS ************************************************************************************************

    void Awake () {
        // Run CSV Reader
        pointList = CSVReader.Read (inputFile);
        rowCount = pointList.Count;

        // Load prefabs
        pointPrefab = Resources.Load ("DataCube", typeof (GameObject)) as GameObject;
        highlightPrefab = Resources.Load ("DataBall", typeof (GameObject)) as GameObject;
        PointHolder = GameObject.Find ("PointContainer");

        // Load materials
        cluster1Material = Resources.Load ("material1", typeof (Material)) as Material;
        cluster2Material = Resources.Load ("material2", typeof (Material)) as Material;
        cluster3Material = Resources.Load ("material3", typeof (Material)) as Material;
        cluster4Material = Resources.Load ("material4", typeof (Material)) as Material;
        highlightMaterial = Resources.Load ("DataMaterial", typeof (Material)) as Material;
    }

    // Use this for initialization
    void Start () {

        // Store dictionary keys (column names in CSV) in a list
        List<string> columnList = new List<string> (pointList[1].Keys);

        foreach (string key in columnList)

            // Assign column names according to index indicated in columnList
            xDimensionName = columnList[dimensionX];
        yDimensionName = columnList[dimensionY];
        zDimensionName = columnList[dimensionZ];
        clusterVariableName = columnList[clusterVariable];
        interviewVariableName = columnList[interviewVariable];
        metadata1 = columnList[metadataVariable1];
        metadata2 = columnList[metadataVariable2];
        metadata3 = columnList[metadataVariable3];
        metadata4 = columnList[metadataVariable4];
        metadata5 = columnList[metadataVariable5];
        metadata6 = columnList[metadataVariable6];
        metadata7 = columnList[metadataVariable7];

        // Get maxes of each axis, using FindMaxValue method defined below
        xMax = FindMaxValue (xDimensionName);
        yMax = FindMaxValue (yDimensionName);
        zMax = FindMaxValue (zDimensionName);

        // Get minimums of each axis, using FindMinValue method defined below
        xMin = FindMinValue (xDimensionName);
        yMin = FindMinValue (yDimensionName);
        zMin = FindMinValue (zDimensionName);

        // Place data points
        if (dataPoints == true) {
            PlacePrefabPoints ();
        }

        // Place highlights on certain data points
        if (highlightPoints == true) {
            HighlightPoints ();
        }

        // Place labels on graph axes
        if (dimensionLabels == true) {
            AssignLabels ();
        }

    }

    // Places the prefabs according to values read in
    private void PlacePrefabPoints () {

        // Loop through every row in the CSV file
        for (var i = 0; i < rowCount; i++) {

            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle (pointList[i][xDimensionName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle (pointList[i][yDimensionName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle (pointList[i][zDimensionName]) - zMin) / (zMax - zMin);

            // Get cluster group number
            int clusterGroup = (Convert.ToInt32 (pointList[i][clusterVariableName]));

            // Create vector 3 for positioning particlePoints
            Vector3 position = new Vector3 (x, y, z) * plotScale;

            // Instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate (pointPrefab, Vector3.zero, Quaternion.identity);

            // Make child of PointHolder object, to keep particlePoints within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Position and scale point at relative to parent
            dataPoint.transform.localPosition = position;
            dataPoint.transform.localScale = new Vector3 (dataPointScale, dataPointScale, dataPointScale);

            // Converts index number to string and assigns it as the name of the prefab
            string dataPointName = i.ToString ();
            dataPoint.transform.name = dataPointName;

            // Set color according to cluster value
            if (clusterGroup == 1) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster1Material;
            } else if (clusterGroup == 2) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster2Material;
            } else if (clusterGroup == 3) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster3Material;
            } else if (clusterGroup == 4) {
                dataPoint.GetComponent<Renderer> ().sharedMaterial = cluster4Material;
            }

            // Pull metadata values from the CSV file for each data point
            string meta1 = (pointList[i][metadata1]).ToString ();
            string meta2 = (pointList[i][metadata2]).ToString ();
            string meta3 = (pointList[i][metadata3]).ToString ();
            string meta4 = (pointList[i][metadata4]).ToString ();
            string meta5 = (pointList[i][metadata5]).ToString ();
            string meta6 = (pointList[i][metadata6]).ToString ();
            string meta7 = (pointList[i][metadata7]).ToString ();

            if (metadataLabels == true) {
                // Set text values in the Text Mesh component of the child object for each data point
                dataPoint.GetComponentInChildren<TextMesh> ().text =
                    meta1Label + meta1 + "\n" + meta2Label + meta2 + "\n" + meta3Label + meta3 + "\n" + meta4Label + meta4 + "\n" + meta5Label + meta5 + "\n" + meta6Label + meta6 + "\n" + meta7Label + meta7;
            } else if (metadataLabels == false) {
                dataPoint.GetComponentInChildren<TextMesh> ().text = "";
            }
        }

    }

    private void HighlightPoints () {

        // Loop through every row in the CSV file
        for (var i = 0; i < rowCount; i++) {

            // Set x/y/z, standardized to between 0-1
            float x = (Convert.ToSingle (pointList[i][xDimensionName]) - xMin) / (xMax - xMin);
            float y = (Convert.ToSingle (pointList[i][yDimensionName]) - yMin) / (yMax - yMin);
            float z = (Convert.ToSingle (pointList[i][zDimensionName]) - zMin) / (zMax - zMin);

            // Get cluster group number
            int clusterGroup = (Convert.ToInt32 (pointList[i][clusterVariableName]));

            // Get value for interview participants 1 = interview, null = no interview
            int interview = (Convert.ToInt32 (pointList[i][interviewVariableName]));

            // Adds highlight to data point only if there is an interview value
            if (interview == 1) {

                // Place prefabas in the right spots blah blah blah (same as everything in the PlacePointPrefabs method)
                Vector3 position = new Vector3 (x, y, z) * plotScale;
                GameObject highlightPoint = Instantiate (highlightPrefab, Vector3.zero, Quaternion.identity);
                highlightPoint.transform.parent = PointHolder.transform;
                highlightPoint.transform.localPosition = position;
                highlightPoint.transform.localScale = new Vector3 (highlightPointScale, highlightPointScale, highlightPointScale);
                string highlightPointName = i.ToString ();
                highlightPoint.transform.name = highlightPointName + " highlight";

                // Set material to be highlight material
                // highlightPoint.GetComponent<Renderer> ().sharedMaterial = highlightMaterial;

                // Set color according to cluster value
                if (clusterGroup == 1) {
                    highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster1Material;
                } else if (clusterGroup == 2) {
                    highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster2Material;
                } else if (clusterGroup == 3) {
                    highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster3Material;
                } else if (clusterGroup == 4) {
                    highlightPoint.GetComponent<Renderer> ().sharedMaterial = cluster4Material;
                }

                string meta1 = (pointList[i][metadata1]).ToString ();
                string meta2 = (pointList[i][metadata2]).ToString ();

                highlightPoint.GetComponentInChildren<TextMesh> ().text = meta1;

                highlightPoint.gameObject.tag = "Label";

            }

        }

    }

    public void HidePoint (GameObject child) {
        if (child.tag != "Label") {
            child.SetActive (false);
        }
    }

    // Finds labels named in scene, assigns values to their text meshes
    // WARNING: game objects need to be named within scene
    private void AssignLabels () {
        // Update point counter
        //GameObject.Find ("Point_Count").GetComponent<TextMesh> ().text = pointList.Count.ToString ("0");

        // Update title according to inputFile name
        //GameObject.Find ("Dataset_Label").GetComponent<TextMesh> ().text = inputFile;

        // Update axis titles to ColumnNames
        GameObject.Find ("X_Title").GetComponent<TextMesh> ().text = xDimensionName;
        GameObject.Find ("Y_Title").GetComponent<TextMesh> ().text = yDimensionName;
        GameObject.Find ("Z_Title").GetComponent<TextMesh> ().text = zDimensionName;

        // Set x Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("X_Min_Lab").GetComponent<TextMesh> ().text = "Low";
        GameObject.Find ("X_Max_Lab").GetComponent<TextMesh> ().text = "High";
        // GameObject.Find ("X_Min_Lab").GetComponent<TextMesh> ().text = xMin.ToString ("0.0");
        // GameObject.Find ("X_Mid_Lab").GetComponent<TextMesh> ().text = (xMin + (xMax - xMin) / 2f).ToString ("0.0");
        // GameObject.Find ("X_Max_Lab").GetComponent<TextMesh> ().text = xMax.ToString ("0.0");

        // Set y Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("Y_Min_Lab").GetComponent<TextMesh> ().text = "Low";
        GameObject.Find ("Y_Max_Lab").GetComponent<TextMesh> ().text = "High";
        // GameObject.Find ("Y_Min_Lab").GetComponent<TextMesh> ().text = yMin.ToString ("0.0");
        // GameObject.Find ("Y_Mid_Lab").GetComponent<TextMesh> ().text = (yMin + (yMax - yMin) / 2f).ToString ("0.0");
        // GameObject.Find ("Y_Max_Lab").GetComponent<TextMesh> ().text = yMax.ToString ("0.0");

        // Set z Labels by finding game objects and setting TextMesh and assigning value (need to convert to string)
        GameObject.Find ("Z_Min_Lab").GetComponent<TextMesh> ().text = "Low";
        GameObject.Find ("Z_Max_Lab").GetComponent<TextMesh> ().text = "High";
        // GameObject.Find ("Z_Min_Lab").GetComponent<TextMesh> ().text = zMin.ToString ("0.0");
        // GameObject.Find ("Z_Mid_Lab").GetComponent<TextMesh> ().text = (zMin + (zMax - zMin) / 2f).ToString ("0.0");
        // GameObject.Find ("Z_Max_Lab").GetComponent<TextMesh> ().text = zMax.ToString ("0.0");

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