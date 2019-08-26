using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class DataPlotter : MonoBehaviour
{
    //name of the inputfile without an extension
    public string InputFile;

    //the scale of the graph overall. Only used when the graph is generated.
    public float plotScale = 10;

    //List for holding data from the CSV
    private List<Dictionary<string, object>> pointList;

    //Which columns in the CSV get assigned to which axes
    //change the numbers to assign a different CSV column to a different axis.
    //by default, we're assigning the first 3 columns in the CSV to the xyz axes
    public int columnX = 0;
    public int columnY = 1;
    public int columnZ = 2;

    //Full Column names, so that you can see what's going on when the app is running
    public string xName;
    public string yName;
    public string zName;

    //the prefab to use to display a data point
    public GameObject PointPrefab;

    //the game object under which to spawn all the data points
    public GameObject PointHolder;


    // Start is called before the first frame update
    void Start()
    {

        //Use the Reader function to read InputFile and output the data into PointList
        pointList = CSVReader.Read(InputFile);

        //make sure it worked by printing the result to console
        Debug.Log(pointList);

        //Declare list of strings, fill with keys (column names)
        List<string> columnList = new List<string>(pointList[1].Keys);

        Debug.Log("There are " + columnList.Count + " columns in CSV");

        foreach (string key in columnList)
        {
            Debug.Log("Column name is " + key);
        }

        //assign the column names using the column variables
        xName = columnList[columnX];
        yName = columnList[columnY];
        zName = columnList[columnZ];

        //get the mins and maxes of each axis
        float xMax = FindMaxValue(xName);
        float yMax = FindMaxValue(yName);
        float zMax = FindMaxValue(zName);

        float xMin = FindMinValue(xName);
        float yMin = FindMinValue(yName);
        float zMin = FindMinValue(zName);




        //instantiate data points (Prefab, location, rotation)
        //Loop through pointlist
        for (var i = 0; i < pointList.Count; i++)
        {
            //Get value in pointList at row i in named column
            //We're using System.Convert to make sure the program doesn't horribly break if there is NOT a number in the csv cell we're looking at.
            //we're also normalizing the data using the min/max info so that we can control the scale of the graph using the PlotScale variable
            float x = (System.Convert.ToSingle(pointList[i][xName]) - xMin) / (xMax - xMin);
            float y = (System.Convert.ToSingle(pointList[i][yName]) - yMin) / (yMax - yMin);
            float z = (System.Convert.ToSingle(pointList[i][zName]) - zMin) / (zMax - zMin);

            //Instantiate the prefab with the coordinates defined above
            //TODO: instantiate a full object that is aware of its data
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            //make the new data point a child of the point holder game object that's in the scene.
            dataPoint.transform.parent = PointHolder.transform;

            //give the data point a better name
            string dataPointName = pointList[i][xName] + " | " + pointList[i][yName] + " | " + pointList[i][zName];
            dataPoint.transform.name = dataPointName;

            //give the data point a color based on its position in the graph
            dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);
        }


      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //find the max value in a column
    private float FindMaxValue(string columnName)
    {
        //set intial value to the first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through the data and overwrite the existing maxValue if a higher one is found
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return maxValue;
    }

    //find the min value in a column
    private float FindMinValue(string columnName)
    {
        //set intial value to the first value
        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through the data and overwrite the existing minValue if a lower one is found
        for (var i = 0; i < pointList.Count; i++)
        {
            if (minValue > Convert.ToSingle(pointList[i][columnName]))
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }
}
