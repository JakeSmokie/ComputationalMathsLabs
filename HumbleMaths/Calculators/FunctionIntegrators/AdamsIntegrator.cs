﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HumbleMaths.Calculators.Integral;
using HumbleMaths.Calculators.Integral.Flat;
using HumbleMaths.Extensions;

namespace HumbleMaths.Calculators.FunctionIntegrators {
    public class AdamsIntegrator : IFunctionIntegrator {
        private readonly ConcurrentDictionary<(int amount, int elem), double> _coefficients =
            new ConcurrentDictionary<(int amount, int elem), double>();

        private readonly FlatIntegralCalculator _integralCalculator =
            new FlatIntegralCalculator(new EdgeFlatPartCalculator(0.0));

        private readonly int _maxAmountOfParts = 8;

        public AdamsIntegrator() {
            GenerateTable();
        }

        public ReadOnlyCollection<(double x, double z)> IntegrateFunction(
            Func<double, double, double> fFunction, Func<double, double> yFunction,
            (double x, double z) caucheSolution, double stepLength, int totalStepsAmount) {
            var points = new List<(double x, double z)> {
                caucheSolution
            };

            while (totalStepsAmount > 0) {
                var stepsAmount = _maxAmountOfParts;

                if (totalStepsAmount % _maxAmountOfParts != 0) {
                    stepsAmount = totalStepsAmount % _maxAmountOfParts;
                }

                for (var m = 1; m < stepsAmount + 1; m++) {
                    var xLast = points.Last().x;
                    var zLast = points.Last().z;

                    var x = xLast + stepLength;
                    var sum = Enumerable.Range(0, m)
                        .Sum(j => q(x) * _coefficients[(m - 1, j)]);

                    points.Add((x, zLast + sum));
                }

                totalStepsAmount -= stepsAmount;
            }

            return points.AsReadOnly();

            double q(double x) {
                return stepLength * fFunction(x, yFunction(x));
            }
        }

        private void GenerateTable() {
            Enumerable.Range(0, _maxAmountOfParts)
                .ToList()
                .ForEach(amount => {
                    Enumerable.Range(0, amount + 1)
                        .ToList()
                        .ForEach(i => GenerateElement(i, amount));
                });
        }

        private void GenerateElement(int i, int amount) {
            double result = i % 2 == 0 ? 1 : -1;

            Func<double, double> func =
                t => Enumerable.Range(0, amount + 1)
                    .SkipElementAtIndex(i)
                    .Aggregate(1.0, (acc, x) => acc * (t + x));

            result *= _integralCalculator.Calculate(func, double.Epsilon, 1 - double.Epsilon, 0.0001).Integral;
            result /= Factorial(i) * Factorial(amount - i);

            _coefficients.TryAdd((amount, i), result);
        }

        private long Factorial(int n) {
            if (n < 0) {
                return 0;
            }

            if (n == 0) {
                return 1;
            }

            if (n == 1 || n == 2) {
                return n;
            }

            return ProdTree(2, n);
        }

        private long ProdTree(long l, long r) {
            if (l > r) {
                return 1;
            }

            if (l == r) {
                return l;
            }

            if (r - l == 1) {
                return l * r;
            }

            var m = (l + r) / 2;
            return ProdTree(l, m) * ProdTree(m + 1, r);
        }
    }
}