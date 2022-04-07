using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskRenderer : MonoBehaviour
{
    private static List<Entity> entities;

    public static void RegisterEntity(Entity entity) { entities.Add(entity); }

    //Properties
    [SerializeField] private ComputeShader compute = null;

    [Range(64, 4096)] [SerializeField] public int TextureSize = 2048;
    [SerializeField] private float mapSize = 0;
    public float MapSize => mapSize;

    [SerializeField] private float BlendDistance = 4.0f;

    public Color MaskColor0;
    public Color MaskColor1;
    public Color MaskColor2;
    public Color MaskColor3;
    public Color MaskColor4;
    public Color MaskColor5;
    public Color MaskColor6;
    public Color MaskColor7;
    public Color MaskColor8;
    public Color MaskColor9;
    public Color MaskColor10;

    public RenderTexture maskTexture;

    //Shader properties cache
    private static readonly int textureSizeId = Shader.PropertyToID("_TextureSize");
    private static readonly int entityCountId = Shader.PropertyToID("_EntityCount");
    private static readonly int mapSizeId = Shader.PropertyToID("_MapSize");
    private static readonly int blendId = Shader.PropertyToID("_Blend");

    private static readonly int color0Id = Shader.PropertyToID("_Color0");
    private static readonly int color1Id = Shader.PropertyToID("_Color1");
    private static readonly int color2Id = Shader.PropertyToID("_Color2");
    private static readonly int color3Id = Shader.PropertyToID("_Color3");
    private static readonly int color4Id = Shader.PropertyToID("_Color4");
    private static readonly int color5Id = Shader.PropertyToID("_Color5");
    private static readonly int color6Id = Shader.PropertyToID("_Color6");
    private static readonly int color7Id = Shader.PropertyToID("_Color7");
    private static readonly int color8Id = Shader.PropertyToID("_Color8");
    private static readonly int color9Id = Shader.PropertyToID("_Color9");
    private static readonly int color10Id = Shader.PropertyToID("_Color10");

    private static readonly int maskTextureId = Shader.PropertyToID("_Mask");

    private static readonly int entityBufferId = Shader.PropertyToID("_EntityBuffer");

    //Entity data buffer
    private struct EntityBufferElement
    {
        public float PositionX;
        public float PositionY;
        public float Range;
        public float Noise;
    }

    private List<EntityBufferElement> bufferElements;
    private ComputeBuffer buffer = null;

    private void Awake()
    {
        entities = new List<Entity>();

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        maskTexture = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
#else
        maskTexture = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.ARGBFloat)
#endif
        {
            enableRandomWrite = true
        };
        maskTexture.Create();

        compute.SetInt(textureSizeId, TextureSize);
        compute.SetTexture(0, maskTextureId, maskTexture);

        compute.SetFloat(blendId, BlendDistance);

        compute.SetVector(color0Id, MaskColor0);
        compute.SetVector(color1Id, MaskColor1);
        compute.SetVector(color2Id, MaskColor2);
        compute.SetVector(color3Id, MaskColor3);
        compute.SetVector(color4Id, MaskColor4);
        compute.SetVector(color5Id, MaskColor5);
        compute.SetVector(color6Id, MaskColor6);
        compute.SetVector(color7Id, MaskColor7);
        compute.SetVector(color8Id, MaskColor8);
        compute.SetVector(color9Id, MaskColor9);
        compute.SetVector(color10Id, MaskColor10);

        Shader.SetGlobalTexture(maskTextureId, maskTexture);
        Shader.SetGlobalFloat(mapSizeId, mapSize);

        bufferElements = new List<EntityBufferElement>();
    }

    private void OnDestroy()
    {
        buffer?.Dispose();

        if (maskTexture != null)
            DestroyImmediate(maskTexture);
    }

    private void Update()
    {
        bufferElements.Clear();

        foreach(Entity entity in entities)
        {
            EntityBufferElement element = new EntityBufferElement
            {
                PositionX = entity.transform.position.x,
                PositionY = entity.transform.position.z,
                Range = entity.Range,
                Noise = entity.Noise
            };
            bufferElements.Add(element);
        }

        buffer?.Release();
        buffer = new ComputeBuffer(bufferElements.Count * 4, sizeof(float));

        buffer.SetData(bufferElements);
        compute.SetBuffer(0, entityBufferId, buffer);

        compute.SetInt(entityCountId, bufferElements.Count);
        //compute.SetFloat(blendId, BlendDistance / entity.Range);
        //compute.SetFloat(blendId, BlendDistance * entity.Range);

        compute.Dispatch(0, Mathf.CeilToInt(TextureSize / 8.0f), Mathf.CeilToInt(TextureSize / 8.0f), 1);
    }
}
