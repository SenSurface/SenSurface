using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class SenseSurfaceBoard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI")]
    public int interfacePosX;
    public int interfacePosY;

    public int tileWidth = 20;
    public int tileHeight = 20;
    [Range(0, 12)]
    public int tileActiveRX = 12;
    [Range(0, 12)]
    public int tileActiveTX = 12;
    [Range(0, 360)]
    public int tileRotation = 0;
    [Range(0, 32)]
    public int tileGain = 10;

    // Optimize for send
    int prevGain = -1;
    int prevtileActiveRX = -1;
    int prevtileActiveTX = -1;


    public bool mirrorX = false;
    public bool mirrorY = false;

    public string boardName = "";

    //Draging stuff
    private bool dragging;
    private Vector2 offset;

    //public int[] lastValues;


    public int[] lastData = null;
    public float[] mappedData = null;
    public string rawData = "";


    [Header("Touch Settings")]
    public int minVal = 0;
    public int maxVal = 255;


    [Header("Calibration")]
    public bool calibrate = false;
    int calibrationStep;
    List<int[]> calibrationsPoints = new List<int[]>();
    int[] calibrationMatrix = null;

    [Header("Interface")]
    public Texture2D normalTexture = null;
    public Transform cellContainer;
    RectTransform rectTransform;
    public GameObject cellPrefab;

    Color[] colors;
    Image[] cellsImage; // TODO : this is currebntly not used

    public int nitems = 144;

    public bool isDebugTile = false;

    [Header("FPS")]
    public string formatedString = "{value} FPS";
    float updateRateSeconds = 1.0f;
    int frameCount = 0;
    float fps = 0.0f;
    float t = 0.0f;
    float prevtt = 0.0f;



    public float timer, refresh, avgFramerate;
    string display = "{0} FPS";



    public void Awake() {
        rectTransform = this.GetComponent<RectTransform>();

        if (isDebugTile)
        {
            useAsciiOptimization = true;
            InitBoard(boardName);
            StartCoroutine(WaitAndPrint());
            minVal = 0;
        }


    }


    IEnumerator WaitAndPrint()
    {
        while (true)
        {
            string fakeData = "";
            for(int x = 0; x < tileActiveRX; x++)
            {
                for (int y = 0; y < tileActiveTX; y++)
                {
                    int rand = Mathf.FloorToInt(10 * Time.time % 7);
                    fakeData += System.Convert.ToChar(x*2 + y*2 + calibrationNumberToSubstract * rand);
                 //   fakeData += ",";
                }
            }
            //Debug.Log(fakeData); Debug.Log(tileActiveTX); Debug.Log(tileActiveRX); Debug.Log("--------------");
            yield return new WaitForSeconds(0.2f);
            //   fakeData.Remove(fakeData.Length - 1);
            UpdateData(fakeData);
        }
    }

    public void InitBoard(string name)
    {

        if(name == "")
        {
            Debug.LogError("The board has no name. Destroying");
            Destroy(this.gameObject);
            return;
        }
        if (isDebugTile)
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndPrint());
        }

        boardName = name;

        LoadBoardData();

        nitems = tileActiveRX * tileActiveTX;

        colors = new Color[nitems];
        cellsImage = new Image[nitems];
        lastData = new int[nitems];
        mappedData = new float[nitems];

        normalTexture = CreateMapTexture.CreateTexture(tileActiveRX, tileActiveTX);
        offset = Vector2.zero;


        InitVisualBoardUI();

    }

    // Visual Init
    void InitVisualBoardUI()
    {
        // Set Board Visual Size
        rectTransform.sizeDelta = new Vector2(tileWidth * 10, tileHeight * 10);
        rectTransform.rotation  = Quaternion.Euler(0f, 0f, tileRotation);

        // Destroy all cells
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);

        // Instanciate new Cells

        this.GetComponent<GridLayoutGroup>().cellSize = new Vector2(
            10.0f * tileWidth / tileActiveTX,
            10.0f * tileHeight / tileActiveRX);

        for (int i = 0; i < nitems; i++)
        {
            GameObject cell = GameObject.Instantiate(cellPrefab) as GameObject;
            cell.transform.SetParent(cellContainer);
            cellsImage[i] = cell.GetComponent<Image>();
        }
    }

    public float thresh = 0.0f;
    public bool useAsciiOptimization = true;
    int calibrationNumberToSubstract = 20;

    public void UpdateData(string data)
    {
        rawData = data;
        //float[,] cells = new float[tileActiveTX, tileActiveRX];
        // Debug.Log(data);

        if (calibrate)
            calibrationMatrix = null;

        if(useAsciiOptimization)
        {
            data.Replace(',', '+'); // replace all comas to avoid the bug
            string newData = "";

            if (data.Length == nitems)
            {
                foreach (char c in data)
                {
                    newData += (int)c;
                    newData += ",";
                }
                
                newData = newData.Remove(newData.Length - 1);
                data = newData;
            } else
            {
             //   Debug.Log("Error for " + boardName + ", length after split is " + data.Length + " instead of " + nitems + ". Truncating.");
              //  Debug.Log("Raw Converterted data:" + newData);
            }
        }

        string[] tmpData = data.Split(',');

       
        if (tmpData.Length != nitems)
        {
            string[] newTmp = new string[nitems];

          //  Debug.Log("Error for " + boardName + ", length after split is " + tmpData.Length + " instead of " + nitems + ". Truncating.");
           // Debug.Log("Raw data:" + data);
            //Debug.Log("Raw Converterted data:");
            foreach(string d in tmpData)
            {
       //         Debug.Log(d + ",");
            }
        //    Debug.Log("----");
            return;
        }
        

        // Mirror values here 



        for (int i = 0; i < nitems; i++)
        {
            //Chars
            int readedInt = -1;
            bool ok = false;

            try {
                int.TryParse(tmpData[i], out readedInt);
                ok = true;
         
            }
            catch (Exception e) {

                Debug.LogError(e);
            }


            if (ok)
            {
                readedInt = readedInt - calibrationNumberToSubstract;
                /* int index = i;
                 int x = i % tileActiveTX;    // % is the "modulo operator", the remainder of i / width;
                 int y = i / tileActiveRX;    // where "/" is an integer division
                                             //   Debug.Log("x " + x + " y " + y);

                 index = (tileActiveRX * y) + x;
             */
                if (!calibrate && calibrationMatrix != null)
                    readedInt -= calibrationMatrix[i];

                float v = Map((float)readedInt, (float)minVal, (float)maxVal, 0, 1);
                mappedData[i] = v;
                //      cells[y, x] = v;
                lastData[i] = readedInt;
                ColorCell(i, v);


            }
            else
            {
                Debug.Log("Error parsing : " + tmpData[i] + " at " + i);
            }

        }

        IncrementFPS();


        if (calibrate)
        {
            calibrationsPoints.Add(lastData);
            calibrationStep++;
            if (calibrationStep == 5)
            {
                CalculateCalibrationMatrix();
            }
        }

        normalTexture = CreateMapTexture.UpdatePixels(normalTexture, colors);
    }

    void ColorCell(int index, float v)
    {
        cellsImage[index].color = new Color(v, v, v);
        colors[index] = new Color(v, v, v);
    }

    public float Map(float x, float x1, float x2, float y1, float y2)
    {
        var m = (y2 - y1) / (x2 - x1);
        var c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.

        return Mathf.Clamp(m * x + c, y1, y2);
    }

    void CalculateCalibrationMatrix()
    {
        calibrationMatrix = new int[nitems];

        for (int i = 0; i < calibrationsPoints[0].Length; i++)
        {
            int avg = 0;
            for (int t = 0; t < calibrationsPoints.Count; t++)
            {
                avg += calibrationsPoints[t][i];
            }
            avg = avg / calibrationsPoints.Count;
            calibrationMatrix[i] = avg;
        }

        calibrate = false;
        calibrationStep = 0;
        Debug.Log("Calibration done");
    }





    // Save and Load


    public void SaveBoardData()
    {
        Debug.Log("Saving data for " + boardName);
        PlayerPrefs.SetInt(boardName + "_interfacePosX", interfacePosX);
        PlayerPrefs.SetInt(boardName + "_interfacePosY", interfacePosY);
        PlayerPrefs.SetInt(boardName + "_tileWidth", tileWidth);
        PlayerPrefs.SetInt(boardName + "_tileHeight", tileHeight);
        PlayerPrefs.SetInt(boardName + "_tileActiveRX", tileActiveRX);
        PlayerPrefs.SetInt(boardName + "_tileActiveTX", tileActiveTX);
        PlayerPrefs.SetInt(boardName + "_tileRotation", tileRotation);

        PlayerPrefs.SetInt(boardName + "_mirrorX", mirrorX ? 1 : 0 );
        PlayerPrefs.SetInt(boardName + "_mirrorY", mirrorY ? 1 : 0);
        PlayerPrefs.SetInt(boardName + "_maxVal", maxVal);
        PlayerPrefs.SetInt(boardName + "_minVal", minVal);

    }

    void LoadBoardData()
    {
        if (boardName == "") return;
        if (!PlayerPrefs.HasKey(boardName + "_interfacePosX"))
        {
            Debug.Log("No key for board " + boardName);
            return;
        }

        interfacePosX =  PlayerPrefs.GetInt(boardName + "_interfacePosX");
        interfacePosY  = PlayerPrefs.GetInt(boardName + "_interfacePosY");
        tileWidth = PlayerPrefs.GetInt(boardName + "_tileWidth");
        tileHeight = PlayerPrefs.GetInt(boardName + "_tileHeight");
        tileActiveRX = PlayerPrefs.GetInt(boardName + "_tileActiveRX");
        tileActiveTX = PlayerPrefs.GetInt(boardName + "_tileActiveTX");
        tileRotation = PlayerPrefs.GetInt(boardName + "_tileRotation");

        mirrorX = PlayerPrefs.GetInt(boardName + "_mirrorX") == 1 ? true : false;
        mirrorY = PlayerPrefs.GetInt(boardName + "_mirrorY") == 1 ? true : false;
        maxVal = PlayerPrefs.GetInt(boardName + "_maxVal");
        minVal = PlayerPrefs.GetInt(boardName + "_minVal");


       // Debug.Log("Loading data for " + boardName + " : ");
      //  Debug.Log(interfacePosX + " " + interfacePosY );

        SetVisualSettings();
    }

    public void SendGain()
    {
        if (prevGain != tileGain)
        {
            Debug.Log("Sending Gain");
            UduinoManager.Instance.sendCommand("g", tileGain);
            prevGain = tileGain;
        //    Calibrate();
        }
    }

    public void SendResolution()
    {
        if (prevtileActiveRX != tileActiveRX || prevtileActiveTX != tileActiveTX)
        {
            UduinoManager.Instance.sendCommand("reso", tileActiveRX, tileActiveTX);
            prevtileActiveRX = tileActiveRX;
            prevtileActiveTX = tileActiveTX;

            Calibrate();
        }
    }

    public void Calibrate()
    {
        UduinoManager.Instance.sendCommand("c");

    }

    public void SetVisualSettings()
    {
      //  transform.position = new Vector2(interfacePosX, interfacePosX);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(interfacePosX, interfacePosY);
    }

    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
            interfacePosX = Mathf.FloorToInt(GetComponent<RectTransform>().anchoredPosition.x);
            interfacePosY = Mathf.FloorToInt(GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SenseSurfaceBoardManager.Instance.SetActiveSensurfaceBoard(this);
        dragging = true;
        offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        if (Vector2.Distance(offset, Vector2.zero) >= 0.01f) {
            //Debug.Log("Pointer up and saving");
          //  Debug.Log(offset);
            SaveBoardData();
            }
        offset = Vector2.zero;
        //   SenseSurfaceBoardManager.Instance.SetActiveSensurfaceBoard(null);
    }

    public void IncrementFPS()
    {
        frameCount++;
        t += Time.time - prevtt;
        if (t > 1.0f)
        {
            fps = frameCount;
            frameCount = 0;
            t = 0;
        }
        prevtt = Time.time;
        string tt = formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0"));

        SenseSurfaceUIManager.Instance.SetFPS(tt);




        //Change smoothDeltaTime to deltaTime or fixedDeltaTime to see the difference
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        String ttt = string.Format(display, avgFramerate.ToString());

        SenseSurfaceUIManager.Instance.SetFPS(tt);


    }





}
