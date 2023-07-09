using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPHive.Game
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        public GameObject CharDeadParticle;
        public ParticleSystemForceField ForceField;
        public ObjectToPool[] objectsToPool;

        public delegate void OnGetFromPool();

        public static event OnGetFromPool GotFromPool;

        private void Awake()
        {
            InitializePool();
        }

        public GameObject GetFromPool(GameObject prefab)
        {
            GameObject _objectToReturn = null;
            foreach (var pool in objectsToPool)
            {
                if (pool.objectToPool != prefab) continue;

                foreach (var _pooledObject in pool.pooledObjects)
                {
                    if (_pooledObject.activeSelf)
                        continue;

                    _objectToReturn = _pooledObject;
                    break;
                }

                if (!ReferenceEquals(_objectToReturn, null))
                    break;


                for (var i = 0; i < pool.expandAmount; i++)
                {
                    var _pooled = Instantiate(pool.objectToPool, transform);
                    _pooled.name = pool.name;
                    _pooled.SetActive(false);
                    pool.pooledObjects.Add(_pooled);
                    _objectToReturn = _pooled;
                }
            }

            GotFromPool?.Invoke();
            return _objectToReturn;
        }

        public void Deposit(GameObject gameObject)
        {
            foreach (var pool in objectsToPool)
                if (pool.pooledObjects.Contains(gameObject))
                {
                    gameObject.transform.SetParent(transform);
                    gameObject.SetActive(false);
                    gameObject.transform.ResetTransform();

                    if (gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
                        rigidbody.ResetVelocity();

                    return;
                }

            Debug.LogError("Trying to deposit an object that isn't in the pool.");
        }

        public void Deposit(string poolName)
        {
            foreach (var pool in objectsToPool)
            {
                if (pool.name != poolName) continue;

                foreach (var pooledObject in pool.pooledObjects)
                {
                    pooledObject.transform.parent = transform;
                    pooledObject.SetActive(false);
                    pooledObject.transform.ResetTransform();

                    if (pooledObject.TryGetComponent<Rigidbody>(out var rigidbody))
                        rigidbody.ResetVelocity();
                }
            }
        }

        public void DepositAll()
        {
            foreach (var pool in objectsToPool)
            foreach (var gameObject in pool.pooledObjects)
            {
                gameObject.transform.parent = transform;
                gameObject.transform.ResetTransform();
                gameObject.SetActive(false);

                if (gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
                    rigidbody.ResetVelocity();
            }
        }

        public void Clear()
        {
            foreach (var pool in objectsToPool)
            {
                foreach (var gameObject in pool.pooledObjects) Destroy(gameObject);

                pool.pooledObjects.Clear();
            }
        }

        private void InitializePool()
        {
            foreach (var pool in objectsToPool)
                for (var i = 0; i < pool.poolCount; i++)
                {
                    var _pooled = Instantiate(pool.objectToPool, transform);
                    _pooled.name = pool.name;
                    _pooled.SetActive(false);
                    pool.pooledObjects.Add(_pooled);
                    if (pool.objectToPool==CharDeadParticle)
                    {
                        _pooled.GetComponent<ParticleSystem>().externalForces.AddInfluence(ForceField);
                    }
                }
        }
    }
}