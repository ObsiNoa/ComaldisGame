using UnityEngine;

[ExecuteInEditMode]
public class PlanarMirrorReflection : MonoBehaviour
{
    public enum Axis
    {
        Forward,
        Back,
        Left,
        Right,
        Up,
        Down
    }

    [Header("Reflection setup")]
    public bool useMeshNormal = false;
    public bool invertNormal = false;
    public Axis mirrorNormalAxis = Axis.Forward;
    public Camera playerCamera;
    public LayerMask reflectLayers = -1;

    [Header("Qualité / Perfs")]
    public int textureSize = 512;
    [Range(1, 10)]
    public int refreshEveryNFrames = 1;
    public float clipPlaneOffset = 0.02f;

    private RenderTexture m_ReflectionTexture;
    private Camera m_ReflectionCamera;
    private MaterialPropertyBlock m_PropertyBlock;
    private int m_FrameCounter = 0;
    private static bool s_InsideRendering = false;

    private void OnDisable()
    {
        CleanupResources();
    }

    private void OnDestroy()
    {
        CleanupResources();
    }

    private void CleanupResources()
    {
        if (m_ReflectionTexture != null)
        {
            if (Application.isPlaying)
                Destroy(m_ReflectionTexture);
            else
                DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }

        if (m_ReflectionCamera != null)
        {
            if (Application.isPlaying)
                Destroy(m_ReflectionCamera.gameObject);
            else
                DestroyImmediate(m_ReflectionCamera.gameObject);
            m_ReflectionCamera = null;
        }
    }

    /// <summary>
    /// Calcule le centre géométrique réel de la surface du miroir 
    /// même si le pivot du modèle 3D est situé au centre du pare-brise.
    /// </summary>
    public Vector3 GetMirrorCenter()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            return rend.bounds.center;
        }
        return transform.position;
    }

    private void LateUpdate()
    {
        if (!enabled || s_InsideRendering)
            return;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            return;

        m_FrameCounter++;
        if (refreshEveryNFrames > 1 && (m_FrameCounter % refreshEveryNFrames != 0))
            return;

        RenderReflection();
    }

    private void RenderReflection()
    {
        s_InsideRendering = true;

        EnsureResources();

        // Utilisation du centre réel du miroir au lieu du pivot (pare-brise)
        Vector3 pos = GetMirrorCenter();
        Vector3 normal = GetNormal();

        // Mise à jour de la caméra de réflexion
        UpdateCameraProperties(playerCamera, m_ReflectionCamera);

        // Calcul du plan de réflexion
        float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        Matrix4x4 reflectionMatrix = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflectionMatrix, reflectionPlane);

        Vector3 oldCamPos = playerCamera.transform.position;
        Vector3 newCamPos = reflectionMatrix.MultiplyPoint(oldCamPos);

        m_ReflectionCamera.worldToCameraMatrix = playerCamera.worldToCameraMatrix * reflectionMatrix;

        // Plan de coupe oblique basé sur le centre réel
        Vector4 clipPlane = CameraSpacePlane(m_ReflectionCamera, pos, normal, 1.0f);
        m_ReflectionCamera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlane);

        m_ReflectionCamera.cullingMask = reflectLayers.value;
        m_ReflectionCamera.transform.position = newCamPos;

        // Rendu avec inversion du culling
        bool oldCull = GL.invertCulling;
        GL.invertCulling = true;

        m_ReflectionCamera.Render();

        GL.invertCulling = oldCull;

        // Application de la texture sur le matériau d'instance
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.GetPropertyBlock(m_PropertyBlock);
            m_PropertyBlock.SetTexture("_ReflectionTex", m_ReflectionTexture);
            m_PropertyBlock.SetTexture("_MainTex", m_ReflectionTexture);
            rend.SetPropertyBlock(m_PropertyBlock);
        }

        s_InsideRendering = false;
    }

    public Vector3 GetNormal()
    {
        Vector3 normal = Vector3.forward;

        if (useMeshNormal)
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null && mf.sharedMesh.normals.Length > 0)
            {
                normal = transform.TransformDirection(mf.sharedMesh.normals[0]);
            }
            else
            {
                normal = transform.forward;
            }
        }
        else
        {
            switch (mirrorNormalAxis)
            {
                case Axis.Forward: normal = transform.forward; break;
                case Axis.Back: normal = -transform.forward; break;
                case Axis.Left: normal = -transform.right; break;
                case Axis.Right: normal = transform.right; break;
                case Axis.Up: normal = transform.up; break;
                case Axis.Down: normal = -transform.up; break;
            }
        }

        if (invertNormal)
        {
            normal = -normal;
        }

        return normal.normalized;
    }

    private void EnsureResources()
    {
        if (m_PropertyBlock == null)
        {
            m_PropertyBlock = new MaterialPropertyBlock();
        }

        if (m_ReflectionTexture == null || m_ReflectionTexture.width != textureSize || m_ReflectionTexture.height != textureSize)
        {
            if (m_ReflectionTexture != null)
            {
                if (Application.isPlaying) Destroy(m_ReflectionTexture);
                else DestroyImmediate(m_ReflectionTexture);
            }

            m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.ARGB32)
            {
                name = "__MirrorReflection_" + GetInstanceID(),
                isPowerOfTwo = true,
                hideFlags = HideFlags.DontSave
            };
        }

        if (m_ReflectionCamera == null)
        {
            GameObject go = new GameObject("MirrorReflectionCamera_" + GetInstanceID(), typeof(Camera), typeof(Skybox))
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            m_ReflectionCamera = go.GetComponent<Camera>();
            m_ReflectionCamera.enabled = false;
        }
    }

    private void UpdateCameraProperties(Camera src, Camera dest)
    {
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
        dest.targetTexture = m_ReflectionTexture;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMatrix, Vector4 plane)
    {
        reflectionMatrix.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMatrix.m01 = (-2F * plane[0] * plane[1]);
        reflectionMatrix.m02 = (-2F * plane[0] * plane[2]);
        reflectionMatrix.m03 = (-2F * plane[3] * plane[0]);

        reflectionMatrix.m10 = (-2F * plane[1] * plane[0]);
        reflectionMatrix.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMatrix.m12 = (-2F * plane[1] * plane[2]);
        reflectionMatrix.m13 = (-2F * plane[3] * plane[1]);

        reflectionMatrix.m20 = (-2F * plane[2] * plane[0]);
        reflectionMatrix.m21 = (-2F * plane[2] * plane[1]);
        reflectionMatrix.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMatrix.m23 = (-2F * plane[3] * plane[2]);

        reflectionMatrix.m30 = 0F;
        reflectionMatrix.m31 = 0F;
        reflectionMatrix.m32 = 0F;
        reflectionMatrix.m33 = 1F;
    }

    private void OnDrawGizmosSelected()
    {
        // Dessine désormais la ligne verte depuis le centre du miroir
        Vector3 pos = GetMirrorCenter();
        Vector3 normal = GetNormal();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + normal * 1.5f);
        Gizmos.DrawWireSphere(pos + normal * 1.5f, 0.08f);
    }
}