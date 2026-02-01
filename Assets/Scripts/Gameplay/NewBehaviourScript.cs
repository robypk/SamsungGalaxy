using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{


    [SerializeField] List<CardData> cards;
    [SerializeField] GameObject cardPrefab;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, transform);
            Card card = cardObj.GetComponent<Card>();
            card.SetCardData(cards[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
