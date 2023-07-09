using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ScaleManager : Singleton<ScaleManager>
{
    [SerializeField] private float scaleIncreasePerLevel, scaleIncreasePerLevelForEnemy;
    public Vector3 startScaleSize;
    [SerializeField] private AdjustCamera _adjustCamera;

    [SerializeField] private CharacterManage playerCharacterManage;

    private void Awake()
    {
        startScaleSize = playerCharacterManage.transform.localScale;
    }

    public Vector3 ScaleAmountFinder(int level, bool enemy)
    {
        var scale = ScaleCalculator(level, enemy);
        var localScale = Vector3.zero;
        if (enemy)
            localScale = startScaleSize + (scale + .1f) * Vector3.one;
        else
            localScale = startScaleSize + scale * Vector3.one;


        return localScale;
    }

    private float ScaleCalculator(int level, bool enemy)
    {
        float scale;

        if (enemy)
        {
            if (level < 100)
            {
                scale = level * scaleIncreasePerLevelForEnemy * 90;
            }
            else
            {
                scale = 100 * scaleIncreasePerLevelForEnemy * 90;
                scale += (level - 100) * scaleIncreasePerLevelForEnemy;
            }
        }
        else
        {
            if (level < 100)
            {
                scale = level * scaleIncreasePerLevel * 90;
            }
            else
            {
                scale = 100 * scaleIncreasePerLevel * 90;
                scale += (level - 100) * scaleIncreasePerLevel;
            }
        }


        return scale;
    }

    public void ChangePlatformAndCamScale(bool set)
    {
        var time = playerCharacterManage.characterLevelChangingTime;
        if (set)
        {
            var playerLevel = playerCharacterManage.GetLevel();
            _adjustCamera.PosSet(scaleIncreasePerLevel, startScaleSize.x, playerLevel);
        }
        else
        {
            var playerLevel = playerCharacterManage.GetLevel();
            StartCoroutine(_adjustCamera.MoveWithinSeconds(time, scaleIncreasePerLevel, startScaleSize.x,
                playerLevel));
        }
    }

    public void ScaleCalculationAndSet(bool set, int realLevel, GameObject character, bool enemy)
    {
        if (!set)
        {
            if (character.activeSelf)
                StartCoroutine(ScaleWithinSeconds(character.transform,
                    ScaleAmountFinder(realLevel, enemy),
                    playerCharacterManage.characterLevelChangingTime));
            if (!enemy)
                Instance.ChangePlatformAndCamScale(false);
        }
        else
        {
            character.transform.localScale = ScaleAmountFinder(realLevel, enemy);

            if (!enemy)
                ChangePlatformAndCamScale(true);
        }
    }

    public void ScaleManagerReset()
    {
        StopAllCoroutines();
    }


    public IEnumerator ScaleWithinSeconds(Transform obj, Vector3 to, float duration)
    {
        var timePassed = 0f;
        var startScalen = obj.transform.localScale;
        while (timePassed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(startScalen, to, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }

        obj.localScale = to;
    }
}