using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;

public class SenseSurfaceBoardManager : MonoBehaviour
{

    public static SenseSurfaceBoardManager Instance = null;

    Dictionary<string, SenseSurfaceBoard> boards = new Dictionary<string, SenseSurfaceBoard>();

    public GameObject sensurfaceBoardPrefab = null;
    public Transform interfaceParent = null;


    public SenseSurfaceBoard activeSensurfaceBoard = null;

    void Awake()
    {
        if (Instance != null) Debug.LogError("There is already an instance of SenseSurfaceBoardManager");
        else
            Instance = this;

        Application.targetFrameRate = 120;
    }

    public void SetActiveSensurfaceBoard(SenseSurfaceBoard target)
    {
        if (activeSensurfaceBoard != null)
        {
            activeSensurfaceBoard.transform.GetComponent<Outline>().enabled = false;
        }

        if (target != null)
        {
            //Debug.Log("A new board is clicked:" + target.transform.name);
            activeSensurfaceBoard = target;
            activeSensurfaceBoard.transform.GetComponent<Outline>().enabled = true;
            SenseSurfaceUIManager.Instance.UpdatePanelData();
            SenseSurfaceUIManager.Instance.ActivateBoardPanel(true);
        } else
        {
            activeSensurfaceBoard = null;
        }
       
    }


    void AddNewBoard(string name)
    {
        GameObject board = GameObject.Instantiate(sensurfaceBoardPrefab) as GameObject;
        board.transform.SetParent(interfaceParent);
        board.name = name;
        SenseSurfaceBoard SSBoard = board.GetComponent<SenseSurfaceBoard>();
        SSBoard.InitBoard(name);

        boards.Add(name, SSBoard);
    }


    void UpdateBoardData(UduinoDevice device, string data )
    {
        SenseSurfaceBoard temp = null;
        if (boards.TryGetValue(device.name, out temp))
        {
            temp.UpdateData(data);
        }
        else
        {
            Debug.Log("The board  " + device.name + " does not exists. Creating a new one. ");
            AddNewBoard(device.name);
            UduinoManager.Instance.sendCommand(device, "c");
        }
    }


    public void OnDataReceived(string data, UduinoDevice device)
    {

        if (data == "" || data == " " || data == null)
            return;



        UpdateBoardData(device, data);

        //TODO : ici faire des vérifications  ??
        //Debug.Log(data);
    }

}
