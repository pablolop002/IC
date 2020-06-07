using System;
using System.Threading.Tasks;

namespace MetodosClasificacion.Algoritmia
{
    public static class Matrix
    {
        public static double[][] CalcularInversa(double[][] matrizCovarianza)
        {
            var n = matrizCovarianza.Length;
            var result = MatrixDuplicate(matrizCovarianza);

            var lum = MatrixDecompose(matrizCovarianza, out var perm, out var toggle);

            if (lum == null)
                throw new Exception("Unable to compute inverse");

            var b = new double[n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                var x = HelperSolve(lum, b);

                for (var j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        static double[][] MatrixCreate(int rows, int cols)
        {
            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            var result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (var i = 0; i < matrix.Length; ++i) // copy the values
                for (var j = 0; j < matrix[i].Length; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // returns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.Length;
            int cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[][] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(result[j][j]); // find largest val in col
                int pRow = j;

                // reader Matt V needed this:
                for (int i = j + 1; i < n; ++i)
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    var goodRow = -1;
                    for (var row = j + 1; row < n; ++row)
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (result[row][j] != 0.0)
                            goodRow = row;

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                for (var i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (var k = j + 1; k < n; ++k)
                        result[i][k] -= result[i][j] * result[j][k];
                }

            } // main j column loop

            return result;
        }

        public static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            var aRows = matrixA.Length; var aCols = matrixA[0].Length;
            var bRows = matrixB.Length; var bCols = matrixB[0].Length;

            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            double[][] result = MatrixCreate(aRows, bCols);

            //for (int i = 0; i < aRows; ++i) // each row of A
            //    for (int j = 0; j < bCols; ++j) // each col of B
            //        for (int k = 0; k < aCols; ++k) // could use k less-than bRows
            //            result[i][j] += matrixA[i][k] * matrixB[k][j];

            Parallel.For(0, aRows, i =>
              {
                  for (int j = 0; j < bCols; ++j)
                      for (int k = 0; k < aCols; ++k)
                          result[i][j] += matrixA[i][k] * matrixB[k][j];
              }
            );
            return result;
        }

        public static double[] MatrixVectorProduct(double[][] matrix, double[] vector)
        {
            // result of multiplying an n x m matrix by a m x 1 
            // column vector (yielding an n x 1 column vector)
            var mRows = matrix.Length; int mCols = matrix[0].Length;
            var vRows = vector.Length;
            if (mCols != vRows)
                throw new Exception("Non-conformable matrix and vector");

            var result = new double[mRows];

            //for (int i = 0; i < mRows; ++i)
            //    for (int j = 0; j < mCols; ++j)
            //        result[i] += matrix[i][j] * vector[j];

            Parallel.For(0, mRows, i =>
            {
                for (var j = 0; j < mCols; ++j)
                    result[i] += matrix[i][j] * vector[j];
            });

            return result;
        }

        public static double MatrixDeterminant(double[][] matrix)
        {
            var lum = MatrixDecompose(matrix, out var perm, out var toggle);
            if (lum == null)
                throw new Exception("Unable to compute MatrixDeterminant");
            double result = toggle;
            for (var i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }
    }
}