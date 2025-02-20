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

public class Player : MonoBehaviour, IGameCharacter
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
    public int Exp { get { return _exp; } 
        set 
        {
            while (value >= MaxExp)
            {
                Level++;
                Debug.Log($"레벨업 했습니다! 현재 레벨 : {Level}");
                AttackPower *= 2;
                Hp = 100;
                MaxExp *= 2;
                value = (value - MaxExp > 0)? value-MaxExp : 0;
            }
                _exp = value; 
        } 
    }
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

    public void PlayAttack()
    {
        Debug.Log("플레이어가 공격모션을 실행합니다. in Player");
    }

    public void Attack(Monster monster)
    {
        OnAttackEvent?.Invoke();
        monster.TakeDamage(AttackPower);

    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"몬스터에게 데미지{damage}를 입었습니다! 현재체력 : {Hp}");
        Hp -= (int)(damage*DefenseRate);
    }

    public void GetExp(int exp)
    {
        Exp += exp;
    }

    void Dead()
    {
            
    }
}
