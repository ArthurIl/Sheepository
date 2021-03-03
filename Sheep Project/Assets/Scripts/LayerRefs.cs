using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>

public class LayerRefs : MonoBehaviour
{
    public static LayerRefs lR;

    public LayerMask blocMask;
    public LayerMask sheepMask;

    private void Awake()
    {
        lR = this;
    }
}
