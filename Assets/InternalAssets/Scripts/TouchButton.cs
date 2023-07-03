using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float SCALE_DOWN_MILTIPLIER = 0.85f;

    public bool IsPressed { get; private set; }

    private RectTransform rectTransform;
    private Vector3 initialScale;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        rectTransform.localScale = initialScale * SCALE_DOWN_MILTIPLIER;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        rectTransform.localScale = initialScale;
    }
}
