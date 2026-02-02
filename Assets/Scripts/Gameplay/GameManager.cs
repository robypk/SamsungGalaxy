using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject levelContainer;
    [SerializeField] CardSpawner cardSpawner;
    [SerializeField] string levelPrefabName = "LevelButton";
    [SerializeField] int totalLevles;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gamePlayUI;
    string cardsAddressableLabel = "Cards";
    int currentLevelCards = 1;

    private Queue<Card> selectedCards = new Queue<Card>();



    public async UniTaskVoid Start( )
    {
        await InitializeGameAsync();
        Services.Get<EventService>().CardClick += onCardClick;
    }

    private void onCardClick(Card card)
    {
        CheckMatching(card).Forget();

    }

    private async UniTask CheckMatching(Card card)
    {
        selectedCards.Enqueue(card);
        if (selectedCards.Count < 2)
        {
            return;
        }
        Card firstCard = selectedCards.Dequeue();
        Card secondCard = selectedCards.Dequeue();
        if (firstCard.GetCardData().CardId == secondCard.GetCardData().CardId)
        {
            await UniTask.Delay(1000);
            firstCard.gameObject.GetComponent<Image>().enabled = false;
            firstCard.gameObject.GetComponent<Button>().enabled = false;

            secondCard.gameObject.GetComponent<Image>().enabled = false;
            secondCard.gameObject.GetComponent<Button>().enabled = false;
        }
        else
        {
            await UniTask.Delay(1000);
            firstCard.FlipBackCardAsync().Forget();
            secondCard.FlipBackCardAsync().Forget();

        }
    }
    public void hitButtonClick() 
    {  
        Services.Get<EventService>().hitClick?.Invoke();
    }

    public async UniTask InitializeGameAsync( )
    {
        await PrepareLevelsAndCards();
        Services.Get<LoadingService>().HideLoadingScreen();
    }

    private async UniTask PrepareLevelsAndCards( )
    {
        var token = this.GetCancellationTokenOnDestroy();
        Services.Get<LoadingService>().ShowLoadingScreen();
        Services.Get<LoadingService>().UpdateLoadingProgress("Loading Levels...");
        await LoadCardPackAsync(token);
        Services.Get<LoadingService>().UpdateLoadingProgress("Spawning Levels...");
        for (int i = 1; i <= totalLevles; i++)
        {
            int levelIndex = i;
            var levelObj = await Addressables.InstantiateAsync(levelPrefabName, levelContainer.transform).ToUniTask(cancellationToken: token);
            Button levelButton = levelObj.GetComponent<Button>();
            levelButton.GetComponentInChildren<TMP_Text>().text = i.ToString();
            levelButton.onClick.AddListener(() =>
            {
                currentLevelCards = levelIndex *2;
                cardSpawner.SetCurrentLevel(currentLevelCards);
                print($"Level {levelIndex} selected.");
            });
            Services.Get<LoadingService>().UpdateLoadingProgress($"Spawning Levels... {(i / (float)totalLevles) * 100f:0}%");
            await UniTask.Yield();
        }
        Services.Get<LoadingService>().UpdateLoadingProgress("Finalizing...");
    }
    private async UniTask LoadCardPackAsync(CancellationToken token)
    {
        var result = await Addressables.LoadAssetsAsync<CardData>(cardsAddressableLabel, null).ToUniTask(cancellationToken: token);
        cardSpawner.LoadAllCards( new List<CardData>(result));
    }


    public void StartGame( )
    {
        mainMenu.SetActive(false);
        gamePlayUI.SetActive(true);
        cardSpawner.StartCardSpawning();
    }

}
