using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CardSpawner : MonoBehaviour
{
    string cardPrefabAddressbleName = "Card";
    string cardsAddressableLabel = "Cards";
    [SerializeField] TMP_Text loadingText;

    private List<CardData> loadedCards = new();
    private CancellationToken token;

    private async UniTask CardSpawing()
    {
        loadingText.gameObject.SetActive(true);
        loadingText.text = "Loading 0%";

        await LoadCardPackAsync();
        await SpawnCardsAsync(496);

        loadingText.text = "Loading Complete";
        await UniTask.Delay(300, cancellationToken: token);
        loadingText.gameObject.SetActive(false);
    }

    public void StartCardSpawning( )
    {
        token = this.GetCancellationTokenOnDestroy();
        CardSpawing().Forget();
    }
    private async UniTask LoadCardPackAsync()
    {
        var result = await Addressables.LoadAssetsAsync<CardData>(cardsAddressableLabel, null).ToUniTask(cancellationToken: token);
        loadedCards = new List<CardData>(result);
    }


    private async UniTask<List<CardData>> ShuffleCards(List<CardData> cards)
    {
        List<CardData> shuffled = new List<CardData>(cards);

        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            token.ThrowIfCancellationRequested();
            int j = Random.Range(0, i + 1);
            CardData temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
            if (i % 1 == 0)
                await UniTask.Yield();
        }
        return shuffled;
    }

    private async UniTask SpawnCardsAsync(int totalCards)
    {
        int pairCount = totalCards / 2;

        loadingText.text = "Loading 10%";
        List<CardData> shuffledPool = await ShuffleCards(loadedCards);
        List<CardData> spawnList = new List<CardData>(totalCards);

        for (int i = 0; i < pairCount; i++)
        {
            spawnList.Add(shuffledPool[i]);
            spawnList.Add(shuffledPool[i]);
        }
        loadingText.text = "Loading 40%";
        spawnList = await ShuffleCards(spawnList);
        for (int i = 0; i < spawnList.Count; i++)
        {
            token.ThrowIfCancellationRequested();

            float progress = (i + 1f) / spawnList.Count * 100f;
            loadingText.text = $"Loading {progress:0}%";

            var handle = Addressables.InstantiateAsync(cardPrefabAddressbleName, transform);
            GameObject cardObj = await handle.ToUniTask(cancellationToken: token);

            cardObj.GetComponent<Card>().SetCardData(spawnList[i]);

            await UniTask.Yield();
        }
    }

}



