using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Range(0,3)] public int NoiseLevel;
    public float Noise => NoiseLevel;

    [Range(0, 40)] public int Range;

    private void Start()
    {
        MaskRenderer.RegisterEntity(this);
    }
}
