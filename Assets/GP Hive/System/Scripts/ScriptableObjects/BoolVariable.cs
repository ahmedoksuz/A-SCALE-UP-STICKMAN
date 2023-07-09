using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Variables/Bool Variable", fileName = "Bool Variable")]
public class BoolVariable : ScriptableObject
{
    public bool Value;
    public GameEvent OnChangeEvent;
}