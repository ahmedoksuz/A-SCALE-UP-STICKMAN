using System.Collections;
using System.Collections.Generic;
using GPHive.Game;
using TMPro;
using UnityEngine;

public class EnemyDamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshPro;
    [SerializeField] private Animator _animator;
    private static readonly int Play = Animator.StringToHash("Play");


    public void Setup(int damageAmount, Transform referanceEnemyDamageText)
    {
        transform.parent = referanceEnemyDamageText.parent;
        transform.localPosition = referanceEnemyDamageText.transform.localPosition +
                                  new Vector3(Random.Range(-2f, 2f), Random.Range(0, 1f), 0);

        transform.localScale = referanceEnemyDamageText.transform.localScale;
        _textMeshPro.text = "-" + PlayerEconomy.Instance.ConvertToKBM(damageAmount);
        gameObject.SetActive(true);

        _animator.SetTrigger(Play);
        Invoke(nameof(CloseMe), .8f);
    }

    private void CloseMe()
    {
        gameObject.SetActive(false);
    }
}