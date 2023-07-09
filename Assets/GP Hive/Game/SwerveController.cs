using System.Collections;
using UnityEngine;
using GPHive.Core;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody))]
public class SwerveController : MonoBehaviour
{
    private float lastFingerPosX;
    private float swerveAmount;
    private float difference;

    private Vector3 swerveInput;

    [SerializeField] private bool useLimitTransform;

    [ShowIf("useLimitTransform")] [SerializeField]
    private Transform leftLimit, rightLimit;

    Rigidbody rigidbody;

    private Coroutine inputResetCoroutine;
    [SerializeField] private SwerveControllerConfig swerveControllerConfig;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
            Swerve();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
            Movement();
    }

    private void Swerve()
    {
        if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);

            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    if (inputResetCoroutine != null)
                        StopCoroutine(inputResetCoroutine);

                    lastFingerPosX = _touch.position.x;
                    swerveAmount = 0;
                    swerveInput = Vector3.zero;
                    break;
                case TouchPhase.Moved:
                    difference = (_touch.position.x - lastFingerPosX) / Screen.width;
                    swerveAmount = Mathf.Clamp(difference * swerveControllerConfig.Sensitivity * Time.fixedDeltaTime,
                        -swerveControllerConfig.SwerveLimit, swerveControllerConfig.SwerveLimit);
                    swerveInput = new Vector3(swerveAmount, 0, 0);
                    lastFingerPosX = _touch.position.x;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    inputResetCoroutine = StartCoroutine(ResetInput(swerveControllerConfig.InputResetTime));
                    break;
            }
        }
    }

    private IEnumerator ResetInput(float time)
    {
        var _time = 0f;

        while (_time < time)
        {
            _time += Time.deltaTime;
            swerveInput = Vector3.Lerp(swerveInput, Vector3.zero, _time / time);
            yield return null;
        }

        swerveInput = Vector3.zero;
    }

    private void Movement()
    {
        var _verticalMovement = swerveControllerConfig.ForwardSpeed * Time.deltaTime * transform.forward;
        var _horizontalMovement = swerveInput * swerveControllerConfig.HorizontalSpeed;

        _horizontalMovement =
            swerveInput.magnitude < swerveControllerConfig.Threshold ? Vector3.zero : _horizontalMovement;

        var _finalMovement = _verticalMovement + _horizontalMovement;

        if (useLimitTransform)
            if ((rigidbody.position + _finalMovement).x < leftLimit.position.x ||
                (rigidbody.position + _finalMovement).x > rightLimit.position.x)
                _finalMovement.x = 0;

        rigidbody.MovePosition(rigidbody.position + _finalMovement);
    }
}