using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MemoryGameManager : MonoBehaviour
{
    // == Variables ==

    // Fill in inspector from Assets > UI > Other > MemoryGame
    public List<Sprite> backgroundSprites = new List<Sprite>();
    public List<Sprite> foregroundSprites = new List<Sprite>();
    public List<Button> cards = new List<Button>();

    // Prevents duplicated cards
    List<Button> cardsToActivate = new List<Button>();
    List<MemoryCard> activatedCards = new List<MemoryCard>(); 

    public bool cardPicked;
    public MemoryCard pickedCard;
    public Sprite chosenBackgroundSprite;
    public bool canActivate = true;

    // Fill in inspector from Assets > Materials > Puzzels > PuzzelsThimo
    public Material solvedMaterial;

    public UnityEvent eventToTrigger;

    // Setup game.
    void Start()
    {
        GenerateCards();
    }

    // Set card background sprites & setup cards (ID & sprites).
    void GenerateCards()
    {
        int randomCardBackground = Random.Range(0, backgroundSprites.Count);
        int randomCard = Random.Range(0, cardsToActivate.Count);

        int CardData = 0;
        int loop = 0;
        chosenBackgroundSprite = backgroundSprites[randomCardBackground];


        for (int i = 0; i < cards.Count; i++)
        {
            cardsToActivate.Add(cards[i]);
        }

        foreach (Button card in cards)
        {
            // Set card background.
            card.image.sprite = backgroundSprites[randomCardBackground];

            // Set card ID & sprite.
            cardsToActivate[randomCard].gameObject.GetComponent<MemoryCard>().ID = CardData;
            cardsToActivate[randomCard].gameObject.GetComponent<MemoryCard>().foregroundSprite = foregroundSprites[CardData];

            cardsToActivate.Remove(cardsToActivate[randomCard]);
            randomCard = Random.Range(0, cardsToActivate.Count);

            loop++;
            if (loop > 1)
            {
                CardData++;
                loop = 0;
            }
        }
    }

    public void CheckCards(MemoryCard matchCard)
    {
        canActivate = false;

        // Check if selected cards match
        if (matchCard.ID == pickedCard.ID)
        {
            matchCard.isActive = false;
            activatedCards.Add(matchCard);
            activatedCards.Add(pickedCard);
            cardPicked = false;
            canActivate = true;
        }
        else
        {
            StartCoroutine(matchCard.ResetCard());
            StartCoroutine(pickedCard.ResetCard());
            cardPicked = false;
        }
        
        // Solve puzzle
        if (activatedCards.Count == cards.Count)
        {
            transform.parent.GetComponent<Renderer>().material = solvedMaterial;
            eventToTrigger.Invoke();
        }
    }
}
