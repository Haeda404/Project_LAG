// SkillCard.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public string skillName;
    private BattleSystem battleSystem;
    private Transform originalParent;

    public void Init(BattleSystem system)
    {
        battleSystem = system;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<DropZone>() != null)
        {
            transform.SetParent(eventData.pointerEnter.transform);
        }
        else
        {
            transform.SetParent(originalParent);
        }
    }
}
