using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Xml.Schema;

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
    public GameObject ExitDungeonBtn;
    public GameObject InventoryBtn;
    public GameObject AttackBtn;

    public TextMeshProUGUI BattleContext;
    public GameObject CharacterInfoObject;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI EnemyNameText;
    public GameObject PlayerSlider;
    public GameObject EnemySlider;
    public GameObject PlayerExpSlider;
    public GameObject LevelText;

    public GameObject NextChoicePanel;

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
            Debug.Log("Error Find Buttons in SetMainSceneUI"); return;
        }

        playerInfoBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.PrintPlayerInfo);
        EnterDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveDungeon);
        MainMenuBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.OnMainMenuButton);
        IntroInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(CompleteInputName);

        //IntroInputField.SetActive(false);
        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.IntroMainScene, 0.05f, IntroText,
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
        BattleContext = GameObject.Find("BattleContext").GetComponent<TextMeshProUGUI>();
        PlayerNameText = GameObject.Find("PlayerText").GetComponent<TextMeshProUGUI>();
        EnemyNameText = GameObject.Find("EnemyText").GetComponent<TextMeshProUGUI>();

        CharacterInfoObject = GameObject.Find("CharacterInfo");
        PlayerSlider = GameObject.Find("PlayerSlider");
        EnemySlider = GameObject.Find("EnemySlider");
        PlayerExpSlider = GameObject.Find("PlayerExpSlider");
        LevelText = GameObject.Find("LevelText");

        CharacterInfoObject.SetActive(false);

        if (buttonsPanel == null || BattleContext == null
            || PlayerNameText == null || EnemyNameText == null ||
            PlayerSlider == null || EnemySlider == null ||
            PlayerExpSlider == null || LevelText == null)
        {
            Debug.Log("SetBattleSceneUI Error");
            return;
        }
        
        ExitDungeonBtn = buttonsPanel.transform.Find("ExitDeungeonBtn").gameObject;
        ExitDungeonBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnExitButton);

        InventoryBtn = buttonsPanel.transform.Find("InventoryBtn").gameObject;
        InventoryBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnInventoryButton);

        AttackBtn = buttonsPanel.transform.Find("AttackBtn").gameObject;
        AttackBtn.GetComponent<Button>().onClick.AddListener(BattleManager.Instance.OnAttackButton);

        //ExitDungeonBtn.GetComponent<Button>().onClick.AddListener(GameManager.Instance.MoveTown);

        GameObject choicebox = CreateItemUI("Prefabs/ChoiceBox");
        Button[] buttons = choicebox.GetComponentsInChildren<Button>();
        choicebox.SetActive(false);

        buttons[0].onClick.AddListener(() => { choicebox.SetActive(false); GameManager.Instance.PlayDungeon(GameManager.Instance.Player); });
        buttons[1].onClick.AddListener(()=> { choicebox.SetActive(false); GameManager.Instance.MoveTown();  });
        choicebox.transform.SetParent(GameObject.Find("Canvas").transform, false);

        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.EnterDungeon, 0.05f, BattleContext, () => { choicebox.SetActive(true); });
        UpdateUI();
    }

    public GameObject CreateItemUI(string address)
    {
        GameObject item = Resources.Load<GameObject>(address);
        GameObject gameObject = Instantiate(item);

        return gameObject;
    }

    public void UpdateUI()
    {
        Monster monster = GameManager.Instance.GetCurMonster();
        PlayerNameText.text = GameManager.Instance.Player.Name;
        PlayerSlider.GetComponentInChildren<Slider>().value = 
            (float)GameManager.Instance.Player.Hp / GameManager.Instance.Player.MaxHp;
        PlayerSlider.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.Player.Hp.ToString();

        PlayerExpSlider.GetComponentInChildren<Slider>().value =
            (float)GameManager.Instance.Player.Exp / GameManager.Instance.Player.MaxExp;
        PlayerExpSlider.GetComponentInChildren<TextMeshProUGUI>().text = 
            $"{GameManager.Instance.Player.Exp.ToString()}";

        LevelText.GetComponent<TextMeshProUGUI>().text = $"Lv {GameManager.Instance.Player.Level.ToString()}";

        EnemyNameText.text = monster.Name;
        EnemySlider.GetComponentInChildren<Slider>().value =
            (float)monster.Hp / monster.MaxHp;
        EnemySlider.GetComponentInChildren<TextMeshProUGUI>().text = monster.Hp.ToString();



        CharacterInfoObject.SetActive(true);
    }
    
    public void UpdateCanvas(ScrollRect rect = null)
    {
        Canvas.ForceUpdateCanvases();
        if(rect != null)
            rect.verticalNormalizedPosition = 0f;
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
    
    // slider 와 text를 동시에 바꿈
    public IEnumerator SliderEffect(int start, int end, int maxValue, GameObject slider,float duration = 1, Action action = null)
    {
        Slider s = slider.GetComponentInChildren<Slider>();
        TextMeshProUGUI t = slider.GetComponentInChildren<TextMeshProUGUI>();

        if (s == null || t == null)
        {
            yield break;
        }

        float startValue = s.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            t.text = ((int)Mathf.Lerp(start, end, Mathf.Clamp01(elapsedTime / duration))).ToString();
            
            s.value = Mathf.Lerp(startValue, (float)end / maxValue, Mathf.Clamp01(elapsedTime / duration));
            
            yield return null;
        }
        Canvas.ForceUpdateCanvases();
        action?.Invoke();
    }

    public GameObject CreateNextChoicePanel()
    {
        if (NextChoicePanel != null) return null;
        GameObject panelPrefab = Resources.Load<GameObject>("Prefabs/NextChoiceBox");
        GameObject panel = Instantiate(panelPrefab);
        panel.transform.SetParent(GameObject.Find("Canvas").transform, false);

        NextChoicePanel = panel;

        
        Button[] buttons = panel.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => { GameManager.Instance.OnContinueButton();Destroy(panel); } );
        buttons[1].onClick.AddListener(()=> { GameManager.Instance.OnMoveTownAfterDungeonButton(); Destroy(panel); } );
        buttons[2].onClick.AddListener(()=> { GameManager.Instance.OnExploreButton(); Destroy(panel); } );

        return panel;
    }

    public GameObject CreatePlayerInfoPanel()
    {
        if (PlayerInfoPanel != null) return null;
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

    public void DeactiveChoiceButtons()
    {
        ExitDungeonBtn.GetComponent<Button>().interactable = false;
        InventoryBtn.GetComponent<Button>().interactable = false;
        AttackBtn.GetComponent<Button>().interactable = false;

        Color32 c = AttackBtn.transform.parent.GetComponent<Image>().color;
        c.a = 100;
        AttackBtn.transform.parent.GetComponent<Image>().color = c;
    }

    public void ActiveChoiceButtons()
    {
        ExitDungeonBtn.GetComponent<Button>().interactable = true;
        InventoryBtn.GetComponent<Button>().interactable = true;
        AttackBtn.GetComponent<Button>().interactable = true;

        Color32 c = AttackBtn.transform.parent.GetComponent<Image>().color;
        c.a = 10;
        AttackBtn.transform.parent.GetComponent<Image>().color = c;
    }
}