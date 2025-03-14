using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public interface IUseableItem
{
    public void Use(Player player);
}
public abstract class Item : IInfoProvider
{
    public string Key { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public ItemType Type { get; protected set; }

    public string TranslateInfoToString()
    {
        string s = "";
        s += $"이름 : {Name}\n";
        s += $"효과 : {Description}";

        return s;
    }
    //public virtual void Use()
    //{
    //    Debug.Log($"{Name}아이템을 사용!");
    //}
};

public class HpPotion : Item,IUseableItem
{
    public int HealAmount { get; }

    public HpPotion()
    {
        var config = ItemManager.Instance.GetItemConfig("HpPotion");
        Key = "HpPotion";
        Type = config != null ? Enum.Parse<ItemType>(config.ItemType) : ItemType.HpPotion; // ✅ JSON에서 `ItemType` 불러오기
        Name = config != null ? config.Name : "체력 포션";
        Description = config != null ? config.Description : $"사용 시 체력을 {HealAmount}회복합니다.";
        HealAmount = config != null ? config.Effect : 50;
    }

    public void Use(Player player)
    {
        player.HealHp(HealAmount);
    }

    
};

// 여기서 사용, 사용종료 구현각각해서 아이ㅏ템매니저에서 Update로 계산
// 턴 개념이 있는 아이템은 이 클래스를 상속
public abstract class DurationItem : Item, IUseableItem
{
    protected int _duration = 3;
    public int Duration { get { return _duration; } set { _duration = value;OnUpdateItem?.Invoke(); } }
    public virtual void Use(Player player) {  }
    public virtual void EndEffect(Player player) 
    {  OnEndItemUI?.Invoke(); Debug.Log($"{Name}아이템의 버프지속시간이 종료되었습니다."); OnEndItem?.Invoke(); }

    public event Action OnUpdateItem;
    public event Action OnEndItem;
    public event Action OnEndItemUI;
}

public class AttackPotion : DurationItem
{
    public int BonusDamage { get; }
    public AttackPotion()
    {
        var config = ItemManager.Instance.GetItemConfig("AttackPotion");
        Key = "AttackPotion";
        Type = config != null ? Enum.Parse<ItemType>(config.ItemType) : ItemType.AttackPotion; // ✅ JSON에서 `ItemType` 불러오기
        Name = config != null ? config.Name : "공격력 증가 포션";
        Description = config != null ? config.Description : $"사용 시 {3}턴 동안 공격력이 {BonusDamage}만큼 증가합니다.";
        BonusDamage = config != null ? config.Effect : 10;
        Duration = config != null ? config.Duration : _duration;
    }
    public override void Use(Player player)
    {
        base.Use(player);
        // 세턴동안 진행
        player.AttackPower += BonusDamage;
    }
    public override void EndEffect(Player player) 
    {
        //if (GameManager.Instance.Player == null) return;
        GameManager.Instance.Player.AttackPower -= BonusDamage;
        base.EndEffect(player);
    }
};

public class ShieldPotion : DurationItem
{
    public float BonusShield { get; }
    public ShieldPotion()
    {
        var config = ItemManager.Instance.GetItemConfig("ShieldPotion");
        Key = "ShieldPotion";
        Type = config != null ? Enum.Parse<ItemType>(config.ItemType) : ItemType.ShieldPotion; // ✅ JSON에서 `ItemType` 불러오기
        Name = config != null ? config.Name : "방어력증가포션";
        Description = config != null ? config.Description : "3턴 동안 받는 피해가 50% 감소합니다.";
        BonusShield = config != null ? config.Effect/10 : 0.5f;
        Duration = config != null ? config.Duration : _duration;

    }

    public override void Use(Player player)
    {
        base.Use(player);
        // 3턴 동안 피해감소
        player.DefenseRate -= BonusShield;
    }
    public override void EndEffect(Player player) 
    {
        GameManager.Instance.Player.DefenseRate += BonusShield;
        base.EndEffect(player);
    }
};

public class RandomPotion : Item, IUseableItem
{
    private List<string> PositiveEffects;
    private List<string> NegativeEffects;

    public RandomPotion()
    {
        var config = ItemManager.Instance.GetItemConfig("RandomPotion");
        Key = "RandomPotion";
        Type = config != null ? Enum.Parse<ItemType>(config.ItemType) : ItemType.RandomPotion; // ✅ JSON에서 `ItemType` 불러오기
        Name = config != null ? config.Name : "랜덤 포션";
        Description = config != null ? config.Description : "랜덤으로 긍정적 또는 부정적 효과를 발생시킴.";
        PositiveEffects = config != null ? config.PositiveEffects : new List<string> { "HpFullRecovery", "DoubleAttackPower" };
        NegativeEffects = config != null ? config.NegativeEffects : new List<string> { "HalfHp", "DoubleDamageTaken" };
    }

    public void Use(Player player)
    {
        bool isPositive = UnityEngine.Random.Range(0, 2) == 0; // ✅ 50% 확률로 긍정 or 부정 효과 선택
        string selectedEffect = isPositive
            ? PositiveEffects[UnityEngine.Random.Range(0,PositiveEffects.Count)]
            : NegativeEffects[UnityEngine.Random.Range(0, NegativeEffects.Count)];

        ApplyEffect(player, selectedEffect);
    }
    private void ApplyEffect(Player player, string effect)
    {
        switch (effect)
        {
            case "HpFullRecovery":
                player.Hp = player.MaxHp;
                Debug.Log($"{player.Name}의 체력이 최대치로 회복되었습니다!");
                break;

            case "DoubleAttackPower":
                player.AttackPower *= 2;
                Debug.Log($"{player.Name}의 공격력이 2배 증가했습니다!");
                break;

            case "HalfHp":
                player.Hp /= 2;
                Debug.Log($"{player.Name}의 체력이 절반으로 감소했습니다!");
                break;

            case "DoubleDamageTaken":
                player.DefenseRate *= 2;
                Debug.Log($"{player.Name}이 받는 피해가 2배 증가했습니다!");
                break;
        }
    }
}