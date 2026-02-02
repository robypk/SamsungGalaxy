using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingService : MonoBehaviour
{
    [SerializeField] private GameObject loadingIcon;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private GameObject loadingScreenGO;
    private void Awake()
    {
        Services.Register<LoadingService>(this);
    }


    public void ShowLoadingScreen()
    {
        loadingScreenGO.SetActive(true);
        LeanTween.rotateAroundLocal(loadingIcon, Vector3.forward, -360f, 1f).setLoopClamp();
    }

    public void UpdateLoadingProgress(string progress)
    {

        loadingText.text = progress;
    }

    public void HideLoadingScreen()
    {
        loadingScreenGO.SetActive(false);
        LeanTween.cancel(loadingIcon);
    }

}
