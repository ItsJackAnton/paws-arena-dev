using Anura.ConfigurationModule.Managers;
using Anura.ConfigurationModule.ScriptableObjects;
using Anura.Extensions;
using Photon.Pun;
using System;
using UnityEngine;

public class PlayerIndicatorBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerGraphicsBehaviour playerGraphicsBehaviour;
    [SerializeField] private Transform indicatorWrapper;
    [SerializeField] private Transform indicator;
    [SerializeField] public IndicatorInputCircleBehaviour indicatorCircle;
    [SerializeField] private LineRenderer lineDirectionIndicator;
    [SerializeField] private LineRenderer lineIndicatorSpeed;
    [SerializeField] private Transform launchPoint;

    [Header("Positions")]
    [SerializeField]
    private Vector2 indicatorWrapperLeftPosition;
    [SerializeField]
    private Vector2 indicatorWrapperRightPosition;
    [Space]
    [SerializeField]
    private Vector2 indicatorLeftPosition;
    [SerializeField]
    private Vector2 indicatorRightPosition;
    [Space]
    [SerializeField]
    private Vector2 launchPointLeftPosition;
    [SerializeField]
    private Vector2 launchPointRightPosition;

    [HideInInspector] public float currentPower;

    private PhotonView photonView;
    private Config config => ConfigurationManager.Instance.Config;
    private Vector2 lastMousePosition;
    private float maxRadius = -1;
    private float angle;

    private bool isHoldingSelect = false;

    public bool DoHandleKeyboardInput;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (maxRadius == -1)
        {
            maxRadius = lineDirectionIndicator.GetPosition(1).x;
        }

        lineDirectionIndicator.SetPosition(1, new Vector3(maxRadius, 0, 1));
        SetPowerLineLength(1.0f);
    }

    private void Update()
    {
        if (isHoldingSelect)
        {
            indicatorCircle.CheckPointerClick(lastMousePosition);
        }
    }
    
    private void FixedUpdate()
    {
        if (DoHandleKeyboardInput)
        {
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {
        float _powerChanger = 0.01f;
        float _angleChanger = 0.1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            currentPower = Mathf.Clamp(currentPower + _powerChanger, 0, 1);
            SetPowerLineLength(currentPower);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            currentPower = Mathf.Clamp(currentPower - _powerChanger, 0, 1);
            SetPowerLineLength(currentPower);
        }
        else if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D))
        {
            angle -= _angleChanger; 
            indicator.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            angle += _angleChanger; 
            indicator.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            return;
        }

        OnIndicatorPlaced(angle,currentPower);
        SetPowerLineLength(currentPower);
    }

    private void OnEnable()
    {
        indicatorCircle.onIndicatorPlaced += OnIndicatorPlaced;
        if (playerGraphicsBehaviour.isFacingRight)
        {
            SetDirection(true);
        }
        else
        {
            SetDirection(false);
        }
    }

    private void SetDirection(bool isRight)
    {
        indicatorWrapper.transform.localPosition = isRight ? indicatorWrapperRightPosition : indicatorWrapperLeftPosition;
        lineDirectionIndicator.transform.localPosition = isRight ? indicatorRightPosition : indicatorLeftPosition;
        lineIndicatorSpeed.transform.localPosition = isRight ? indicatorRightPosition : indicatorLeftPosition;
        launchPoint.transform.localPosition = isRight ? launchPointRightPosition : launchPointLeftPosition;
    }

    private void OnDisable()
    {
        indicatorCircle.onIndicatorPlaced -= OnIndicatorPlaced;
    }

    public void RegisterDirectionCallbacks(GameInputActions.PlayerActions playerActions)
    {
        playerActions.ScreenPosition.performed += value => lastMousePosition = value.ReadValue<Vector2>();
        playerActions.Select.started += _ => isHoldingSelect = indicatorCircle.IsSelected(lastMousePosition);
        playerActions.Select.canceled += _ => isHoldingSelect = false;
    }
    public void RegisterDirectionCallbacks(BotInputActions.PlayerActions playerActions)
    {
        playerActions.ScreenPosition.performed += value => lastMousePosition = value.ReadValue<Vector2>();
        playerActions.Select.started += _ => isHoldingSelect = indicatorCircle.IsSelected(lastMousePosition);
        playerActions.Select.canceled += _ => isHoldingSelect = false;
    }

    private void OnIndicatorPlaced(float _angle, float power)
    {
        angle = _angle;
        indicator.rotation = Quaternion.Euler(Vector3.zero.WithZ(_angle));

        SingleAndMultiplayerUtils.RpcOrLocal(this, photonView, false, "SetPowerLineLength", RpcTarget.All, power);
    }

    [PunRPC]
    public void SetPowerLineLength(float power)
    {
        lineIndicatorSpeed.SetPosition(1, new Vector3(0.2f + power * maxRadius, 0, 1));
        currentPower = power;
    }
}
