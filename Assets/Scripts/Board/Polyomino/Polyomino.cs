using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Polyomino
{
    #region Polyomino Shapes
    public static readonly int[][,] Tetrominoes = {
    
        // I 테트리미노
        new int[,] { {1, 1, 1, 1} },  // 수평
        new int[,] { {1}, {1}, {1}, {1} },  // 수직
    
    
        // O 테트리미노
        new int[,] { {1, 1}, {1, 1} },
    
    
        // T 테트리미노
        new int[,] { {1, 1, 1}, {0, 1, 0} },  // 원래
        new int[,] { {0, 1}, {1, 1}, {0, 1} },  // 90도 회전
        new int[,] { {0, 1, 0}, {1, 1, 1} },  // 180도 회전
        new int[,] { {1, 0}, {1, 1}, {1, 0} },  // 270도 회전
    
    
        // S 테트리미노
        new int[,] { {0, 1, 1}, {1, 1, 0} },  // 원래
        new int[,] { {1, 0}, {1, 1}, {0, 1} }, // 90도 회전
    
    
        // Z 테트리미노
        new int[,] { {1, 1, 0}, {0, 1, 1} },  // 원래
        new int[,] { {0, 1}, {1, 1}, {1, 0} },  // 90도 회전
    
    
        // J 테트리미노
        new int[,] { {1, 0, 0}, {1, 1, 1} },  // 원래
        new int[,] { {1, 1}, {1, 0}, {1, 0} },  // 90도 회전
        new int[,] { {1, 1, 1}, {0, 0, 1} },  // 180도 회전
        new int[,] { {0, 1}, {0, 1}, {1, 1} },  // 270도 회전
    
    
        // L 테트리미노
        new int[,] { {0, 0, 1}, {1, 1, 1} },  // 원래
        new int[,] { {1, 0}, {1, 0}, {1, 1} },  // 90도 회전
        new int[,] { {1, 1, 1}, {1, 0, 0} },  // 180도 회전
        new int[,] { {1, 1}, {0, 1}, {0, 1} },  // 270도 회전
    
    };

    public static readonly int[][,] Triominoes = {
        // I 트리오미노
        new int[,] { {1, 1, 1} },  // 수평
        new int[,] { {1}, {1}, {1} },  // 수직

        // L 트리오미노
        new int[,] { {1, 0}, {1, 0}, {1, 1} },  // 원래
        new int[,] { {1, 1, 1}, {1, 0, 0} },  // 90도 회전
    };

    public static readonly int[][,] Dominoes = {
        // I 도미노
        new int[,] { {1, 1} },  // 수평
        new int[,] { {1}, {1} },  // 수직
    };

    public static readonly int[][,] Monominoes = {
        new int[,] { { 1 } }
    };

    public static readonly Dictionary<int, int[][,]> MinoDict = new Dictionary<int, int[][,]> {
        { 1, Monominoes },
        { 2, Dominoes}, 
        { 3, Triominoes },
        { 4, Tetrominoes }
    };
    #endregion

    public int[,] Shape { get; private set; }
    public int Size {
        get {
            int size = 0;
            foreach (int i in Shape) {
                if (i == 1) size++;
            }
            return size;
        }
    }

    public Polyomino(int[,] shape) {
        Shape = shape;
    }

    public Polyomino ClockwiseSpin() {
        int[,] originShape = Shape;
        int originCol = originShape.GetLength(0);
        int originRow = originShape.GetLength(1);
        int[,] rotatedShape = new int[originRow, originCol];

        for (int i = 0; i < originRow; i++) {
            for (int j = 0; j < originCol; j++) {
                rotatedShape[i, j] = originShape[j, originRow - 1 - i];
            }
        }

        return new Polyomino(rotatedShape);
    }

    public Polyomino AnticlockwiseSpin() {
        int[,] originShape = Shape;
        int originCol = originShape.GetLength(0);
        int originRow = originShape.GetLength(1);
        int[,] rotatedShape = new int[originRow, originCol];

        for (int i = 0; i < originRow; i++) {
            for (int j = 0; j < originCol; j++) {
                rotatedShape[i, j] = originShape[originCol - 1 - j, i];
            }
        }

        return new Polyomino(rotatedShape);
    }

    public static Polyomino GetRandomPolyomino(int number) {//블럭 크기
        int[][,] Minoes = MinoDict[number];
        int[,] shape = Minoes[UnityEngine.Random.Range(0, Minoes.Length)];
        return new Polyomino(shape);
    }

    public static Polyomino GetRandomPolyomino() {//블럭 크기
        return GetRandomPolyomino(UnityEngine.Random.Range(1, 5));
    }
}
