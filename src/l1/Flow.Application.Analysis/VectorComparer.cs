﻿using System.Globalization;
using Flow.Domain.Analysis;

namespace Flow.Application.Analysis;

internal class VectorComparer : IComparer<Vector>
{
    private CultureInfo culture;

    public VectorComparer(CultureInfo culture)
    {
        this.culture = culture;
    }

    public int Compare(Vector? x, Vector? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;

        var idx = 0;
        while (idx < x.Length) {
            if (y.Length <= idx) return 1;
            var result = string.Compare(x[idx], y[idx], culture, CompareOptions.StringSort);
            if (result != 0) return result;
            idx++;
        }

        return x.Length - y.Length;
    }

}