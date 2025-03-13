using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PanelEvent : MonoBehaviour , IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("LoadingManager").GetComponent<LoadingManager>().StartGame();
    }
    
}
