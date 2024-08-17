using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public static class Utils {
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

    public static void Shuffle<T>(this IList<T> list) {
        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static int[,] ConvertBoolArrayToIntArray(bool[,] boolArray) {
        int cols = boolArray.GetLength(0);
        int rows = boolArray.GetLength(1);
        int[,] intArray = new int[rows, cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                intArray[i, j] = boolArray[i, j] ? 1 : 0;
            }
        }

        return intArray;
    }

    public static bool[,] ConvertIntArrayToBoolArray(int[,] intArray) {
        int cols = intArray.GetLength(0);
        int rows = intArray.GetLength(1);
        bool[,] boolArray = new bool[rows, cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                boolArray[i, j] = intArray[i, j] == 1 ? true : false;
            }
        }

        return boolArray;
    }

    public static bool[,] ConvertCellArrayToBoolArray(Cell[,] cellArray) {
        int cols = cellArray.GetLength(0);
        int rows = cellArray.GetLength(1);
        bool[,] boolArray = new bool[rows, cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                boolArray[i, j] = cellArray[i, j].Unit != null ? true : false;
            }
        }

        return boolArray;
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) //5 = UI layer
            {
                return true;
            }
        }

        return false;
    }
}
