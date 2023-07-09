using System;
using Cinemachine;
using UnityEngine;
using System.Collections;
using System.Numerics;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

public class AdjustCamera : MonoBehaviour
{
    public Vector3 minFollow;
    private CinemachineTransposer _cinemachineTransposer;
    private CinemachineVirtualCamera _vCam;
    [SerializeField] private Material skyMaterial;

    [SerializeField] private float levelByPlayerMove;

    public IEnumerator MoveWithinSeconds(float duration, float scaleIncreasePerLevel, float startScaleSize,
        int playerLevel)
    {
        StopAllCoroutines();
        var timePassed = 0f;
        var startPos = transform.position;
        var levelByScaleForScaleRaitoOne = minFollow * scaleIncreasePerLevel / startScaleSize;
        //  var to = minFollow + levelByScaleForScaleRaitoOne * playerLevel;

        var to = PossitionCalculator(playerLevel);

        while (timePassed < duration)
        {
            transform.position = Vector3.Lerp(startPos, to, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
        var temp = 1f / (50000 / playerLevel);
        if (temp > 1) temp = 1f;

        skyMaterial.DOComplete();
        skyMaterial.DOFloat(temp, "_TIME", .25f);
        //_vCam.m_Lens.FarClipPlane = 100 + (int)playerLevel * 5;
    }

    public void PosSet(float scaleIncreasePerLevel, float startScaleSize, int playerLevel)
    {
        //_vCam.m_Lens.FarClipPlane = 500 + (int)playerLevel * 5;
        var temp = 1f / (50000 / playerLevel);
        if (temp > 1) temp = 1f;
        skyMaterial.SetFloat("_TIME", temp);


        transform.position = PossitionCalculator(playerLevel);
    }

    private Vector3 PossitionCalculator(int playerLevel)
    {
        Vector3 to;
        if (playerLevel < 100)
        {
            to = playerLevel * 90 * levelByPlayerMove * -transform.forward + minFollow;
        }
        else
        {
            to = 100 * 90 * levelByPlayerMove * -transform.forward + minFollow;
            if (playerLevel > 20000)
                to += (playerLevel - 100) * levelByPlayerMove * 1.1f * -transform.forward;
            else
                to += (playerLevel - 100) * levelByPlayerMove * -transform.forward;
        }

        return to;
    }

    private void OnEnable()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
        _cinemachineTransposer = _vCam.GetCinemachineComponent<CinemachineTransposer>();
        transform.position = minFollow;
    }
}