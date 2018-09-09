﻿using System;

namespace HumbleMaths.Calculators {
    public interface IIntegralCalculator {
        double Calculate(Func<double, double> func, double start, double end);
    }
}