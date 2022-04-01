using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Range(0,3)] public int NoiseLevel;
    public float Noise { get; private set; }

    [Range(0, 40)] public int Range;

    private void Awake()
    {
        MaskRenderer.RegisterEntity(this);
    }
}
