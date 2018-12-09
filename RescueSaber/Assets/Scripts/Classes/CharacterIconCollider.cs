using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterIconCollider : MonoBehaviour, IPointerClickHandler {
    [Header("References")]
    public CharacterIcon characterIcon;

    public void OnPointerClick(PointerEventData eventData) { //Onpointerclick is inherited from pointerclickhandler
        characterIcon.Clicked();
    }
}