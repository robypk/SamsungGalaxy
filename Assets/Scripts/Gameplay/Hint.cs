using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] private Button hintButton;
    [SerializeField] private TMP_Text hintTimerText;

    private void OnEnable()
    {
        Services.Get<EventService>().HintRequested += OnHintRequested;
    }

    private void OnDisable()
    {
        Services.Get<EventService>().HintRequested -= OnHintRequested;
    }

    private void OnHintRequested()
    {
        hintButton.interactable = false;
        StartHintTimer(4).Forget();
    }

    private async UniTask StartHintTimer(float seconds)
    {
        float remainingTime = seconds;

        while (remainingTime > 0)
        {
            hintTimerText.text = Mathf.CeilToInt(remainingTime).ToString();
            await UniTask.Delay(1000);
            remainingTime--;
        }

        hintTimerText.text = "Hint";
        hintButton.interactable = true;
    }
}
