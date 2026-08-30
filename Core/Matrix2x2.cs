
using UnityEngine;

namespace Core
{
    public class Matrix2x2
    {
        public float[,] matrix;

        public Matrix2x2(Vector2 column1, Vector2 column2)
        {
            matrix = new float[2,2];
            matrix[0, 0] = column1.x;
            matrix[1, 0] = column1.y;
            matrix[0, 1] = column2.x;
            matrix[1, 1] = column2.y;
        }

        public Matrix2x2(float[,] matrix)
        {
            this.matrix = new float[2,2];
            this.matrix[0, 0] = matrix[0, 0];
            this.matrix[0, 1] = matrix[0, 1];
            this.matrix[1, 0] = matrix[1, 0];
            this.matrix[1, 1] = matrix[1, 1];
        }

        public float determinant()
        {
            return matrix[0,0] * matrix[1,1] - matrix[1,0] * matrix[0,1];
        }

        public Vector2 applyTransformation(Vector2 vec)
        {
            return new Vector2(matrix[0,0] * vec.x + matrix[0,1] * vec.y,matrix[1,0] * vec.x + matrix[1,1] * vec.y);
        }

        public Vector2 applyTransformation(float x, float y)
        {
            return applyTransformation(new Vector2(x, y));
        }
        public Vertex applyTransformation(Vertex vec)
        {
            return new Vertex(matrix[0,0] * vec.x + matrix[0,1] * vec.y,matrix[1,0] * vec.x + matrix[1,1] * vec.y);
        }
    }
}