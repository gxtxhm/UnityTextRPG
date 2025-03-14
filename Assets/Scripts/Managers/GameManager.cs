using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum ResultBattle
{
    PlayerDie,
    MonsterDie,
    RetreatPlayer
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsLoadData = false;

    public bool IsSkip = false;

    public bool IsPlayingCo = false;

    public static int MonsterCount { get; } = BattleSetting.MonsterCount;

    private GameObject PlayerObj;
    public Player Player;
    public List<Monster> monsters;

    public int CurCount = 0;
    Boss _boss;
    public Boss Boss { get { return _boss; } }

    public void Init()
    {
        // Test
        //Debug.Log($"{ItemManager.Instance.Inventory.Count}");
        for (int i = 0; i < 5; i++)
        {
            Item it = ItemManager.Instance.RandomCreateItem();
            //Debug.Log($"{it.Name} : {it.Type}");
        }


        // End Test

        CurCount = 0;
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        PlayerObj = Instantiate(playerPrefab);
        Player = PlayerObj.GetComponent<Player>();
        
        monsters = new List<Monster>();
        GameObject monsterprefab = Resources.Load<GameObject>("Prefabs/Monster");
        for (int i = 0; i < MonsterCount; i++)
        {
            monsters.Add(Instantiate(monsterprefab).GetComponent<Monster>());
            monsters[i].OnDeadEvent += BroadcastMonsterDead;
            monsters[i].OnDeadEvent += KillMonster;
        }

        _boss = Instantiate(Resources.Load<GameObject>("Prefabs/Boss")).GetComponent<Boss>();
        _boss.OnDeadEvent += BroadcastMonsterDead;
        _boss.OnDeadEvent += KillMonster;
        // 이벤트 등록

        Player.OnDeadEvent += GameOver;
        Player.OnAttackEvent += BroadcastPlayerAttack;

        //ItemManager.Instance.LoadItemsFromJson();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(PlayerObj);
        foreach (Monster monster in monsters)
            DontDestroyOnLoad(monster);
        DontDestroyOnLoad(Boss);

        StartCoroutine(LoadStartScene());
    }

    void InitFromJsonFile()
    {
        SaveLoadManager.Instance.LoadDefaultDataFromJson();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitFromJsonFile();
        //UIManager.Instance.LoadFilePanelInit();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(UtilTextManager.Instance.IsUsed && IsSkip==false)
            {
                IsSkip = true;
            }
        }
    }

    public void LoadDungeonData(ProgressGameData data)
    {
        CurCount = data.currentFloor;
    }

    public void KillMonster()
    {
        string s;
        Monster m = GetCurMonster();

        Action a;

        if (m is Boss)
        {
            s = UtilTextManager.ClearBoss;
            a = () => { UIManager.Instance.EndGameUI(); };
        }
        else
        {
            s = $"{GetCurMonster().Name}을 물리쳤습니다! " +
             $"경험치 {GetCurMonster().Exp}를 획득했습니다.";
            a = () =>
            {
                Player.Exp += GetCurMonster().Exp;
            };
        }
        UtilTextManager.Instance.PrintStringByTick(s
            , 0.05f, UIManager.Instance.BattleContext,a);

    }

    public Monster GetCurMonster()
    {
        if (CurCount < BattleSetting.MonsterCount)
            return monsters[CurCount];
        else
            return Boss;
    }

    public List<Monster> FindHalfHpMonster()
    {
        return monsters?.Where(n => n.Hp < n.MaxHp / 2).ToList() ?? new List<Monster>();
    }

    public void ResetMonter()
    {
        foreach (Monster monster in monsters)
        {
            monster.Hp = 30 * monster.Id;
        }
    }

    public void PrintPlayerInfo()
    {
        //Debug.Log("캐릭터 상태창입니다.");
        GameObject item = UIManager.Instance.CreatePlayerInfoPanel();
        if (item == null) return;
    }

    public void MoveTown()
    {
        //Debug.Log(UtilTextManager.EnterTown);
        StartCoroutine(LoadMainScene());
    }

    // 로드 배틀씬 후에 배틀씬에서의 동작완성하기
    public void MoveDungeon()
    {
        //Debug.Log(UtilTextManager.EnterDungeon);

        StartCoroutine(LoadBattleScene());
    }

    IEnumerator LoadBattleScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("BattleScene");

        while (!operation.isDone)
        {
            yield return null;
        }
        UIManager.Instance.SetBattleSceneUI();
    }

    public void NextStep()
    {
        UIManager.Instance.CharacterInfoObject.SetActive(false);

        //Debug.Log(UtilTextManager.NextStepChoice);
        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.NextStepChoice, 0.05f,
            UIManager.Instance.BattleContext, () => { UIManager.Instance.CreateNextChoicePanel(); }, true);
    }

    public void OnMoveTownAfterDungeonButton()
    {
        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.MoveTownAfterBattle, 0.05f,
            UIManager.Instance.BattleContext, () => {/*TODO : Fade  Out */ },true);
    }

    public void OnContinueButton()
    {
        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.DungeonContinue[CurCount], 0.05f,
            UIManager.Instance.BattleContext, () => {
                CurCount++;
                UIManager.Instance.EnemyNameText.GetComponent<HoverText>().SetPanel(GetCurMonster(),true);
                PlayDungeon(Player);
            }, true);
    }

    public void OnExploreButton()
    {
        Explore();
    }

    public void Explore()
    {
        //0~99 범위의 난수 생성
        int randomValue = UnityEngine.Random.Range(0, 100);

        if (randomValue < 75)
        {
            Item randitem = ItemManager.Instance.RandomCreateItem();
            UtilTextManager.Instance.PrintStringByTick("당신은 주변을 탐색하던 중 희미하게 빛나는 물체를 발견했습니다.\r\n" +
                $"가까이 다가가 확인하니, {randitem.Name}을(를) 발견했습니다!\r\n" +
                "이 아이템은 당신의 여정에 큰 도움이 될 것입니다.",0.05f,UIManager.Instance.BattleContext,
                () => { });
        }
        else
        {
            UtilTextManager.Instance.PrintStringByTick("당신은 주변을 탐색했지만, 특별한 것을 발견하지 못했습니다.\r\n" +
                "어둠 속에서는 아무것도 보이지 않으며, 조용히 다시 길을 준비합니다.",0.05f,
                UIManager.Instance.BattleContext, () => { });
        }

        UtilTextManager.Instance.PrintStringByTick(UtilTextManager.DungeonContinue[CurCount],0.01f,
            UIManager.Instance.BattleContext, () => {
                CurCount++;
                UIManager.Instance.EnemyNameText.GetComponent<HoverText>().SetPanel(GetCurMonster(),true);
                PlayDungeon(Player);
            },true);
    }

    public void PlayDungeon(Player player)
    {
        // 던전 입장
        if (CurCount < BattleSetting.MonsterCount)
        {
            UtilTextManager.Instance.PrintStringByTick(UtilTextManager.DungeonAppearedMonster[CurCount], 0.05f,
                UIManager.Instance.BattleContext, () =>
                {
                    UIManager.Instance.UpdateUI();
                    BattleManager.Instance.StartBattle(player, GetCurMonster());
                }, true);
        }
        else
        {
            // 보스등장
            UtilTextManager.Instance.PrintStringByTick(UtilTextManager.AppearedBoss,0.05f,UIManager.Instance.BattleContext,
                () => {
                    UIManager.Instance.UpdateUI();
                    BattleManager.Instance.StartBattle(player, Boss);
                },true);
         }
    }

    void GameOver()
    {
        Debug.Log("플레이어가 사망하여 게임이 종료되었습니다. in GameManager");
        UtilTextManager.Instance.PrintStringByTick("플레이어가 사망하여 게임이 종료되었습니다", 0.05f,
            UIManager.Instance.BattleContext, () => { });
    }

    void BroadcastPlayerAttack()
    {
        Debug.Log("GameManager : 플레이어가 공격을 시도합니다!");
    }

    void BroadcastMonsterDead()
    {
        Debug.Log("몬스터가 사망합니다!");
    }

    void BroadcastUseItemLog(string s)
    {
        Debug.Log(s);
    }

    // UI 버튼관련
    public void OnStartButton()
    {
        MoveTown();
    }
    

    IEnumerator LoadMainScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        
        while (!operation.isDone)
        {
            yield return null;
        }
        UIManager.Instance.SetMainSceneUI();
    }

    public void OnEndButton()
    {
        Application.Quit();
    }

    public void OnLoadButton()
    {
        UIManager.Instance.LoadFilePanel.SetActive(true);
    }

    // Rule : 모든 Cancel버튼은 그 패널의 바로 아래에 자식으로 둔다.
    public void OnCancelButton()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        clickedButton.transform.parent.gameObject.SetActive(false);
    }

    public void OnMainMenuButton()
    {
        StartCoroutine(LoadStartScene());
    }

    IEnumerator LoadStartScene()
    {
        //
        IsLoadData = false;
        AsyncOperation operation = SceneManager.LoadSceneAsync("StartScene");
        while(!operation.isDone)
        {
            yield return null;
        }
        UIManager.Instance.SetStartSceneUI();
    }

    public void OnEndEditIntroInputField(string s)
    {
        //Debug.Log(s);
        Player.Name = s;
    }
}