[System.Serializable]
public class BoolReference
{
    public bool UseConstant;
    public bool ConstantValue;
    public BoolVariable Variable;

    public bool Value
    {
        get => UseConstant ? ConstantValue : Variable.Value;
        set
        {
            Variable.Value = value;
            if (Variable.OnChangeEvent != null)
                Variable.OnChangeEvent.Raise();
        }
    }
}