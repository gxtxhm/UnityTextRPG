using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IGameCharacter
{
    public int Hp { get; set; }
    public int AttackPower { get; set; }
    public string Name { get;}

    //delegate void OnDead();
    //event Action OnDeadEvent;
    event UnityAction OnDeadEvent;
    
    //delegate void OnAttack();
    //event Action OnAttackEvent;
    event UnityAction OnAttackEvent;

    
}


public class Characters : MonoBehaviour
{
    //public int Hp { get; set; }
    //public int AttackPower { get; set; }
    //public string Name { get; set; }


}
