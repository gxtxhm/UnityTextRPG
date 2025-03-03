using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UtilTextManager : MonoBehaviour
{
    public static UtilTextManager Instance { get; private set; }

    public bool IsUsed = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Init()
    {
        
    }
    #region 텍스트 데이터
    public static string IntroMainScene { get; } =
        "어둠의 그림자가 세상을 덮쳤다.\n"+
        "당신은 이 세계를 구할 유일한 용사로 선택받았다.\n"+
        "지금부터의 여정은 쉽지 않을 것이다.\n" +
        "당신의 선택과 용기가 모든 것을 바꿀 것이다.\n\n" + 
        "용사의 이름은 무엇입니까?";

    public static string MainMenuChoice { get; } = 
        "무엇을 하시겠습니까?\n1. 마을로 이동\n2. 던전 탐험\n3. 캐릭터 상태 확인\n4. 게임 종료\n";

    public static string EnterTown { get; } = "마을에 입장하셨습니다.";

    public static string EnterDungeon { get; } = "당신은 던전의 입구에 서 있습니다.\n" +
    "차가운 바람이 얼굴을 스치고, 어두운 안개가 던전 깊숙이 흘러갑니다.\n" +
    "입구 근처에는 오래된 경고문이 적혀 있습니다:\n" +
    "모든 준비가 갖춰졌는가? 용기를 낼 때, 비로소 길이 열릴 것이다.\n\nO. 던전에 들어간다.\nX. 마을로 돌아간다.";

    public static string ExitDungeonEntrance { get; } =
        "당신은 잠시 던전 입구에서 머뭇거리다 발길을 돌립니다. " +
        "마을로 돌아가 추가 준비를 하기로 결심합니다.\r\n" +
        "모험은 서두르지 않는 것이 좋다. 당신은 다시 한 번 마음을 가다듬습니다.\n";

    public static string ExitDungeon { get; } =
        "당신은 던전에서 벗어나 마을로 돌아옵니다. 상처를 회복하고, 다음 여정을 준비하기로 합니다.\n";

    public static string[] DungeonAppearedMonster { get; } =
    {
        "당신은 깊은 숨을 들이쉬고 던전으로 발을 내딛습니다. " +
        "어둠 속에서 당신의 발소리가 메아리칩니다.\r\n" +
        "희미한 빛줄기가 벽에 반사되며, 어디선가 들려오는 이상한 소리가 당신을 긴장하게 만듭니다.\r\n" +
        "누군가 있나... 당신의 목소리는 어둠 속으로 사라집니다.\r\n" +
        "잠시 후, 던전 입구 근처에서 들려오는 발소리에 몸을 숨기던 중, 작은 그림자가 빠르게 움직이는 것이 보입니다.\r\n" +
        "갑자기 나타난 몬스터! 덩치가 작지만 움직임이 날렵합니다.\r\n" +
        "이 몬스터를 쓰러뜨려야 더 안으로 들어갈 수 있습니다.\r\n",

        "조금 더 깊은 곳으로 들어가자, 공간이 더 좁아지고 공기가 무거워지는 것을 느낍니다.  \r\n" +
        "어둠 속에서 갑자기 거대한 소리가 울려 퍼지고, 커다란 몬스터가 당신 앞을 막아섭니다!  \r\n" +
        "이 몬스터는 이전보다 더 강력해 보입니다. 조심하세요!\r\n",

        "던전 중간에 도달했을 때, 분위기는 더욱 음산해지고 당신의 손에 땀이 흐릅니다.  \r\n" +
        "갑자기 벽이 움직이기 시작하더니, 엄청난 크기의 몬스터가 모습을 드러냅니다.  \r\n" +
        "던전의 마지막 수호자처럼 보이는 이 몬스터를 쓰러뜨리지 못하면 앞으로 나아갈 수 없습니다.\r\n"
    };

    public static string[] DungeonContinue { get; } =
    {
        "더 어두운 곳으로 발걸음을 옮겼습니다. 벽에 걸린 오래된 횃불이 바람에 흔들리고, 발밑에서 먼지가 날립니다.\r\n" +
        "또 다른 몬스터가 당신 앞에 나타납니다!",

        "던전의 중간 지점에 도달했습니다. 이상한 기운이 감돌며, 멀리서 무거운 발소리가 들립니다.\r\n" +
        "앞에 더 강한 몬스터가 기다리고 있을 것 같습니다!",

        "마침내 던전 깊숙한 곳에 도달했습니다. 커다란 문이 앞을 가로막고 있으며, 문 너머로 강력한 에너지가 느껴집니다.\r\n" +
        "최종 보스와의 전투를 준비하세요!"
    };

    public static string ChoiceMenuInBattle { get; } =
        "무엇을 하시겠습니까?\n1. 공격\n2. 아이템 사용\n3. 도망";

    public static string PlayerDead { get; } = 
        "몬스터가 당신을 공격했습니다. 치명적인 공격을 받았습니다!\r\n" +
            "당신은 쓰러졌습니다...\r\n" +
            "어두운 시야 속에서, 당신은 마지막으로 희미한 빛을 떠올립니다.\r\n" +
            "게임 오버. 마을로 돌아갑니다...\n";

    public static string NextStepChoice { get; } = 
        "다음 행동을 선택하세요:\r\n" +
        "1. 던전 안으로 더 깊이 들어간다.\r\n" +
        "2. 마을로 돌아간다.\r\n" +
        "3. 주변을 탐색한다.";

    public static string MoveTownAfterBattle { get; } =
        "당신은 던전에서 벗어나 마을로 돌아옵니다. 상처를 회복하고, 다음 여정을 준비하기로 합니다.";

    public static string AppearedBoss { get; } =
        "문이 천천히 열리며, 엄청난 크기의 그림자가 드러납니다. " +
        "몬스터가 당신을 주시하며 낮게 으르렁거립니다.\r\n" +
        "이제 최후의 결전을 시작합니다!\r\n";

    public static string ChoiceMenuBoss { get; } =
        "무엇을 하시겠습니까?\r\n" +
        "1. 공격\r\n2. 아이템 사용\r\n3. 전략적으로 후퇴";

    public static string RetreatBoss { get; }=
        "몬스터의 압도적인 위용에 당신은 잠시 망설이다가 뒤로 물러섭니다.\r\n" +
        "그러나 보스는 쉽게 놓아주지 않습니다! 문이 닫히며 당신의 도망길을 막습니다.\r\n" +
        "다시 마음을 가다듬고 싸울 준비를 해야 합니다.";

    public static string AttackBoss { get; } =
        "당신은 칼을 단단히 쥐고 몬스터에게 달려듭니다.\r\n" +
        "보스는 거대한 팔을 휘둘러 공격하지만, 당신은 이를 간신히 피합니다.\r\n" +
        "전투가 치열하게 펼쳐집니다!";

    public static string ClearBoss { get; } =
        "당신은 최종 보스를 물리치고 세계를 구했습니다. 마을 사람들은 당신을 영웅으로 칭송합니다.\r\n" +
        "새로운 모험이 기다리고 있을지도 모릅니다. 다시 도전하시겠습니까?";
    #endregion
    // 문자열 받아서 한글자씩 띄우는 함수 만들기
    public void PrintStringByTick(string s, float interval,TextMeshProUGUI text,Action action,bool isReset=false)
    {
        if(IsUsed)
            StartCoroutine(WaitUsed(s,interval,text,action,isReset));
        else
            StartCoroutine(PlayByTick(s, interval, text,action,isReset));
    }

    IEnumerator WaitUsed(string s, float interval, TextMeshProUGUI text, Action action, bool isReset)
    {
        while(IsUsed)
        {
            yield return null;
        }
        StartCoroutine(PlayByTick(s,interval, text,action,isReset));
    }

    IEnumerator PlayByTick(string s, float interval, TextMeshProUGUI text, Action action,bool isReset)
    {
        IsUsed = true;
        text.text = (isReset==true)? "" : text.text+"\n";

        ScrollRect sc = text.GetComponentInParent<ScrollRect>();

        foreach (char c in s)
        {
            if(GameManager.Instance.IsSkip)
            {
                text.text = s;
                UIManager.Instance.UpdateCanvas(sc);
                break;
            }
            text.text += c;
            UIManager.Instance.UpdateCanvas(sc);
            yield return new WaitForSeconds(interval);
        }
        while(Input.GetKeyDown(KeyCode.Space)==false)
        {
            yield return null;
        }
        UIManager.Instance.UpdateCanvas(sc);
        action?.Invoke();
        //UIManager.Instance.UpdateUI();
        GameManager.Instance.IsSkip = false;
        IsUsed = false;
    }
}