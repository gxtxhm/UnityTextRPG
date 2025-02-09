using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Monster : MonoBehaviour, IGameCharacter
{
    static int CountId = 1;

    int _hp = 30;

    public int MaxHp { get; } = 100;
    public int Id { get; } = CountId;
    public string Name { get; set; }
    public int Level { get; set; } = 1;
    public int Exp { get; set; } = 10;
    public int Hp { get { return _hp; } set { if (value <= 0) { _hp = 0; OnDeadEvent?.Invoke(); } else { _hp = value; } } }

    //public delegate void OnDead();
    public event UnityAction OnDeadEvent;

    //public delegate void OnAttack();
    public event UnityAction OnAttackEvent;

    public int AttackPower { get; set; } = 10;
        
    public Monster()
    {
        Level = 1*CountId;
        Exp = 10*CountId;
        Hp = 30*CountId;
        AttackPower = 10*CountId/2;
        this.Name = $"몬스터{CountId++}";

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
        player.TakeDamage(AttackPower);
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log($"{Name}에게 데미지{damage}를 입혔습니다. {Name}의 체력 : {Hp}");
    }

    void Dead()
    {
        Debug.Log($"{CountId}번 몬스터 사망!");
    }
}