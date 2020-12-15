using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragLimit : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler,
    IEndDragHandler
{
    //父级root
    public RectTransform root;
    //限制区域rect
    public RectTransform limitRect;
    public Camera uiCamera;

    private RectTransform selfRect;
    private Vector2 offset = new Vector3();
    private Vector2 bottomLeft = new Vector2();
    private Vector2 topRight = new Vector2();

    void Awake()
    {
        selfRect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 mouseDown = eventData.position;
        Vector2 mouseUguiPos = new Vector2();
        bool isRect =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, mouseDown, eventData.enterEventCamera,
                out mouseUguiPos);
        if (isRect)
        {
            //计算屏幕偏移
            Vector2 selfScreenPos = uiCamera.WorldToScreenPoint(selfRect.position);
            offset = selfScreenPos - mouseDown;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouseDrag = eventData.position;
        Vector2 uguiPos = new Vector2();
        bool isRect =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, mouseDrag, eventData.enterEventCamera,
                out uguiPos);

        if (isRect)
        {
            if (limitRect)
            {
                //计算限制区域（limitRect的rect范围 + selfRect的锚点偏移）
                bottomLeft.x = limitRect.position.x
                               - limitRect.rect.width / 2 * limitRect.lossyScale.x
                               + selfRect.rect.width * (selfRect.pivot.x - 0.5f) * selfRect.lossyScale.x;
                bottomLeft.y = limitRect.position.y
                               - limitRect.rect.height / 2 * limitRect.lossyScale.y
                               + selfRect.rect.height * (selfRect.pivot.y - 0.5f) * selfRect.lossyScale.y;

                topRight.x = limitRect.position.x
                             + limitRect.rect.width / 2 * limitRect.lossyScale.x
                             + selfRect.rect.width * (selfRect.pivot.x - 0.5f) * selfRect.lossyScale.x;
                topRight.y = limitRect.position.y
                             + limitRect.rect.height / 2 * limitRect.lossyScale.y
                             + selfRect.rect.height * (selfRect.pivot.y - 0.5f) * selfRect.lossyScale.y;

                var screenPos = mouseDrag + offset;

                var bottomLeftScreenPos = uiCamera.WorldToScreenPoint(bottomLeft);
                var topRightScreenPos = uiCamera.WorldToScreenPoint(topRight);

                screenPos.x = Mathf.Clamp(screenPos.x, bottomLeftScreenPos.x, topRightScreenPos.x);
                screenPos.y = Mathf.Clamp(screenPos.y, bottomLeftScreenPos.y, topRightScreenPos.y);

                var pos = uiCamera.ScreenToWorldPoint(screenPos);
                pos.z = selfRect.position.z;

                selfRect.position = pos;
            }
            else
            {
                selfRect.anchoredPosition = offset + uguiPos;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        offset = Vector2.zero;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector2.zero;
    }
}