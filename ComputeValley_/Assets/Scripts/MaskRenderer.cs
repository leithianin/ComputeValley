using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskRenderer : MonoBehaviour
{
    private static List<Entity> entities;

    public static void RegisterEntity(Entity entity) { entities.Add(entity); }

    //Properties
    [SerializeField] private ComputeShader compute = null;

    [Range(64, 4096)] [SerializeField] private int TextureSize = 2048;
    [SerializeField] private float mapSize = 0;
    public float MapSize => mapSize;

    [SerializeField] private float BlendDistance = 4.0f;

    public Color MaskColor0;
    public Color MaskColor1;
    public Color MaskColor2;
    public Color MaskColor3;

    public Texture2D NoiseTexture;
    [Range(0.0f, 5.0f)] public float NoiseDetail = 4.0f;

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

    private static readonly int noiseTexId = Shader.PropertyToID("_NoiseTex");
    private static readonly int noiseDetailId = Shader.PropertyToID("_NoiseDetail");

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

        compute.SetTexture(0, noiseTexId, NoiseTexture);
        compute.SetFloat(noiseDetailId, NoiseDetail);

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

        compute.Dispatch(0, Mathf.CeilToInt(TextureSize / 8.0f), Mathf.CeilToInt(TextureSize / 8.0f), 1);
    }
}
