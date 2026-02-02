using System;
using System.Collections.Generic;

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