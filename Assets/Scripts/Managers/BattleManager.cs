﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

internal class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    
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

    // 배틀에서 3개의 선택지 오류 없애기, 이 함수랑 PlayDungeon 고쳐야함.
    public void StartBattle(Player player, Monster monster)
    {
        Debug.Log($"{monster.Name}이(가) 당신을 공격해옵니다! 어떤 선택을 하시겠습니까\n");
        UtilTextManager.Instance.PrintStringByTick($"{monster.Name}이(가) 당신을 공격해옵니다! 어떤 선택을 하시겠습니까", 0.01f,
            UIManager.Instance.BattleContext, () => { });

        // TODO : 이거 대신에 패널을 반짝이게 하던지 하자. 
        //UtilTextManager.Instance.PrintStringByTick(UtilTextManager.ChoiceMenuInBattle, 0.01f,
        //    UIManager.Instance.BattleContext, () => { });
        //Debug.Log(UtilTextManager.ChoiceMenuInBattle);

    }

    public void OnExitButton()
    {
        UIManager.Instance.DeactiveChoiceButtons();
        Monster monster = GameManager.Instance.GetCurMonster();
        if (monster is Boss)
            UtilTextManager.Instance.PrintStringByTick(UtilTextManager.RetreatBoss,0.005f,UIManager.Instance.BattleContext,
                () => { UIManager.Instance.ActiveChoiceButtons(); });
        else
        {
            UtilTextManager.Instance.PrintStringByTick(UtilTextManager.ExitDungeon,0.005f,
                UIManager.Instance.BattleContext, () => { UIManager.Instance.ActiveChoiceButtons(); });
            //return ResultBattle.RetreatPlayer;
        }
    }

    public void OnInventoryButton()
    {
        //UIManager.Instance.DeactiveChoiceButtons();
        ItemManager.Instance.PrintInventory();
        //UIManager.Instance.ActiveChoiceButtons();
    }

    public void OnAttackButton()
    {
        UIManager.Instance.DeactiveChoiceButtons();
        Player player = GameManager.Instance.Player;
        Monster monster = GameManager.Instance.GetCurMonster();
        Debug.Log($"용사{player.Name}가 {monster.Name}을 공격!");

        StartCoroutine(PlayTurn(player, monster));

        
    }

    IEnumerator PlayTurn(Player player, Monster monster)
    {
        player.Attack(monster);

        yield return new WaitUntil(()=>UtilTextManager.Instance.IsUsed == false);

        monster.Attack(player);

        yield return new WaitUntil(() => UtilTextManager.Instance.IsUsed == false);

        UIManager.Instance.ActiveChoiceButtons();
    }
}