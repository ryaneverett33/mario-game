using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUpdateSelectedHandler
{
    private bool isLeftButton;
    bool clicked = false;

    void Start() {
        isLeftButton = gameObject.name == "LeftBtn";
    }
    
    public void OnUpdateSelected(BaseEventData data) {
        if (clicked) {
            if (isLeftButton) {
                GameManager.getPlayerController().MoveLeft();
            }
            else {
                GameManager.getPlayerController().MoveRight();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        clicked = true;
    }
    public void OnPointerUp(PointerEventData eventData) {
        clicked = false;
    }

}
