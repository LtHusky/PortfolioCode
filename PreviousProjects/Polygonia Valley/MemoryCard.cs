using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    public int ID;
    public Sprite foregroundSprite;

    public bool isActive = true;

    MemoryGameManager mgm;

    void Start()
    {
        mgm = transform.parent.GetComponent<MemoryGameManager>();
    }

    public void ActivateCard()
    {
        if (isActive && mgm.canActivate)
        {
            gameObject.GetComponent<Button>().image.sprite = foregroundSprite;

            if (mgm.cardPicked == false)
            {
                mgm.pickedCard = this;
                mgm.cardPicked = true;
                isActive = false;
            }
            else
            {
                mgm.CheckCards(this);
            }
        }
    }

    public IEnumerator ResetCard()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Button>().image.sprite = mgm.chosenBackgroundSprite;
        isActive = true;
        mgm.canActivate = true;
    }
}
