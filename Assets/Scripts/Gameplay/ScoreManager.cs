using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Transform scoreCardContainer;
    [SerializeField] private string scoreCardPrefabName = "ScoreCard";

    private readonly List<GameObject> spawnedCards = new();
    private CancellationTokenSource cts;

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        LoadAndDisplayScores(cts.Token).Forget();
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
        ClearScoreCards();
    }

    private async UniTask LoadAndDisplayScores(CancellationToken token)
    {
        ScoreData scoreData = await Services.Get<SaveService>().LoadScoreDataAsync().AttachExternalCancellation(token);
        if (scoreData?.GamePlayDatas == null || scoreData.GamePlayDatas.Count == 0)
            return;
        foreach (GamePlayData data in scoreData.GamePlayDatas)
        {
            token.ThrowIfCancellationRequested();
            GameObject card = await Addressables.InstantiateAsync(scoreCardPrefabName, scoreCardContainer).ToUniTask(cancellationToken: token);
            spawnedCards.Add(card);
            if (card.TryGetComponent(out ScoreCard scoreCard))
            {
                scoreCard.SetScoreCard(data.Level, data.Score, data.Turn);
            }
        }
    }

    private void ClearScoreCards()
    {
        foreach (var card in spawnedCards)
        {
            Addressables.ReleaseInstance(card);
        }
        spawnedCards.Clear();
    }
}
