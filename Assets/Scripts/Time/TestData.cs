using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestData
{
    public string timeStart;
    public string timeEnd;

    public TestData()
    {
        timeStart = "";
        timeEnd = "";
    }

    public TestData(DateTime start, DateTime end)
    {
        timeStart = start.ToString();
        timeEnd = end.ToString();
    }
}
