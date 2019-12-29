using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separator
{
    // Separatory używane przy zarządzaniu danymi.
    // Domyślny separator liczb.
    public static string decimalSeparatorString = ",";
    public static char decimalSeparatorChar = ',';
    // Domyślny separator danych.
    public static char dataSeparatorChar = ';';
    public static string dataSeparatorString = ";";

    // Ustawienie separatora między cyframi liczby (np. 12,23 -> 12.23)
    public static void Set(string sign = null)
    {
        System.Globalization.CultureInfo cultureInfo = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        cultureInfo.NumberFormat.NumberDecimalSeparator = sign ?? decimalSeparatorString;
        System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
    }

    // Funkcja zmieniająca separator danych.
    public static void SetDataSeparator(char newSeparator)
    {
        dataSeparatorChar = newSeparator;
        dataSeparatorString = newSeparator.ToString();
    }

    // Funkcja zmieniająca separator cyfr.
    public static void SetDecimalSeparator(char newSeparator)
    {
        decimalSeparatorChar = newSeparator;
        decimalSeparatorString = newSeparator.ToString();
    }
}
