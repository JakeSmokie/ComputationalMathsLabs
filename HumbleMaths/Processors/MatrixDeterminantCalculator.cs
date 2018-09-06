﻿using HumbleMaths.Structures;

namespace HumbleMaths.Processors {
    public class MatrixDeterminantCalculator {
        private readonly MatrixClamper _matrixClamper = new MatrixClamper();
        private readonly MatrixMinorCalculator _minorCalculator = new MatrixMinorCalculator();

        public double CalculateDeterminant(Matrix<double> matrix)
        {
            var squareMatrix = _matrixClamper.ClampMatrixToSquare(matrix);

            if (matrix.Height == 2) {
                return CalculateDeterminantForMatrix2D(matrix);
            }

            return CalculateDeterminantUsingMinors(squareMatrix);
        }

        private double CalculateDeterminantUsingMinors(Matrix<double> squareMatrix)
        {
            var determinant = 0.0;

            for (var column = 0; column < squareMatrix.Width; column++) {
                var minor = _minorCalculator.GetMinor(squareMatrix, 0, column);
                var multiplier = column % 2 == 0 ? 1 : -1;

                determinant += CalculateDeterminant(minor) *
                               squareMatrix[0, column] * multiplier;
            }

            return determinant;
        }

        private static double CalculateDeterminantForMatrix2D(Matrix<double> matrix)
        {
            var determinant = matrix[0, 0] * matrix[1, 1];
            determinant -= matrix[1, 0] * matrix[0, 1];

            return determinant;
        }
    }
}