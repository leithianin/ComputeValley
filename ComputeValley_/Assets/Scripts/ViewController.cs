using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    public Material[] Materials;

    [SerializeField] private MaskRenderer msk;

    private bool isEnabled;

    void Start()
    {
        EnableHeatmapView(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HandleHeatmap();
        }

        foreach (Material m in Materials)
        {
            m.SetFloat("_MapSize", msk.MapSize);
        }
    }

    private void OnApplicationQuit()
    {
        EnableHeatmapView(false);
    }

    public void HandleHeatmap()
    {
        EnableHeatmapView(!isEnabled);
    }

    private void EnableHeatmapView(bool enable)
    {
        isEnabled = enable;

        foreach(Material m in Materials)
        {
            if (enable) m.EnableKeyword("RENDER_HEATMAP");
            else m.DisableKeyword("RENDER_HEATMAP");
        }
    }
}
