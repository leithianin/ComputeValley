using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Range(0, 10)] public int NoiseLevel;
    public float Noise => NoiseLevel;

    [HideInInspector] public int Range;
    [Range(0, 40)] public int minRange;

    [Range(2, 10)] public int rangeSpread = 2;

    private void Start()
    {
        MaskRenderer.RegisterEntity(this);
    }

    private void Update()
    {
        UpdateRange();
    }

    public void UpdateRange()
    {
        Range = minRange + ((NoiseLevel - 1) * rangeSpread);
    }
}
