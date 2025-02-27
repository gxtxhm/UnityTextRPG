using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum ResultBattle
{
    PlayerDie,
    MonsterDie,
    RetreatPlayer
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    

    public static int MonsterCount { get; } = 3;

    private GameObject PlayerObj;
    public Player Player;
    public List<Monster> monsters;

    Boss _boss;
    public Boss Boss { get { return _boss; } }

    public void Init()
    {
        //Player = new Player();
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        PlayerObj = Instantiate(playerPrefab);
        Player = PlayerObj.GetComponent<Player>();
        monsters = new List<Monster>();
        for (int i = 0; i < MonsterCount; i++)
        {
            monsters.Add(new Monster());
            monsters[i].OnDeadEvent += BroadcastMonsterDead;
        }

        _boss = new Boss();
        _boss.OnDeadEvent += BroadcastMonsterDead;
        // 이벤트 등록

        Player.OnDeadEvent += GameOver;
        Player.OnAttackEvent += BroadcastPlayerAttack;

        //ItemManager.Instance.LoadItemsFromJson();
        ItemManager.Instance.OnUsedItem += BroadcastUseItemLog;

        //Player.PrintPropertyValueByReflection(Player);

        
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(Player);
            DontDestroyOnLoad(PlayerObj);
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void PlayStartScene()
    {
        /*PrintStringByTick("어둠의 그림자가 세상을 덮쳤다.\n" +
        "당신은 이 세계를 구할 유일한 용사로 선택받았다.\n" +
        "지금부터의 여정은 쉽지 않을 것이다.\n" +
        "당신의 선택과 용기가 모든 것을 바꿀 것이다.\n\n",30);*/


        Debug.Log("용사의 이름은 무엇인가?\n");
        Player.Name = Console.ReadLine();
    }

    public void PrintPlayerInfo()
    {
        Debug.Log("캐릭터 상태창입니다.");
        //Player.PrintInfo();
        GameObject item = UIManager.Instance.CreatePlayerInfoPanel();
        ItemManager.Instance.PrintInventory();
    }

    //public void ExitGame()
    //{
    //    Debug.Log("게임을 종료합니다.");
    //    Environment.Exit(0);
    //}

    //public void PrintMainMenu()
    //{
    //    Debug.Log(UtilTextManager.MainMenuChoice);
    //}



    public void MoveTown()
    {
        Debug.Log(UtilTextManager.EnterTown);
    }

    public void MoveDungeon()
    {
        Debug.Log(UtilTextManager.EnterDungeon);

        //PlayDungeon(Player);
    }

    void PlayDungeon(Player player)
    {
        // 던전 입장
        int count = 0;// 몬스터 등장 횟수
        int choice;
        for (int i = 0; i < GameManager.MonsterCount; i++)
        {
            Debug.Log(UtilTextManager.DungeonAppearedMonster[count]);

            ResultBattle result = BattleManager.Instance.StartBattle(player, monsters[count]);

            if (result == ResultBattle.RetreatPlayer) return;
            else if (result == ResultBattle.PlayerDie)
            {
                Debug.Log(UtilTextManager.PlayerDead); return;
            }
            else
            {
                Debug.Log($"{GameManager.Instance.monsters[count].Name}을 물리쳤습니다! " +
                    $"경험치 {GameManager.Instance.monsters[count].Exp}를 획득했습니다.\r\n");
                player.GetExp(GameManager.Instance.monsters[count].Exp);
            }

            Debug.Log(UtilTextManager.NextStepChoice);

            choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Debug.Log(UtilTextManager.DungeonContinue[count]);
            }
            else if (choice == 2)
            {
                Debug.Log(UtilTextManager.MoveTownAfterBattle);
                return;
            }
            else
            {

                // 0~99 범위의 난수 생성
                int randomValue = UnityEngine.Random.Range(0, 100);

                if (randomValue < 45)
                {
                    Item randitem = ItemManager.Instance.RandomCreateItem();
                    Debug.Log("당신은 주변을 탐색하던 중 희미하게 빛나는 물체를 발견했습니다.\r\n" +
                        $"가까이 다가가 확인하니, {randitem.Name}을(를) 발견했습니다!\r\n" +
                        "이 아이템은 당신의 여정에 큰 도움이 될 것입니다.");
                }
                else
                {
                    Debug.Log("당신은 주변을 탐색했지만, 특별한 것을 발견하지 못했습니다.\r\n" +
                        "어둠 속에서는 아무것도 보이지 않으며, 조용히 다시 길을 준비합니다.");
                }

                Debug.Log(UtilTextManager.DungeonContinue[count]);
            }
            count++;
        }

        // 보스등장
        Debug.Log(UtilTextManager.AppearedBoss);

        ResultBattle resultBattle = BattleManager.Instance.StartBattle(player, GameManager.Instance.Boss);

        if (resultBattle == ResultBattle.PlayerDie)
        {
            Debug.Log(UtilTextManager.PlayerDead); return;
        }
        else
        {
            Debug.Log($"{GameManager.Instance.Boss.Name}을 물리쳤습니다! " +
                $"경험치 {GameManager.Instance.Boss.Exp}를 획득했습니다.\r\n");
            player.GetExp(GameManager.Instance.Boss.Exp);

            Debug.Log(UtilTextManager.ClearBoss);
        }
    }

    void GameOver()
    {
        Debug.Log("플레이어가 사망하여 게임이 종료되었습니다. in GameManager");
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
        Debug.Log("StartBtn");
        StartCoroutine(LoadMainScene());
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
        Debug.Log("EndBtn");
        Application.Quit();
    }

    public void OnMainMenuButton()
    {
        StartCoroutine(LoadStartScene());
    }

    IEnumerator LoadStartScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("StartScene");
        while(!operation.isDone)
        {
            yield return null;
        }
        UIManager.Instance.SetStartSceneUI();
    }

    public void OnEndEditIntroInputField(string s)
    {
        Debug.Log(s);
        Player.Name = s;
        
    }
}