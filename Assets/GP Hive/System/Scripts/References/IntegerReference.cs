[System.Serializable]
public class IntegerReference
{
    public bool UseConstant;
    public int ConstantValue;
    public IntegerVariable Variable;

    public int Value
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