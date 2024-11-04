using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using NewTypes;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameData _gameData;
    [SerializeField] public TextMeshProUGUI _coinTxt;
    [SerializeField] private TextMeshProUGUI _foodTxt;
    [SerializeField] private GameObject _coinImg;
    [SerializeField] private float _txtScaleTime;
    [SerializeField] private Vector3 _txtScaleMax;
    [SerializeField] private List<GameObject> _disableIngame;
    [SerializeField] private List<GameObject> _enableIngame;
    private LevelStateEnum _levelState;
    private Joystick _joystick;
    private InputSystem _inputSystem;
    private Player _player;

    private int playerCoins = 0;
    public PlayerProfile playerProfile; 
   
    void Awake()
    {
        _gameData.Food = 0;
        _gameData.Coins.ToString();
        _levelState = LevelStateEnum.WaitingTap;
        _inputSystem = new InputSystem();
        _coinTxt.text = _gameData.Coins.ToString();
        _foodTxt.text = _gameData.Food.ToString("00") + "/" + _gameData.MaxStacked.ToString();
        _joystick = new Joystick();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _player.GameData = _gameData;
        _player.GameManager = this.GetComponent<GameManager>();

        SystemSave.Load(_gameData);

    }

    void Update()
    {
        _inputSystem.ReadInput();
        LevelStateManager();
    }

    void LevelStateManager()
    {
        switch (_levelState)
        {
            case LevelStateEnum.WaitingTap:
                if (_inputSystem.TouchInfo.Phase == TouchPhase.Ended)
                {
                    ChangeLevelState(LevelStateEnum.Ingame);
                }
                break;
            case LevelStateEnum.Ingame:
                if (_inputSystem.TouchInfo.Phase == TouchPhase.Began)
                {
                    _joystick.ShowJoystick(true, _inputSystem.TouchInfo.StartPos);

                }
                else if ((_inputSystem.TouchInfo.Phase == TouchPhase.Moved || _inputSystem.TouchInfo.Phase == TouchPhase.Stationary))
                {
                    _player.Move(_joystick.MoveJoystick(_inputSystem.TouchInfo.Direction));
                }
                else if (_inputSystem.TouchInfo.Phase == TouchPhase.Ended)
                {
                    _joystick.ShowJoystick(false, _inputSystem.TouchInfo.StartPos);
                }
                break;
            default:
                break;
        }
    }

    public void ChangeLevelState(LevelStateEnum newLevelState)
    {
        
        switch (_levelState)
        {
            case LevelStateEnum.WaitingTap:
                if (newLevelState == LevelStateEnum.Ingame)
                {
                    foreach (GameObject gameObject in _disableIngame)
                    {
                        gameObject.SetActive(false);
                    }
                }
                break;
            case LevelStateEnum.Ingame:
                break;
            default:
                break;
        }
        _levelState = newLevelState;
    }

    public void SpawnCoin(Vector3 startPosition)
    {
        GameObject coin = Instantiate(_coinImg, _coinTxt.transform.parent);
        coin.GetComponent<Coin>().Move(Camera.main.WorldToScreenPoint(startPosition));
    }
    public void AddCoins(int value)
    {
        _gameData.Coins += (value * _gameData.FoodValue);
        if (_gameData.Coins > 9999) _gameData.Coins = 9999;
        UpdateTXT(_gameData.Coins.ToString(), _coinTxt);
        SystemSave.Save(_gameData);
        
    }

    public void AddFood(int value)
    {
        _gameData.Food += value;
        UpdateTXT(_gameData.Food.ToString("00") + "/" + _gameData.MaxStacked.ToString(), _foodTxt);
        
    }

    private void UpdateTXT(string value, TextMeshProUGUI txtObj)
    {
        txtObj.text = value;
        StartCoroutine(ScaleTXT(_txtScaleTime, txtObj));
    }

    IEnumerator ScaleTXT(float time, TextMeshProUGUI txtObj)
    {
        txtObj.transform.localScale = _txtScaleMax;

        yield return new WaitForSeconds(time);

        txtObj.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
