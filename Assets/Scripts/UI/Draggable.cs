using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging;
    private Vector2 offset;

    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}