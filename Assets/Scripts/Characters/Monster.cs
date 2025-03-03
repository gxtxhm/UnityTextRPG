﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Monster : MonoBehaviour, IGameCharacter
{
    static int CountId = 1;
    [SerializeField]
    int _hp;

    [field: SerializeField]
    public int MaxHp { get; private set; }

    [field: SerializeField]
    public int Id { get; private set; }

    [field : SerializeField]
    public string Name { get; protected set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Hp { get { return _hp; } set { if (value <= 0) { _hp = 0; OnDeadEvent?.Invoke(); } else { _hp = value; } } }

    //public delegate void OnDead();
    public event UnityAction OnDeadEvent;

    //public delegate void OnAttack();
    public event UnityAction OnAttackEvent;

    public int AttackPower { get; set; }
        
    public void Awake()
    {
        Init();
    }
    void Init()
    {
        Id = CountId;
        Level = 1 * CountId;
        Exp = 10 * CountId;
        Hp = 30 * CountId;
        MaxHp = Hp;
        AttackPower = 10 * CountId / 2;
        this.Name = $"몬스터{CountId}";
        CountId++;

        OnDeadEvent += Dead;
        OnAttackEvent += PlayAttack;
    }
    public void PlayAttack()
    {
        Debug.Log($"{Name}몬스터가 공격을 시도합니다.");
    }

    public void Attack(Player player)
    {
        OnAttackEvent?.Invoke();
        UtilTextManager.Instance.PrintStringByTick($"몬스터 {Name}가 용사{player.Name}을 공격!", 0.005f,
            UIManager.Instance.BattleContext, () =>
            {
                player.TakeDamage(AttackPower);
                UIManager.Instance.UpdateUI();
            }, false);
        
    }

    public void TakeDamage(int damage)
    {
        //Hp -= damage;
        Debug.Log($"{Name}에게 데미지{damage}를 입혔습니다. {Name}의 체력 : {Hp}");

        //StartCoroutine(UIManager.Instance.SliderEffect(Hp,))
    }

    void Dead()
    {
        Debug.Log($"{CountId}번 몬스터 사망!");
    }
}