using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance { get; private set; }

    // StartScene 관련
    public GameObject startBtn;
    public GameObject endBtn;

    // MainScene 관련
    public GameObject playerInfoBtn;
    public GameObject EnterDungeonBtn;
    public GameObject MainMenuBtn;

    public GameObject IntroPanel;
    public TextMeshProUGUI IntroText;
    public GameObject IntroInputField;
    public GameObject LastText;
    [SerializeField]
    GameObject PlayerInfoPanel;

    // BattleScene 관련
    public GameObject optionBtn1;
    public GameObject optionBtn2;
    public GameObject optionBtn3;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetStartSceneUI()
    {
        startBtn = GameObject.Find("StartBtn");
        endBtn = GameObject.Find("EndBtn");

        if (startBtn == null || endBtn == null)
        {
            Debug.Log("Error Find Buttons in SetStartSceneUI"); return;
        }

        startBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnStartButton);
        endBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnEndButton);

    }

    public void SetMainSceneUI()
    {
        playerInfoBtn = GameObject.Find("PlayerInfoBtn");
        EnterDungeonBtn = GameObject.Find("EnterDungeonBtn");
        MainMenuBtn = GameObject.Find("MainMenuBtn");
        IntroPanel = GameObject.Find("IntroPanel");
        IntroText = GameObject.Find("IntroText").GetComponent<TextMeshProUGUI>();
        IntroInputField = GameObject.Find("InputField");
        IntroInputField.SetActive(false);
        LastText = GameObject.Find("LastText");
        LastText.SetActive(false);

        if (playerInfoBtn == null || EnterDungeonBtn == null ||
            MainMenuBtn == null || IntroPanel == null ||
            IntroText == null || IntroInputField == null ||
            LastText == null)
        {
            Debug.Log("Error Find Buttons in SetMainSceneUI");return;
        }

        playerInfoBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PrintPlayerInfo);
        EnterDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveDungeon);
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnMainMenuButton);
        IntroInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(CompleteInputName);

        UtilTextManager.Instance.PrintStringByTick("hi"/*UtilTextManager.IntroMainScene*/, 0.05f, IntroText, 
            () => { IntroInputField.SetActive(true); });
    }

    void CompleteInputName(string s)
    {
        IntroInputField.SetActive(false);
        GameManager.Instance.OnEndEditIntroInputField(s);
        LastText.SetActive(true);
        UtilTextManager.Instance.PrintStringByTick($"{s}님 환영합니다!", 0.1f,
            LastText.GetComponent<TextMeshProUGUI>(),
            () => { StartCoroutine(FadeOutCo(IntroPanel, 1f)); });
    }

    public void SetBattleSceneUI()
    {
        GameObject buttonsPanel = GameObject.Find("ButtonsPanel");
        if (buttonsPanel == null) return;
        optionBtn1 = buttonsPanel.transform.Find("Option1").gameObject;
        optionBtn2 = buttonsPanel.transform.Find("Option2").gameObject;
        optionBtn3 = buttonsPanel.transform.Find("Option3").gameObject;
    }

    
    public IEnumerator FadeOutCo(GameObject panel, float duration)
    {
        CanvasGroup i = panel.GetComponent<CanvasGroup>();
        
        float time = 0f;

        while(time < duration)
        {
            time += Time.deltaTime;
            i.alpha = Mathf.Lerp(1, 0, time / duration);
            yield return null;
        }
        panel.SetActive(false);
    }

    public GameObject CreatePlayerInfoPanel()
    {
        GameObject playerInfoPanelPrefab = Resources.Load<GameObject>("Prefabs/PlayerInfoPanel");
        GameObject panel = Instantiate(playerInfoPanelPrefab);
        panel.transform.SetParent(GameObject.Find("Canvas").transform,false);
        Player p = GameManager.Instance.Player;
        panel.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy(panel); });
        TextMeshProUGUI text = panel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "이름 : " + p.Name+"\n";
        text.text += "레벨 : " + p.Level + "\n";
        text.text += $"경험치 : {p.Exp}/{p.MaxExp}\n";
        text.text += $"체력 : {p.Hp}/{p.MaxHp}\n";
        text.text += "공격력 : " + p.AttackPower + "\n";
        text.text += "방어력(비율) : " + p.DefenseRate + "\n";
        // 플레이어정보출력에서 인벤토리 목록도 출력하기
        text.text += "\n 인벤토리 목록\n";
        foreach(var item in ItemManager.Instance.Inventory)
        {
            text.text += $"-{item.Key} : {item.Value.Count}\n";
        }

        text.color = Color.black;
        PlayerInfoPanel = panel;
        return PlayerInfoPanel;
    }
    
}