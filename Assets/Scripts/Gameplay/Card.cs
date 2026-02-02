using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    private Sprite cardBackImage;
    private CardData cardData;
    private Button cardButton;

    private void Awake()
    {
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardClicked);
        cardBackImage = cardImage.sprite;
    }

    private void OnEnable()
    {
        Services.Get<EventService>().HintRequested += hitClicked;
        Services.Get<EventService>().LevelComplete += DestroyCard;
    }

    private void DestroyCard()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        Services.Get<EventService>().HintRequested -= hitClicked;
        Services.Get<EventService>().LevelComplete -= DestroyCard;
    }
    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClicked);
    }

    public void ClearCard()
    {
        cardImage.enabled = false;
        cardButton.enabled = false;
    }

    public void SetCardData(CardData data)
    {
        cardData = data;
        Init();
    }

    public CardData GetCardData()
    {
        return cardData;
    }
    private void Init()
    {
        cardImage.sprite = cardBackImage;
        transform.localScale = Vector3.one;
    }

    private void OnCardClicked()
    {
        ShowCardAsync().Forget();
        Services.Get<EventService>().CardClick?.Invoke(this);
    }

    private void hitClicked()
    {
       hintRequired().Forget();
    }

    private async UniTask hintRequired()
    {
        await ShowCardAsync();
        await UniTask.Delay(4000);
        await FlipBackCardAsync();
    }



    public async UniTask ShowCardAsync()
    {
        cardButton.interactable = false;
        await LeanTweenScaleXAsync(0f, 0.15f);
        cardImage.sprite = cardData.CardImage;
        await LeanTweenScaleXAsync(1f, 0.15f);
    }

    public async UniTask FlipBackCardAsync()
    {
        await LeanTweenScaleXAsync(0f, 0.15f);
        cardImage.sprite = cardBackImage;
        await LeanTweenScaleXAsync(1f, 0.15f);
        cardButton.interactable = true;
    }

    public void HideCard()
    {
        cardImage.sprite = cardBackImage;
        transform.localScale = Vector3.one;
    }

    private UniTask LeanTweenScaleXAsync(float target, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        LeanTween.scaleX(gameObject, target, duration).setEaseInOutQuad().setOnComplete(() => tcs.TrySetResult());
        return tcs.Task;
    }
}
