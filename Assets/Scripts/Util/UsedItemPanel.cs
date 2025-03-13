using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UsedItemPanel : MonoBehaviour
{
    [SerializeField]
    Vector2 StartPanelPos;

    [SerializeField]
    Vector2 EndPanelPos;

    GameObject PanelPrefab;

    GameObject DurationPanel;
    GameObject durationPrefab;

    List<GameObject> durationItems = new List<GameObject>();

    private void Awake()
    {
        PanelPrefab = Resources.Load<GameObject>("Prefabs/NotifyUsedItem");
        ItemManager.Instance.OnUsedItem += StartUseItemPanel;

        DurationPanel = GameObject.Find("Durations");
        durationPrefab = Resources.Load<GameObject>("Prefabs/DurationsItemUI");

        ItemManager.Instance.OnUsedDurationItem += RegistDurationUI;
    }

    public void RegistDurationUI(DurationItem item)
    {
        GameObject go = Instantiate(durationPrefab);
        go.transform.SetParent(DurationPanel.transform, false);


        go.GetComponent<Image>().sprite = UIManager.Instance.GetSpriteByItemKey(item.Key);
        go.GetComponentInChildren<TextMeshProUGUI>().text = item.Duration.ToString();

        go.GetComponent<HoverText>().SetPanel(item);
        durationItems.Add(go);
        item.OnUpdateItem += () =>
        {
            go.GetComponentInChildren<TextMeshProUGUI>().text = item.Duration.ToString();
        };
        item.OnEndItemUI += () =>
        {
            Debug.Log("Remove Duration Item");
            durationItems.Remove(go);
            Destroy(go);
        };
        ItemManager.Instance.OnDuplecatedUseItem += (string s) => go.GetComponentInChildren<TextMeshProUGUI>().text = s;
    }



    public void StartUseItemPanel(Item item)
    {
        GameObject go = Instantiate(PanelPrefab);
        go.transform.SetParent(gameObject.transform,false);
        go.GetComponent<RectTransform>().anchoredPosition = StartPanelPos;
        go.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.Name}아이템을 사용!";
        StartCoroutine(StartEffect(go));
    }
    
    IEnumerator StartEffect(GameObject item)
    {
        float elaspedTime = 0;
        float duration = 1f;
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        while(elaspedTime < duration)
        {
            elaspedTime += Time.deltaTime;
            rectTransform.anchoredPosition = new Vector2(StartPanelPos.x,
                Mathf.Lerp(StartPanelPos.y, EndPanelPos.y, elaspedTime / duration));
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        elaspedTime = 0;
        while (elaspedTime < duration)
        {
            elaspedTime += Time.deltaTime;
            rectTransform.anchoredPosition = new Vector2(StartPanelPos.x,
                Mathf.Lerp(EndPanelPos.y, StartPanelPos.y, elaspedTime / duration));
            yield return null;
        }

        Destroy(item);
    }

}
