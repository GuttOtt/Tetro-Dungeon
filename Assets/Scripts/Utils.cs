using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T Pick<T>() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector3.forward, 1);

        List<GameObject> targetList = new List<GameObject>();

        foreach (RaycastHit2D hit in hits) {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.GetComponent<T>() != null) {
                return hitObject.GetComponent<T>();
            }
        }

        return default(T);
    }

    public static T Pick<T>(Vector3 origin) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector3.forward, 1);

        List<GameObject> targetList = new List<GameObject>();

        foreach (RaycastHit2D hit in hits) {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.GetComponent<T>() != null) {
                return hitObject.GetComponent<T>();
            }
        }

        return default(T);
    }
}
