using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{

    [SerializeField] GridLayoutGroup gridLayoutGroup;


    string cardPrefabAddressbleName = "Card";
    private List<CardData> loadedCards = new();
    private CancellationToken token;
    int CurrentLevelcards;



    public void LoadAllCards(List<CardData> cardDatas)
    {
        loadedCards = cardDatas;
    }

    public void SetCurrentLevel(int level)
    {
        CurrentLevelcards = level;
    }

    public void StartCardSpawning( )
    {
        token = this.GetCancellationTokenOnDestroy();
        CardSpawing().Forget();
    }

    private async UniTask CardSpawing()
    {
        Services.Get<LoadingService>().ShowLoadingScreen();
        await SpawnCardsAsync(CurrentLevelcards);
        Services.Get<LoadingService>().UpdateLoadingProgress("Finalizing...");
        Services.Get<LoadingService>().HideLoadingScreen();
        Services.Get<EventService>().HintRequested?.Invoke();
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
        Services.Get<LoadingService>().UpdateLoadingProgress("Preparing cards...");
        PrepareGrid();
        List<CardData> shuffledPool = await ShuffleCards(loadedCards);
        List<CardData> spawnList = new List<CardData>(totalCards);
        for (int i = 0; i < pairCount; i++)
        {
            spawnList.Add(shuffledPool[i]);
            spawnList.Add(shuffledPool[i]);
        }
        print($"Total Cards to spawn: {spawnList.Count}");
        Services.Get<LoadingService>().UpdateLoadingProgress("Spawning cards...");
        spawnList = await ShuffleCards(spawnList);
        for (int i = 0; i < spawnList.Count; i++)
        {

            token.ThrowIfCancellationRequested();
            float progress = (i + 1f) / spawnList.Count * 100f;
            Services.Get<LoadingService>().UpdateLoadingProgress($"Spawning cards... {progress:0}%");
            var cardObj = await Addressables.InstantiateAsync(cardPrefabAddressbleName, transform).ToUniTask(cancellationToken: token);
            cardObj.GetComponent<Card>().SetCardData(spawnList[i]);
            await UniTask.Yield();
        }
    }


    private void PrepareGrid()
    {
        int totalCards = CurrentLevelcards;
        int gridCount = Mathf.CeilToInt(Mathf.Sqrt(totalCards));
        float totalSpacing = (gridCount - 1) * 10f;
        float availableSize = 1000 - totalSpacing;
        float cellSize = availableSize / gridCount;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

}



