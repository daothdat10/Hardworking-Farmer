using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 2)]

[System.Serializable]
public class GameData : ScriptableObject
{
    [SerializeField] private int _coins;
    [SerializeField] private int _foodValue;
    [SerializeField] private int _maxStacked;
    

    
    public int Coins 
    {
        get => _coins;
        set => _coins = value;
    }

    public int MaxStacked
    {
        get => _maxStacked;
    }

    private int _levelNum;

    private int _food = 0;
    public int Food
    {
        get => _food;
        set => _food = value;
    }

    public int FoodValue
    {
        get => _foodValue;
    }

    
}
