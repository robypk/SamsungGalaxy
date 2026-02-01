using UnityEngine;

[CreateAssetMenu(
    fileName = "CardData",
    menuName = "SamsungGalaxy/Card Data"
)]
public class CardData : ScriptableObject
{
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage;
    public string CardName => cardName;
    public Sprite CardImage => cardImage;
}
