using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _turnSpeed;

    [Header("Coin")]
    [SerializeField] private float _sellTime;
    [SerializeField] private float _coinTime;

    [Header("Attack")]
    [SerializeField] private GameObject _weaponAttack;
    [SerializeField] private GameObject _weaponIdle;
    [SerializeField] private Transform _stackBlocksPoint;

    [Header("Cam")]
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private float _cameraLimitDistance;
    [SerializeField] private float _cameraStep;
    private CinemachineFramingTransposer _cameraSettings;

   
    [Header("Music")]
    [SerializeField] private AudioSource runningSound;
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource CutSound;
    [SerializeField] private AudioSource CoinSound;
    [SerializeField] private AudioSource CoinIn;

    private Stack<Transform> _stackedBlocks;
    private bool _isMoving = false;
    private Vector3 _movePosition;
    private Rigidbody _ridigbody;
    private Animator _animator;
    private Transform _model;
    private const float _delta = 0.001f;
    private PlayerState _state;
    private bool _sellBlocks;
    private int _coinsToSpawn = 0;
    private GameData _gameData;
    public GameData GameData
    {
        set => _gameData = value;
    }

    private GameManager _gameManager;
    public GameManager GameManager
    {
        set => _gameManager = value;
    }

    enum PlayerState
    {
        Idle,
        Moving
    }
    void Awake()
    {
        _ridigbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _model = _animator.gameObject.transform;
        _state = PlayerState.Idle;
        _stackedBlocks = new Stack<Transform>();
        _sellBlocks = false;
        _cameraSettings = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
        
    }

    public void Move(Vector3 newPosition)
    { 
        
        _movePosition = new Vector3(newPosition.x, 0, newPosition.y);
        if (!Mathf.Approximately(_movePosition.sqrMagnitude, 0))
        {
            MoveAnim(_movePosition.sqrMagnitude);
            _isMoving = true;
            
        }
            
    }

    private void MoveAnim(float strength)
    {
        _state = PlayerState.Moving;
        if (Mathf.Approximately(strength, 1) && !_animator.GetBool("IsAttacking"))
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isRunning", true);
             walkSound.Play();


        }
        else
        {
            _animator.SetBool("isWalking", true);
            _animator.SetBool("isRunning", false);
            
            runningSound.Play();

        }
    }

    private void AttackAnim(bool value)
    {
        _animator.SetBool("IsAttacking", value);
        _weaponAttack.SetActive(value);
        _weaponIdle.SetActive(!value);
      
    }

    private void StopAnim()
    {
        _state = PlayerState.Idle;
        walkSound.Stop();
        runningSound.Stop();
       
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isRunning", false);
    }
    void FixedUpdate()
    {
        if (_isMoving)
        {
            _ridigbody.velocity = _movePosition * _moveSpeed;
            _model.rotation = Quaternion.Slerp(_model.rotation, Quaternion.LookRotation(_movePosition), _turnSpeed * Time.fixedDeltaTime);
            _isMoving = false;

        }
        else
        {
            if (_state != PlayerState.Idle) StopAnim();
            
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Field":
                AttackAnim(true);
                CutSound.Play();
                break;
            case "Wheat":
                other.GetComponent<Food>().Cut();
                break;
            case "Block":
                CollectBlock(other.gameObject);
                break;
            case "Sell":
                SellBlocks(other.gameObject);
                
                break;
            default:
                Debug.Log("Trigger enter not implemented: " + other.tag);
                
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Field":
                AttackAnim(false);
                CutSound.Stop();
                break;
            case "Sell":
                _sellBlocks = false;
                
                break;
            default:
                Debug.Log("Trigger exit not implemented: " + other.tag);
                break;
        }
    }

    private void CollectBlock(GameObject block)
    {
        Debug.Log("Current stack count: " + _stackedBlocks.Count);
        Debug.Log("Max stacked: " + _gameData.MaxStacked);
        if (_stackedBlocks.Count < _gameData.MaxStacked)
        {
            _gameManager.AddFood(1);
            _stackedBlocks.Push(block.transform);
            block.GetComponent<Block>().Stack(_stackBlocksPoint, _stackedBlocks.Count);
            SetCameraDistance(_stackedBlocks.Count);
           
        }
        else
        {
            block.GetComponent<Block>().SetLimitTexture();
        }
    }

    private void SellBlocks(GameObject block)
    {
        _sellBlocks = true;
        StartCoroutine(SellBlock(_sellTime, block.GetComponentsInChildren<Transform>()[1]));
        
    }

    IEnumerator SellBlock(float time, Transform block)
    {
        _coinsToSpawn = 0;
        while (_sellBlocks)
        {
            if (_stackedBlocks.Count > 0)
            {
                _coinsToSpawn++;
                _gameManager.AddFood(-1);
                CoinSound.Play();
                _stackedBlocks.Pop().GetComponent<Block>().Unstack(block);
                SetCameraDistance(_stackedBlocks.Count);

            }
            else
            {
                _sellBlocks = false;
                CoinSound.Stop();
            }
            yield return new WaitForSeconds(time);
        }
        if (_coinsToSpawn > 0)
        {
            StartCoroutine(SpawnCoins(_coinTime, block.position));
            
        }
    }

    IEnumerator SpawnCoins(float time, Vector3 startPoint)
    {
        yield return new WaitForSeconds(time);

        while (_coinsToSpawn > 0)
        {
            _coinsToSpawn--;
            _gameManager.SpawnCoin(startPoint);
            yield return new WaitForSeconds(time);
            CoinIn.Play();
        }
        

    }
    private void SetCameraDistance(int blocksCount)
    {
        float newCameraDistance = blocksCount / _cameraLimitDistance + _cameraStep;
        if (newCameraDistance > _cameraLimitDistance) _cameraSettings.m_CameraDistance = newCameraDistance;
        else _cameraSettings.m_CameraDistance = _cameraLimitDistance;
    }
}
