using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent animatorTriggerEvent, animatorFireTriggerEvent;

    public void SetAnimatorTriggerEvent()
    {
        animatorTriggerEvent.Raise();
    }

    public void SetFireAnimatorTriggerEvent()
    {
        animatorFireTriggerEvent.Raise();
    }
}