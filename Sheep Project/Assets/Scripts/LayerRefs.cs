using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>

public class LayerRefs : MonoBehaviour
{
    public static LayerRefs lR;

    public LayerMask blocMask;
    public LayerMask sheepMask;
    public LayerMask slopeMask;

    private void Awake()
    {
        lR = this;
    }
}
