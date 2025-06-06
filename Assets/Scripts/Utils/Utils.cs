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

    public static T Pick<T>(Vector3 origin, T exception = default(T)) {
        // Cast a ray downwards to find the Cell below the BlockPart
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector3.back, 100); // Increased distance and changed direction

        List<GameObject> targetList = new List<GameObject>();

        foreach (RaycastHit2D hit in hits) {
            GameObject hitObject = hit.collider.gameObject;
            T component = hitObject.GetComponent<T>();

            Debug.Log($"Component T: {component}");

            if (component != null && !EqualityComparer<T>.Default.Equals(component, exception))
            {
                return component;
            }
        }

        return default(T);
    }

    public static CharacterBlock PickCharacterBlock() {
        BlockPart selectedBlockPart = Pick<BlockPart>();
        if (selectedBlockPart == null) return null;

        CharacterBlock selectedBlock = selectedBlockPart.CharacterBlock;
        if (selectedBlock == null) return null;

        return selectedBlock;
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

    public static T[,] HorizontalFlip<T>(T[,] original) {
        int x = original.GetLength(0);
        int y = original.GetLength(1);

        T[,] fliped = new T[x, y];

        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                fliped[i, j] = original[x - i - 1, j];
            }
        }

        return fliped;
    }

    public static T[,] VertialFlip<T>(T[,] original) {
        int x = original.GetLength(0);
        int y = original.GetLength(1);

        T[,] fliped = new T[x, y];

        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                fliped[i, j] = original[i, y - j - 1];
            }
        }

        return fliped;
    }

    public static T[,] RotateRight<T>(T[,] original) {
        int lengthX = original.GetLength(0);
        int lengthY = original.GetLength(1);

        T[,] rotated = new T[lengthY, lengthX];

        for (int x = 0; x < rotated.GetLength(0); x++) {
            for (int y = 0; y < rotated.GetLength(1); y++) {
                rotated[x, y] = original[y, lengthY - x - 1];
            }
        }

        //rotated 배열을 Debug.Logging
        Debug.Log("Right Rotated Array:");
        for (int x = 0; x < rotated.GetLength(0); x++) {
            for (int y = 0; y < rotated.GetLength(1); y++) {
                Debug.Log($"({x}, {y}): {rotated[x, y]} ");
            }
        }

        return rotated;
    }

    public static T[,] RotateLeft<T>(T[,] original) {
        int lengthX = original.GetLength(0);
        int lengthY = original.GetLength(1);

        T[,] rotated = new T[lengthY, lengthX];

        for (int x = 0; x < rotated.GetLength(0); x++) {
            for (int y = 0; y < rotated.GetLength(1); y++) {
                rotated[x, y] = original[lengthX - 1 - y, x];
            }
        }

        //rotated 배열을 Debug.Logging
        Debug.Log("Left Rotated Array:");
        for (int x = 0; x < rotated.GetLength(0); x++) {
            for (int y = 0; y < rotated.GetLength(1); y++) {
                Debug.Log($"({x}, {y}): {rotated[x, y]} ");
            }
        }

        return rotated;
    }

    public static int CalculateDamageAmount(BaseUnit unit, int baseDamage, float attackRatio, float spellPowerRatio){
        return (int)(baseDamage + unit.Attack * attackRatio + unit.SpellPower * spellPowerRatio);
    }
}
