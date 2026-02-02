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
    [SerializeField] GameObject congratzUI;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text turnText;
    [SerializeField] TMP_Text selectedLevelText;
    [SerializeField] ScrollRect scrollRect;

    string cardsAddressableLabel = "Cards";

    int currentLevelCards = 1;
    int currentlevel;
    int currentScore = 0;
    int currentTurn = 0;

    private Queue<Card> selectedCards = new Queue<Card>();

    public async UniTaskVoid Start()
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
            return;

        Card firstCard = selectedCards.Dequeue();
        Card secondCard = selectedCards.Dequeue();

        UpdateTurn();

        if (IsMatch(firstCard, secondCard))
        {
            await HandleCorrectMatch(firstCard, secondCard);
        }
        else
        {
            await HandleWrongMatch(firstCard, secondCard);
        }
    }

    private void UpdateTurn()
    {
        currentTurn++;
        turnText.text = currentTurn.ToString();
    }

    private bool IsMatch(Card a, Card b)
    {
        return a.GetCardData().CardId == b.GetCardData().CardId;
    }

    private async UniTask HandleCorrectMatch(Card firstCard, Card secondCard)
    {
        await UniTask.Delay(1000);
        firstCard.ClearCard();
        secondCard.ClearCard();

        currentScore++;
        scoreText.text = currentScore.ToString();
        Services.Get<AudioService>().PlayCorrect();

        if (currentScore == currentlevel)
        {
            await HandleLevelComplete();
        }
    }

    private async UniTask HandleWrongMatch(Card firstCard, Card secondCard)
    {
        await UniTask.Delay(1000);
        firstCard.FlipBackCardAsync().Forget();
        secondCard.FlipBackCardAsync().Forget();
        Services.Get<AudioService>().PlayWrong();
    }

    private async UniTask HandleLevelComplete()
    {
        congratzUI.SetActive(true);
        Services.Get<AudioService>().PlayLevelComplete();
        await UniTask.Delay(2000);

        ScoreData data = await Services.Get<SaveService>().LoadScoreDataAsync();
        SaveLevelData(data);

        await Services.Get<SaveService>().SaveScoreDataAsync(data);

        Services.Get<EventService>().LevelComplete?.Invoke();
        mainMenu.SetActive(true);
        gamePlayUI.SetActive(false);
    }

    private void SaveLevelData(ScoreData data)
    {
        int index = data.GamePlayDatas.FindIndex(d => d.Level == currentlevel);

        GamePlayData newData = new GamePlayData
        {
            Level = currentlevel,
            Score = currentScore,
            Turn = currentTurn
        };

        if (index >= 0)
            data.GamePlayDatas[index] = newData;
        else
            data.GamePlayDatas.Add(newData);
    }

    public void hitButtonClick()
    {
        Services.Get<EventService>().HintRequested?.Invoke();
    }

    public async UniTask InitializeGameAsync()
    {
        await PrepareLevelsAndCards();
        Services.Get<LoadingService>().HideLoadingScreen();
    }

    private async UniTask PrepareLevelsAndCards()
    {
        var token = this.GetCancellationTokenOnDestroy();

        Services.Get<LoadingService>().ShowLoadingScreen();
        Services.Get<LoadingService>().UpdateLoadingProgress("Loading Levels...");

        await LoadCardPackAsync(token);

        Services.Get<LoadingService>().UpdateLoadingProgress("Spawning Levels...");

        for (int i = 1; i <= totalLevles; i++)
        {
            await CreateLevelButton(i, token);
        }

        Services.Get<LoadingService>().UpdateLoadingProgress("Finalizing...");
        scrollRect.verticalScrollbar.value = 1;
    }

    private async UniTask CreateLevelButton(int levelIndex, CancellationToken token)
    {
        var levelObj = await Addressables.InstantiateAsync(
            levelPrefabName,
            levelContainer.transform
        ).ToUniTask(cancellationToken: token);

        Button levelButton = levelObj.GetComponent<Button>();
        levelButton.GetComponentInChildren<TMP_Text>().text = levelIndex.ToString();

        levelButton.onClick.AddListener(() => OnLevelSelected(levelIndex));

        Services.Get<LoadingService>().UpdateLoadingProgress(
            $"Spawning Levels... {(levelIndex / (float)totalLevles) * 100f:0}%"
        );

        scrollRect.verticalScrollbar.value = 0;
        await UniTask.Yield();
    }

    private void OnLevelSelected(int levelIndex)
    {
        currentLevelCards = levelIndex * 2;
        cardSpawner.SetCurrentLevel(currentLevelCards);
        currentlevel = levelIndex;
        selectedLevelText.text = levelIndex.ToString();
        Services.Get<AudioService>().ButtonClick();
    }

    private async UniTask LoadCardPackAsync(CancellationToken token)
    {
        var result = await Addressables.LoadAssetsAsync<CardData>(
            cardsAddressableLabel,
            null
        ).ToUniTask(cancellationToken: token);

        cardSpawner.LoadAllCards(new List<CardData>(result));
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        congratzUI.SetActive(false);
        gamePlayUI.SetActive(true);

        cardSpawner.StartCardSpawning();

        ResetScoreAndTurn();
    }

    private void ResetScoreAndTurn()
    {
        currentScore = 0;
        currentTurn = 0;
        scoreText.text = currentScore.ToString();
        turnText.text = currentTurn.ToString();
    }

    public void GotoHome()
    {
        Services.Get<EventService>().LevelComplete?.Invoke();
        mainMenu.SetActive(true);
        gamePlayUI.SetActive(false);
        congratzUI.SetActive(false);
    }

    public void GameQuit()
    {
        Services.Unregister<AudioService>();
        Services.Unregister<LoadingService>();
        Services.Unregister<EventService>();
        Services.Unregister<SaveService>();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
