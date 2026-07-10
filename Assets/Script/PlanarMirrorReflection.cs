using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// A mettre sur CHAQUE objet retroviseur (le mesh qui affiche le reflet).
// Le mesh doit etre a peu plat (un quad/plan) et orienter sa normale
// selon l'axe choisi ci-dessous (verifie dans la Scene view avec Gizmos).
[ExecuteAlways]
[DisallowMultipleComponent]
public class PlanarMirrorReflection : MonoBehaviour
{
    public enum MirrorAxis { Forward, Back, Up, Down, Right, Left }

    [Header("Reflection setup")]
    [Tooltip("Axe local du mesh qui pointe vers l'exterieur du miroir (vers ce qu'il doit reflechir)")]
    public MirrorAxis mirrorNormalAxis = MirrorAxis.Forward;

    [Tooltip("Camera principale du joueur (si vide, prend Camera.main)")]
    public Camera playerCamera;

    [Tooltip("Layers a inclure dans le reflet (exclure le layer du miroir lui-meme !)")]
    public LayerMask reflectLayers = ~0;

    [Header("Qualite / perfs")]
    [Tooltip("Resolution de la texture de reflet. 512 pour les retros principaux, 256 pour les grands-angles")]
    public int textureSize = 512;

    [Tooltip("Ne recalculer le reflet qu'une frame sur N (1 = chaque frame)")]
    [Range(1, 6)]
    public int refreshEveryNFrames = 2;

    [Tooltip("Decalage du plan de clip pour eviter les artefacts au bord du miroir")]
    public float clipPlaneOffset = 0.02f;

    private Camera reflectionCamera;
    private RenderTexture reflectionTexture;
    private Renderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;
    private int frameCounter;
    private static readonly int MainTexId = Shader.PropertyToID("_BaseMap");
    private static readonly int UnlitTexId = Shader.PropertyToID("_MainTex");

    void OnEnable()
    {
        meshRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (playerCamera == null) playerCamera = Camera.main;

        CreateReflectionCameraAndTexture();
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        CleanUp();
    }

    void CreateReflectionCameraAndTexture()
    {
        CleanUp();

        reflectionTexture = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.Default)
        {
            name = $"ReflectionRT_{gameObject.name}",
            antiAliasing = 1,
            useMipMap = false
        };

        GameObject camGO = new GameObject($"ReflectionCam_{gameObject.name}")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        reflectionCamera = camGO.AddComponent<Camera>();
        reflectionCamera.enabled = false; // on le rend manuellement
        reflectionCamera.targetTexture = reflectionTexture;
        reflectionCamera.cullingMask = reflectLayers;

        var camData = camGO.AddComponent<UniversalAdditionalCameraData>();
        camData.renderShadows = true;
        camData.requiresColorOption = CameraOverrideOption.Off;
        camData.requiresDepthOption = CameraOverrideOption.Off;
        camData.renderType = CameraRenderType.Base;

        ApplyTextureToMaterial();
    }

    void ApplyTextureToMaterial()
    {
        if (meshRenderer == null) return;
        meshRenderer.GetPropertyBlock(propertyBlock);
        // fonctionne pour Universal Render Pipeline/Unlit (_BaseMap) et Unlit/Texture classique (_MainTex)
        propertyBlock.SetTexture(MainTexId, reflectionTexture);
        propertyBlock.SetTexture(UnlitTexId, reflectionTexture);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (cam != playerCamera) return; // ne reagit qu'au rendu de la camera du joueur
        if (reflectionCamera == null || reflectionTexture == null) return;

        frameCounter++;
        if (frameCounter % refreshEveryNFrames != 0) return;

        UpdateReflectionCameraTransform(cam);

        // clip plane oblique pour ne pas refleter ce qui est derriere le miroir
        Vector3 pos = transform.position;
        Vector3 normal = GetWorldNormal();
        Vector4 clipPlaneCamSpace = CameraSpacePlane(reflectionCamera, pos, normal, clipPlaneOffset);
        reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlaneCamSpace);

        UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
    }

    Vector3 GetWorldNormal()
    {
        switch (mirrorNormalAxis)
        {
            case MirrorAxis.Forward: return transform.forward;
            case MirrorAxis.Back: return -transform.forward;
            case MirrorAxis.Up: return transform.up;
            case MirrorAxis.Down: return -transform.up;
            case MirrorAxis.Right: return transform.right;
            case MirrorAxis.Left: return -transform.right;
        }
        return transform.forward;
    }

    void UpdateReflectionCameraTransform(Camera sourceCam)
    {
        Vector3 pos = transform.position;
        Vector3 normal = GetWorldNormal();

        // matrice de reflexion par rapport au plan du miroir
        float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
        Matrix4x4 reflectionMatrix = CalculateReflectionMatrix(reflectionPlane);

        reflectionCamera.worldToCameraMatrix = sourceCam.worldToCameraMatrix * reflectionMatrix;
        reflectionCamera.fieldOfView = sourceCam.fieldOfView;
        reflectionCamera.nearClipPlane = sourceCam.nearClipPlane;
        reflectionCamera.farClipPlane = sourceCam.farClipPlane;
        reflectionCamera.aspect = sourceCam.aspect;

        // position/rotation "reelles" (utilisees pour le culling / oblique matrix)
        Vector3 reflectedPos = reflectionMatrix.MultiplyPoint(sourceCam.transform.position);
        reflectionCamera.transform.position = reflectedPos;

        Vector3 forward = reflectionMatrix.MultiplyVector(sourceCam.transform.forward);
        Vector3 up = reflectionMatrix.MultiplyVector(sourceCam.transform.up);
        reflectionCamera.transform.rotation = Quaternion.LookRotation(forward, up);
    }

    static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = 1f - 2f * plane.x * plane.x;
        m.m01 = -2f * plane.x * plane.y;
        m.m02 = -2f * plane.x * plane.z;
        m.m03 = -2f * plane.x * plane.w;

        m.m10 = -2f * plane.y * plane.x;
        m.m11 = 1f - 2f * plane.y * plane.y;
        m.m12 = -2f * plane.y * plane.z;
        m.m13 = -2f * plane.y * plane.w;

        m.m20 = -2f * plane.z * plane.x;
        m.m21 = -2f * plane.z * plane.y;
        m.m22 = 1f - 2f * plane.z * plane.z;
        m.m23 = -2f * plane.z * plane.w;

        m.m30 = 0f; m.m31 = 0f; m.m32 = 0f; m.m33 = 1f;
        return m;
    }

    static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideOffset)
    {
        Vector3 offsetPos = pos + normal * sideOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    void CleanUp()
    {
        if (reflectionCamera != null)
        {
            if (Application.isPlaying) Destroy(reflectionCamera.gameObject);
            else DestroyImmediate(reflectionCamera.gameObject);
            reflectionCamera = null;
        }
        if (reflectionTexture != null)
        {
            reflectionTexture.Release();
            if (Application.isPlaying) Destroy(reflectionTexture);
            else DestroyImmediate(reflectionTexture);
            reflectionTexture = null;
        }
    }
}