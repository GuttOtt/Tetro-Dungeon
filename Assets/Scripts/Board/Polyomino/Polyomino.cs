using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Polyomino
{
    #region Polyomino Shapes
    public static readonly int[][,] Tetrominoes = {
    
        // I ��Ʈ���̳�
        new int[,] { {1, 1, 1, 1} },  // ����
        new int[,] { {1}, {1}, {1}, {1} },  // ����
    
    
        // O ��Ʈ���̳�
        new int[,] { {1, 1}, {1, 1} },
    
    
        // T ��Ʈ���̳�
        new int[,] { {1, 1, 1}, {0, 1, 0} },  // ����
        new int[,] { {0, 1}, {1, 1}, {0, 1} },  // 90�� ȸ��
        new int[,] { {0, 1, 0}, {1, 1, 1} },  // 180�� ȸ��
        new int[,] { {1, 0}, {1, 1}, {1, 0} },  // 270�� ȸ��
    
    
        // S ��Ʈ���̳�
        new int[,] { {0, 1, 1}, {1, 1, 0} },  // ����
        new int[,] { {1, 0}, {1, 1}, {0, 1} }, // 90�� ȸ��
    
    
        // Z ��Ʈ���̳�
        new int[,] { {1, 1, 0}, {0, 1, 1} },  // ����
        new int[,] { {0, 1}, {1, 1}, {1, 0} },  // 90�� ȸ��
    
    
        // J ��Ʈ���̳�
        new int[,] { {1, 0, 0}, {1, 1, 1} },  // ����
        new int[,] { {1, 1}, {1, 0}, {1, 0} },  // 90�� ȸ��
        new int[,] { {1, 1, 1}, {0, 0, 1} },  // 180�� ȸ��
        new int[,] { {0, 1}, {0, 1}, {1, 1} },  // 270�� ȸ��
    
    
        // L ��Ʈ���̳�
        new int[,] { {0, 0, 1}, {1, 1, 1} },  // ����
        new int[,] { {1, 0}, {1, 0}, {1, 1} },  // 90�� ȸ��
        new int[,] { {1, 1, 1}, {1, 0, 0} },  // 180�� ȸ��
        new int[,] { {1, 1}, {0, 1}, {0, 1} },  // 270�� ȸ��
    
    };

    public static readonly int[][,] Triominoes = {
        // I Ʈ�����̳�
        new int[,] { {1, 1, 1} },  // ����
        new int[,] { {1}, {1}, {1} },  // ����

        // L Ʈ�����̳�
        new int[,] { {1, 0}, {1, 0}, {1, 1} },  // ����
        new int[,] { {1, 1, 1}, {1, 0, 0} },  // 90�� ȸ��
    };

    public static readonly int[][,] Dominoes = {
        // I ���̳�
        new int[,] { {1, 1} },  // ����
        new int[,] { {1}, {1} },  // ����
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

    public static Polyomino GetRandomPolyomino(int number) {//�� ũ��
        int[][,] Minoes = MinoDict[number];
        int[,] shape = Minoes[UnityEngine.Random.Range(0, Minoes.Length)];
        return new Polyomino(shape);
    }

    public static Polyomino GetRandomPolyomino() {//�� ũ��
        return GetRandomPolyomino(UnityEngine.Random.Range(1, 5));
    }
}
