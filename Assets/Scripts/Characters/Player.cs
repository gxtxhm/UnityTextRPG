using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

public class Player : MonoBehaviour, IGameCharacter, IInfoProvider
{
    public int MaxHp { get; set; } = 100;
    int _hp;
    int _exp = 0;

    //public delegate void OnDead();
    public event UnityAction OnDeadEvent;

    //public delegate void OnAttack();
    public event UnityAction OnAttackEvent;

    public string Name { get; set; }
    public int Level { get; set; } = 1;
    public int Exp { get { return _exp; } set {GetExp(value);} }
    public int MaxExp { get; set; } = 10;
    public int Hp { get { return _hp; }
        set {
            if (value <= 0) { _hp = 0; OnDeadEvent?.Invoke(); }
            else if (value > MaxHp) { _hp = MaxHp; }
            else { _hp = value; } 
        } 
    } 
    public int AttackPower { get; set; } = 10;
    // 높을수록 데미지 더 받음. 
    public float DefenseRate { get; set; } = 1;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;


    public Player() {  }

    private void Awake()
    {
        _hp = MaxHp; OnDeadEvent += Dead; OnAttackEvent += PlayAttack;
    }

    public Player(string Name) : this()
    {
        this.Name = Name;
    }

    public string TranslateInfoToString()
    {
        string s="";
        s += "이름 : " + Name + "\n";
        s += "레벨 : " + Level + "\n";
        s += $"경험치 : {Exp}/{MaxExp}\n";
        s += $"체력 : {Hp}/{MaxHp}\n";
        s += "공격력 : " + AttackPower + "\n";
        s += "방어력(비율) : " + DefenseRate + "\n";

        return s;
    }

    public IEnumerator ExpEffect(int exp, GameObject slider, Action action = null)
    {
        while(Input.GetKeyDown(KeyCode.Space)==false)
        {
            yield return null;
        }

        action?.Invoke();
    }

    public void PlayAttack()
    {
        Debug.Log("플레이어가 공격모션을 실행합니다. in Player");
    }

    public void Attack(Monster monster)
    {
        OnAttackEvent?.Invoke();
        UtilTextManager.Instance.PrintStringByTick($"용사{Name}가 {monster.Name}을 공격!", 0.005f,
            UIManager.Instance.BattleContext, () =>
            {
                monster.TakeDamage(AttackPower);
                //if (monster.Hp <= 0)
                    //GameManager.Instance.KillMonster();

            }, false);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"몬스터에게 데미지{(int)(damage * DefenseRate)}를 입었습니다! 현재체력 : {Hp}");
        Debug.Log($"damage : {damage}, DefenseRate : {DefenseRate}");
        int calDamage = (int)(damage * DefenseRate);
        StartCoroutine(UIManager.Instance.SliderEffect(Hp, Hp - calDamage, MaxHp, UIManager.Instance.PlayerSlider, 1, 
            () => { Hp -= calDamage; UIManager.Instance.UpdateUI(); }));
        
    }

    public void HealHp(int heal)
    {
        StartCoroutine(UIManager.Instance.SliderEffect(Hp, Hp + heal, MaxHp,
            UIManager.Instance.PlayerSlider, 1,
            () => { Hp += heal; UIManager.Instance.UpdateUI(); }));
    }

    IEnumerator PlayLevelUp(int value)
    {
        bool isPlaying = false;

        while (value >= MaxExp)
        {
            isPlaying = true;
            StartCoroutine(UIManager.Instance.SliderEffect(_exp, MaxExp, MaxExp, UIManager.Instance.PlayerExpSlider, 
                1,() => { isPlaying = false; }));

            yield return new WaitUntil(()=>isPlaying==false);
            
            Level++;
            Debug.Log($"레벨업 했습니다! 현재 레벨 : {Level}");
            AttackPower *= 2;
            Hp = 100;
            _exp = 0;
            MaxExp *= 2;
            value = (value - MaxExp > 0) ? value - MaxExp : 0;

            yield return new WaitForSeconds(0.3f);
        }
        
        if(value > 0)
        {
            isPlaying = true;
            StartCoroutine(UIManager.Instance.SliderEffect(_exp, value, MaxExp, UIManager.Instance.PlayerExpSlider,1,
            () => { isPlaying = false; }));
            while (isPlaying == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            _exp = value;
        }
        UIManager.Instance.UpdateUI();

        // 키 입력 시 진행되게

        yield return new WaitUntil(() => {return Input.GetKeyDown(KeyCode.Space); });
        
        GameManager.Instance.NextStep();
    }

    public void GetExp(int value)
    {
        StartCoroutine(PlayLevelUp(value));
        
    }
    void Dead()
    {
       
    }

    public void LoadPlayerData(PlayerData data)
    {
        this.Name = data.name;
        this.Level = data.level;
        _exp = data.exp;
        this.MaxExp = data.maxExp;
        this.Hp = data.hp;
        this.MaxHp = data.maxHp;
        this.AttackPower = data.attackPower;

        Debug.Log("플레이어 데이터 로드 완료");
    }

}
