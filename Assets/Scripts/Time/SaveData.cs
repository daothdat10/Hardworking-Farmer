using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    private static SaveData _current;

    public static SaveData current
    {
        get
        {
            if (_current == null) 
            {
                _current = new SaveData();
            }
            return _current;
        }
        set { _current = value; }
    }
    
    public TestData testData = new TestData();
    
    public PlayerProfile profile;
    public int toyHouse;
    public int toypets;

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        if (testData == null)
        {
            testData = new TestData();
        }
    }
}
