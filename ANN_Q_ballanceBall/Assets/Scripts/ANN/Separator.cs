using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separator
{
    public static string decimalSeparator = ".";
    public static char dataSeparator = ',';

    public static void Set(string sign = null)
    {
        System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        cultureInfo.NumberFormat.NumberDecimalSeparator = sign ?? decimalSeparator;
        System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
    }
}
