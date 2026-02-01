using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] string cardPrefabAddressbleName;
    [SerializeField] string cardsAddressableGroupName;

    private List<CardData> loadedCards = new();

    private async UniTaskVoid Start()
    {
        await LoadCardPackAsync();
        await SpawnCardsAsync();
    }

    private async UniTask LoadCardPackAsync()
    {
        var result = await Addressables.LoadAssetsAsync<CardData>(cardsAddressableGroupName, null).ToUniTask();
        loadedCards = new List<CardData>(result);
    }

    private async UniTask SpawnCardsAsync()
    {
        foreach (var cardData in loadedCards)
        {
            var handle = Addressables.InstantiateAsync(cardPrefabAddressbleName, transform);
            GameObject cardObj = await handle.ToUniTask();
            cardObj.GetComponent<Card>().SetCardData(cardData);
        }
    }

}
