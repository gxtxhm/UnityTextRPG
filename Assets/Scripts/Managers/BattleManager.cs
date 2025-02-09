using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class BattleManager
{
    public static BattleManager Instance = new BattleManager();


    public void Init()
    {

    }

    public ResultBattle StartBattle(Player player, Monster monster)
    {
        Debug.Log($"{monster.Name}이(가) 당신을 공격합니다!\n");
        int choice;
        while (player.Hp > 0 && monster.Hp > 0)
        {
            Debug.Log(UtilTextManager.ChoiceMenuInBattle);
            choice = int.Parse(Console.ReadLine());
            if (choice == 3)
            {
                if (monster is Boss)
                    Debug.Log(UtilTextManager.RetreatBoss);
                else
                {
                    Debug.Log(UtilTextManager.ExitDungeon);
                    return ResultBattle.RetreatPlayer;
                }
            }
            else if (choice == 2)
            {
                // 인벤토리 보여주기
                ItemManager.Instance.PrintInventory();
            }
            else
            {
                Debug.Log($"용사{player.Name}가 {monster.Name}을 공격!");
                player.Attack(monster);
                if (monster.Hp <= 0) return ResultBattle.MonsterDie;
                monster.Attack(player);
            }
        }
        return ResultBattle.PlayerDie;
    }
}