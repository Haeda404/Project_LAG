// DropZone.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        SkillCard skillCard = eventData.pointerDrag.GetComponent<SkillCard>();
        if (skillCard != null)
        {
            skillCard.transform.SetParent(transform);
        }
    }
}
