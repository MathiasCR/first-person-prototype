using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ISlot
{
    Image Icon { get; }
    GameObject Stack { get; }
    Sprite DefautSlotIcon { get; }
    TextMeshProUGUI StackNbr { get; }

    void UpdateIconImage();
}
