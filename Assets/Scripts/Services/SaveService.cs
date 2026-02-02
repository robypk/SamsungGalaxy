using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveService : MonoBehaviour
{

    private string SavePath => Path.Combine(Application.persistentDataPath, "scoreData.json");
    private void Awake()
    {
        Services.Register<SaveService>(this);
    }
    public async UniTask SaveScoreDataAsync(ScoreData data)
    {
        string json = JsonUtility.ToJson(data, true);
        await UniTask.RunOnThreadPool(() =>
        {
            File.WriteAllText(SavePath, json);
        });
    }

    public async UniTask<ScoreData> LoadScoreDataAsync()
    {
        if (!File.Exists(SavePath))
        {
            return new ScoreData();
        }
        string json = await UniTask.RunOnThreadPool(() =>
        {
            return File.ReadAllText(SavePath);
        });
        return JsonUtility.FromJson<ScoreData>(json);
    }

}

public class ScoreData
{
    public List<GamePlayData> GamePlayDatas = new List<GamePlayData>();
}

public struct GamePlayData
{
    public int Level;
    public int Score;
    public int Turn;
}
