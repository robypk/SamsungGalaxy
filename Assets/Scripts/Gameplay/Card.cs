using Cysharp.Threading.Tasks;
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

    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClicked);
    }

    public void SetCardData(CardData data)
    {
        cardData = data;
        Init();
    }
    private void Init()
    {
        cardImage.sprite = cardBackImage;
        transform.localScale = Vector3.one;
    }

    private void OnCardClicked()
    {
        ShowCardAsync().Forget();
    }

    public async UniTask ShowCardAsync()
    {
        cardButton.interactable = false;
        await LeanTweenScaleXAsync(0f, 0.15f);
        cardImage.sprite = cardData.CardImage;
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
