using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveService : MonoBehaviour
{

    private string savePath;
    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "scoreData.json");
        print("Save Path: " + savePath);
        Services.Register<SaveService>(this);
    }
    public async UniTask SaveScoreDataAsync(ScoreData data)
    {
        string json = JsonUtility.ToJson(data, true);
        await UniTask.RunOnThreadPool(() =>
        {
            File.WriteAllText(savePath, json);
        });
    }

    public async UniTask<ScoreData> LoadScoreDataAsync()
    {
        if (!File.Exists(savePath))
        {
            return new ScoreData();
        }
        string json = await UniTask.RunOnThreadPool(() =>
        {
            return File.ReadAllText(savePath);
        });
        return JsonUtility.FromJson<ScoreData>(json);
    }

}

[Serializable]
public class ScoreData
{
    public List<GamePlayData> GamePlayDatas = new List<GamePlayData>();
}
[Serializable]
public struct GamePlayData
{
    public int Level;
    public int Score;
    public int Turn;
}
