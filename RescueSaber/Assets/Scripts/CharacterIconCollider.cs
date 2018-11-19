using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterIconCollider : MonoBehaviour, IPointerClickHandler {
    public CharacterIcon characterIcon;

    public void OnPointerClick(PointerEventData eventData)
    {
        characterIcon.Clicked();
    }
}