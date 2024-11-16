using UnityEngine;

public static class Tools
{
    public static bool IsBetween(float first, float second, float floatToCheck)
    {
        return floatToCheck >= Mathf.Min(first, second) && floatToCheck <= Mathf.Max(first, second);
    }
}
