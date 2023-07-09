using System;
using System.Collections;
using System.Collections.Generic;
using GPHive.Game;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public ParticleSystem hitParticle;
    [SerializeField] private GameObject hit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void AddVelocitiy(Vector3 targetPos, float time)
    {
        _rigidbody.velocity = (targetPos - transform.position) / time;
        Invoke(nameof(CloseMe), time);
    }

    private void CloseMe()
    {
        var go = ObjectPooling.Instance.GetFromPool(hit);
        go.transform.position = hitParticle.transform.position;
        go.transform.localScale = hitParticle.transform.lossyScale;
        go.SetActive(true);
        go.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
    }
}