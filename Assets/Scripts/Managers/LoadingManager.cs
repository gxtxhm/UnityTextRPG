using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    Slider LoadingSlider;
    TextMeshProUGUI LoadText;

    GameObject gameManagerPrefab;

    [SerializeField]
    float duration;

    bool IsLoading = false;

    private void Awake()
    {
        LoadingSlider = GameObject.Find("LoadingSlider").GetComponent<Slider>();
        LoadText = GameObject.Find("LoadText").GetComponent <TextMeshProUGUI>();
        LoadText.gameObject.SetActive(false);
        LoadingSlider.value = 0;

        gameManagerPrefab = Resources.Load<GameObject>("Prefabs/GameManager");
    }

    void Start()
    {
        StartCoroutine(LoadSlider());
    }

    public void StartGame()
    {
        if (IsLoading)
        {
            StartCoroutine(FadeInOutCo(LoadingSlider.transform.parent.gameObject, 0.5f, 1, 0, false,
                () => { SceneManager.LoadScene("StartScene"); }));
        }
    }

    IEnumerator LoadSlider()
    {
        float elapsedTime = 0;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            LoadingSlider.value = Mathf.Lerp(0,1,elapsedTime/duration);
            yield return null;
        }
        IsLoading = true;

        LoadText.gameObject.SetActive(true);
        StartCoroutine(actionText(0,1));
    }

    IEnumerator actionText(float startValue,float targetValue)
    {
        float elapsedTime = 0;
        float duration = 1.5f;

        while( elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Color c = LoadText.color;
            c.a = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            LoadText.color = c;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);

        if(startValue == 0)
            StartCoroutine (actionText(1,0));
        else
            StartCoroutine (actionText(0,1));
    }

    public IEnumerator FadeInOutCo(GameObject panel, float duration,int startValue,int targetValue,bool b,
        Action action = null)
    {
        CanvasGroup i = panel.GetComponent<CanvasGroup>();
        if (i == null)
        {
            Debug.LogError("Null Error received Object In FadeInOutCo, LoadingManager");
            yield break;
        }
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            i.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }
        panel.SetActive(b);

        action?.Invoke();
    }
}
