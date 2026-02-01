using UnityEngine;

[CreateAssetMenu(
    fileName = "CardData",
    menuName = "SamsungGalaxy/Card Data"
)]
public class CardData : ScriptableObject
{
    [SerializeField] private int cardId;
    [SerializeField] private Sprite cardImage;
    public int CardId => cardId;
    public Sprite CardImage => cardImage;
}
