using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenseSurfaceUIManager : MonoBehaviour
{
    public static SenseSurfaceUIManager Instance = null;

    public GameObject boardActivePanel;
    bool panelVisible = false;

    public Text boardName;
    public Text fpsCounter;
    public InputField boardWidth;
    public InputField boardHeight;
    public Slider boardActiveRX;
    public Slider boardActiveTX;
    public Slider boardRotation;
    public Slider boardGain;
    public Toggle boardMirrorX;
    public Toggle boardMirrorY;

    public Slider thresholdMin;
    public Slider thresholdMax;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Debug.LogError("Instance of SenseSurfaceUIManager altrady exists");

        ActivateBoardPanel(false);
    }

    public void UpdatePanelData()
    {
        if (SenseSurfaceBoardManager.Instance.activeSensurfaceBoard == null) {
            Debug.Log("No Active Board to Update on the panel");
            return;
        }
      //  Debug.Log("Loading" + SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileHeight);
        boardName.text = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.boardName;
        boardWidth.text = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileWidth.ToString();
        boardHeight.text = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileHeight.ToString();
        boardActiveRX.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileActiveRX;
        boardActiveTX.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileActiveTX;
        boardRotation.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileRotation;
        boardGain.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.tileGain;
        boardMirrorX.isOn = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.mirrorX;
        boardMirrorY.isOn = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.mirrorY;

        thresholdMin.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.minVal;
        thresholdMax.value = (float)SenseSurfaceBoardManager.Instance.activeSensurfaceBoard.maxVal;

    }

    public void UpdateRealTimeCorrection()
    {
        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
            return;
        }
        activeBoard.mirrorX = boardMirrorX.isOn;
        activeBoard.mirrorY = boardMirrorY.isOn;
        activeBoard.maxVal = Mathf.FloorToInt(thresholdMax.value);
        activeBoard.minVal = Mathf.FloorToInt(thresholdMin.value);

        activeBoard.SaveBoardData();
    }


    public void UpdateGain()
    {

        //  Debug.Log("Saving" + boardActiveRX.value);
        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
            return;
        }
        activeBoard.tileGain = Mathf.FloorToInt(boardGain.value);

    }


    public void UpdateBoardData()
    {

        //  Debug.Log("Saving" + boardActiveRX.value);
        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
            return;
        }

        activeBoard.tileWidth = int.Parse(boardWidth.text);
        activeBoard.tileHeight = int.Parse(boardHeight.text);
        activeBoard.tileActiveRX = Mathf.FloorToInt(boardActiveRX.value);
        activeBoard.tileActiveTX = Mathf.FloorToInt(boardActiveTX.value);
        activeBoard.tileRotation = Mathf.FloorToInt(boardRotation.value);
        activeBoard.tileGain = Mathf.FloorToInt(boardGain.value);

        UpdateRealTimeCorrection();

        activeBoard.SaveBoardData();
        activeBoard.SendResolution();
        activeBoard.SendGain();
        activeBoard.InitBoard(activeBoard.name);
    }

    public void Calibrate()
    {

        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
            return;
        }
        activeBoard.Calibrate();
    }




    public void SendGain()
    {
        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
            return;
        }
        activeBoard.SendGain();
    }


    public SenseSurfaceBoard GetActiveBoard()
    {
        SenseSurfaceBoard activeBoard = SenseSurfaceBoardManager.Instance.activeSensurfaceBoard;
        if (activeBoard == null)
        {
            Debug.Log("No Active Board to save value");
        }
            return activeBoard;
    }


    public void DeleteAllPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ActivateBoardPanel(bool active = false)
    {
        if(!active && SenseSurfaceBoardManager.Instance != null)
            SenseSurfaceBoardManager.Instance.SetActiveSensurfaceBoard(null);
        boardActivePanel.SetActive(active);
    }

    public void SetFPS(string fps)
    {
        fpsCounter.text = fps;
    }
}
