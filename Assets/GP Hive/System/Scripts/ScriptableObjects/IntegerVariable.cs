using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Variables/Integer Variable", fileName = "Integer Variable")]
public class IntegerVariable : ScriptableObject
{
    public int Value;
    public GameEvent OnChangeEvent;
}