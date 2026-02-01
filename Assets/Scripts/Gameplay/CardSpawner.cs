using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] string cardPrefabAddressbleName;
    [SerializeField] string cardsAddressableLabel;
    [SerializeField] TMP_Text loadingText;

    private List<CardData> loadedCards = new();

    private async UniTask CardSpawing(CancellationToken token)
    {
        loadingText.gameObject.SetActive(true);
        loadingText.text = "Loading 0%";

        await LoadCardPackAsync(token);
        await SpawnCardsAsync(token);

        loadingText.text = "Loading Complete";
        await UniTask.Delay(300, cancellationToken: token);
        loadingText.gameObject.SetActive(false);
    }

    public void StartCardSpawning()
    {
        var token = this.GetCancellationTokenOnDestroy();
        CardSpawing(token).Forget();
    }



    private async UniTask LoadCardPackAsync(CancellationToken token)
    {
        var result = await Addressables
            .LoadAssetsAsync<CardData>(cardsAddressableLabel, null)
            .ToUniTask(cancellationToken: token);

        loadedCards = new List<CardData>(result);
    }

    private async UniTask SpawnCardsAsync( CancellationToken token)
    {
        int totalCards = 10000;

        for (int i = 0; i < totalCards; i++)
        {
             token.ThrowIfCancellationRequested();
            var cardData = loadedCards[Random.Range(0, loadedCards.Count)];

            var handle = Addressables.InstantiateAsync(cardPrefabAddressbleName, transform);
            GameObject cardObj = await handle.ToUniTask( cancellationToken: token);

            cardObj.GetComponent<Card>().SetCardData(cardData);

            // Update loading %
            float progress = (i + 1) / (float)totalCards * 100f;
            loadingText.text = $"Loading {progress:0}%";

            if(i % 1 == 0)
            {
               await UniTask.Yield();
            }
        }
    }
}
