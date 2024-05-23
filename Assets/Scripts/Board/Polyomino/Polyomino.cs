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
    #endregion

    public int[,] Shape { get; private set; }

    public Polyomino(int[,] shape) {
        Shape = shape;
    }


}
