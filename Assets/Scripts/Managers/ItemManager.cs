using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable

public enum ItemType
{
    HpPotion,
    AttackPotion,
    ShieldPotion,
    RandomPotion
}

// TODO : 아이템에 아이디 부여해서 관리하는 방식이 더 나을 것 같음.
// ItemDataTableManager 같은것도 관리해서 하드코딩대신 사용하면 좋을듯.
public class ItemManager
{
    public static ItemManager Instance { get; private set; }= new ItemManager();

    // 아이템이름, 개수
    public Dictionary<string,List<Item>> Inventory { get; set; }

    public Dictionary<string, DurationItem> DurationItems { get; set; }

    // 나중에 이거활용해서 개선할 수 있으면 하기. 인벤토리를 list가 아닌 개수로하거나
    Dictionary<ItemType, Type> itemMap = new Dictionary<ItemType, Type>();

    private Dictionary<string, ItemData>? itemConfigs;

    public event Action<Item>? OnUsedItem;
    public event Action<DurationItem>? OnUsedDurationItem;
    public event Action<string>? OnDuplecatedUseItem;

    ItemManager() 
    {
        //LoadItemsFromJson();
        Inventory = new Dictionary<string,List<Item>>();
        DurationItems = new Dictionary<string, DurationItem>();
        itemMap.Add(ItemType.HpPotion,typeof(HpPotion));
        itemMap.Add(ItemType.AttackPotion, typeof(AttackPotion));
        itemMap.Add(ItemType.ShieldPotion, typeof(ShieldPotion));
        itemMap.Add(ItemType.RandomPotion, typeof(RandomPotion));
    }

    // Test 용 함수
    public int GetItemCount()
    {
        int count = 0;
        foreach(var item in Inventory)
        {
            count+=item.Value.Count;
        }
        return count;
    }

    public void UpdateItemManager()
    {
        if (DurationItems == null)
        {
            Debug.Log("durationItems Null!");
            return;
        }
        if (DurationItems.Count == 0) return;
        List<DurationItem> RemoveItems = new List<DurationItem>();
        foreach (var item in DurationItems)
        {
            item.Value.Duration--;
            if (item.Value.Duration <= 0 )
            {
                RemoveItems.Add(item.Value);
                item.Value.OnEndItem+= () => { DurationItems.Remove(item.Key); };
            }
        }
        foreach (var item in RemoveItems)
        {
            item.EndEffect(GameManager.Instance.Player);
        }
    }
#region 인벤토리 관련
    public void AddItem(Item item)
    {
        if(Inventory.ContainsKey(item.Key)==false)
        {
            Inventory[item.Key] = new List<Item>();
        }
        Inventory[item.Key].Add(item);
                
    }

    // TODO : 이것에 대한 델리게이트로 바꿔도 될듯 -> uimanager에 인벤토리 패널 업데이트를 
    public void RemoveItem(string itemKey)
    {
        Item? it;
        if(Inventory.ContainsKey(itemKey) )
        {
            it = Inventory[itemKey][0];
            Inventory[itemKey].RemoveAt(0);
            if (Inventory[itemKey].Count()==0)
                Inventory.Remove(itemKey);
        }
        else
        {
            Debug.Log($"Error! 없는 아이템 사용!{itemKey}");
        }
    }

    public void LogError(Exception ex)
    {
        string logMessage = $"[{DateTime.Now}] 예외 발생: {ex.Message}\n{ex.StackTrace}\n";
        File.AppendAllText("error_log3.txt", logMessage);

        //Debug.Log($"error Text : {ex} 오류가 발생했습니다. 로그 파일을 확인하세요.");
    }

    public void UsedItem(string itemKey, Player player)
    {
        try
        {
            if (Inventory.ContainsKey(itemKey) == false)
            {
                throw new Exception($"{itemKey}이 없습니다.");
            }

            Item it = Inventory[itemKey][0];
            IUseableItem? useableItem = it as IUseableItem;
            if (useableItem == null)
            {
                Debug.Log("소비아이템이 아닙니다!"); return;
            }

            useableItem.Use(player);

            //OnUsedItem?.Invoke(it);
            // -> Invoke 로 한번에 실행하니까 다른 함수에서 오류나는걸 여기서 catch 해서 
            // 아이템 관련 오류인줄 알고 한참해멤. 그래서 각 함수 실행별로 try - catch 적용
            if (OnUsedItem != null)
            {
                foreach (var handler in OnUsedItem.GetInvocationList())
                {
                    try
                    {
                        handler.DynamicInvoke(it);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"OnUsedItem 이벤트 핸들러 실행 중 오류 발생: {ex.Message}");
                    }
                }
            }


            RemoveItem(itemKey);
            if (it is DurationItem)
                RegistItem(it);
        }
        catch (Exception e)
        {
            LogError(e);
            Debug.Log($"{itemKey}아이템을 사용할 수 없습니다. 존재하는 아이템을 선택하세요.");
        }
        
    }
        
    public void PrintInventory()
    {
        Debug.Log("------인벤토리------");
        foreach (var item in Inventory)
        {
            Debug.Log($"{item.Key} : {item.Value.Count}개");
        }
        UIManager.Instance.OpenInventory();
    }
        
    public List<T> FindItemByType<T>(ItemType type) where T : Item
    {
        if (Inventory.ContainsKey(type.ToString()) == false)
            return new List<T>();

        return Inventory[type.ToString()].OfType<T>().ToList();
    }
    #endregion

    #region 사용버프아이템 관련
    public void RegistItem(Item item)
    {
        DurationItem? di = item as DurationItem;
        if (di == null)
        {
            Debug.Log("버프아이템 등록오류 : RegistItem");
            return;
        }
        if (DurationItems.ContainsKey(di.ToString()))
        {
            OnDuplecatedUseItem?.Invoke(di.Duration.ToString());
        }
        else
        {
            OnUsedDurationItem?.Invoke(di);
        }
        DurationItems[di.Key] = di;
    }

    public void PrintDurationItemList()
    {
        foreach(var item in DurationItems)
        {
            Debug.Log($"{item.Key} : {item.Value.Duration}턴 남음");
        }
    }
    #endregion
        
    public Item RandomCreateItem()
    {
        ItemType randomType = (ItemType)UnityEngine.Random.Range(0,Enum.GetValues(typeof(ItemType)).Length);
        Item item = (Item)Activator.CreateInstance(itemMap[randomType])!;

        AddItem(item);

        return item;
    }

    public void LoadItemsInfoFromJson(ItemInfo itemInfo)
    {
        Debug.Log("LoadItemsFromJson");
        //string json = File.ReadAllText("items.json"); // ✅ JSON 파일 읽기
        itemConfigs = itemInfo.data;
    }

    // 인벤토리 아이템이름받고 개수만큼 생성하기
    public void LoadInventoryData(Dictionary<string, int> datas)
    {
        foreach (var data in datas)
        {
            ItemType type;
            bool b = Enum.TryParse(data.Key, out type);
            if(b==false)
            {
                Debug.LogError($"{data.Key} Load Error");break;
            }
            Item item = (Item)Activator.CreateInstance(itemMap[type]);

            for(int i=0;i<data.Value;i++)AddItem(item);
        }
    }

    public ItemData? GetItemConfig(string itemType)
    {
        return itemConfigs != null && itemConfigs.ContainsKey(itemType) ? itemConfigs[itemType] : null;
    }
}

