namespace MajorsilenceTypes;

public class UserId
{
    private readonly int internalValue = -1;

    public UserId(object value)
        : this(value.ToString())
    {
    }

    public UserId(string value)
    {
        internalValue = int.Parse(value);
    }

    public UserId(int value)
    {
        if (value < 0) throw new LengthException("UserId length must not be less then 0.");
        internalValue = value;
    }

    public int Value()
    {
        return internalValue;
    }

    public override string ToString()
    {
        return internalValue.ToString();
    }

    public bool Equals(UserId c1)
    {
        if (c1 == null) return false;
        return ToString() == c1.ToString();
    }
}