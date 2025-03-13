using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public interface IInfoProvider
{
    string TranslateInfoToString();
}

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    IInfoProvider _infoProvider=null;
    [SerializeField]
    Vector2 HoverPos;
    TextMeshProUGUI text;
    GameObject panel;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        GameObject panelPrefab = Resources.Load<GameObject>("Prefabs/HoverInfoPanel");
        panel = Instantiate(panelPrefab);
        panel.transform.SetParent(transform, false);
        // 위치를 우선 에디터에서 설정하도록함
        panel.GetComponent<RectTransform>().anchoredPosition = new Vector3(HoverPos.x, HoverPos.y, 0);
        text = panel.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetPanel(IInfoProvider info,bool ForcedAssigned=false)
    {
        if(_infoProvider ==null) _infoProvider = info;
        if (ForcedAssigned) _infoProvider = info;
        text.text = info.TranslateInfoToString();

        

        panel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_infoProvider == null )
        {
            Debug.LogError("Error OnPointerEnter In HoverText : _infoProvider is Null!");return;
        }
        SetPanel(_infoProvider);
        panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panel.SetActive(false);
        // Revert the text color or size when not hovered
        //text.color = Color.white; // Example: revert to white
    }
}
