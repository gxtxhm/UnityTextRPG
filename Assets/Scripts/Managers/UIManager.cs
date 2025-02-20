using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager
{
    public static UIManager Instance { get; private set; }=new UIManager();

    // StartScene ����
    public GameObject startBtn;
    public GameObject endBtn;

    // MainScene ����
    public GameObject playerInfoBtn;
    public GameObject EnterDungeonBtn;
    public GameObject MainMenuBtn;

    public void SetStartSceneUI()
    {
        startBtn = GameObject.Find("StartBtn");
        endBtn = GameObject.Find("EndBtn");

        if(startBtn == null || endBtn == null )
        {
            Debug.Log("Error Find Buttons in SetStartSceneUI");return;
        }

        startBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnStartButton);
        endBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnEndButton);
    }

    public void SetMainSceneUI()
    {
        playerInfoBtn = GameObject.Find("PlayerInfoBtn");
        EnterDungeonBtn = GameObject.Find("EnterDungeonBtn");
        MainMenuBtn = GameObject.Find("MainMenuBtn");

        if(playerInfoBtn == null || EnterDungeonBtn==null ||
            MainMenuBtn == null)
        {
            Debug.Log("Error Find Buttons in SetMainSceneUI");return;
        }

        playerInfoBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PrintPlayerInfo);
        EnterDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveDungeon);
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnMainMenuButton);
    }


}