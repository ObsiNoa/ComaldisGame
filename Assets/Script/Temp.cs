using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointer, results);

            Debug.Log("===== UI RAYCAST =====");

            foreach (RaycastResult result in results)
            {
                Debug.Log(
                    result.gameObject.name +
                    " | Canvas: " +
                    result.gameObject.GetComponentInParent<Canvas>()?.name +
                    " | Sorting Order: " +
                    result.gameObject.GetComponentInParent<Canvas>()?.sortingOrder
                );
            }
        }
    }
}