using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MeshIslandSplitterWindow
{
    [MenuItem("Tools/Split Mesh By Islands")]
    static void SplitSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Erreur", "Selectionne d'abord l'objet contenant le mesh combine.", "OK");
            return;
        }

        MeshFilter mf = selected.GetComponent<MeshFilter>();
        MeshRenderer mr = selected.GetComponent<MeshRenderer>();
        if (mf == null || mf.sharedMesh == null || mr == null)
        {
            EditorUtility.DisplayDialog("Erreur", "L'objet selectionne doit avoir un MeshFilter (avec un mesh) et un MeshRenderer.", "OK");
            return;
        }

        Mesh sourceMesh = mf.sharedMesh;
        Material sourceMaterial = mr.sharedMaterial;

        Vector3[] verts = sourceMesh.vertices;
        Vector3[] normals = sourceMesh.normals;
        Vector2[] uvs = sourceMesh.uv;
        int[] tris = sourceMesh.triangles;

        // Regroupe les positions identiques (a epsilon pres) sous un meme id canonique
        var posToCanon = new Dictionary<Vector3Int, int>();
        int[] canon = new int[verts.Length];
        const float precision = 10000f; // ~0.0001 unite de tolerance

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 p = verts[i];
            Vector3Int key = new Vector3Int(
                Mathf.RoundToInt(p.x * precision),
                Mathf.RoundToInt(p.y * precision),
                Mathf.RoundToInt(p.z * precision));

            if (!posToCanon.TryGetValue(key, out int id))
            {
                id = posToCanon.Count;
                posToCanon[key] = id;
            }
            canon[i] = id;
        }

        // Union-Find sur les ids canoniques, unis via les triangles
        int[] parent = new int[posToCanon.Count];
        for (int i = 0; i < parent.Length; i++) parent[i] = i;

        int Find(int x) { while (parent[x] != x) { parent[x] = parent[parent[x]]; x = parent[x]; } return x; }
        void Union(int a, int b) { int ra = Find(a), rb = Find(b); if (ra != rb) parent[ra] = rb; }

        for (int t = 0; t < tris.Length; t += 3)
        {
            int a = canon[tris[t]];
            int b = canon[tris[t + 1]];
            int c = canon[tris[t + 2]];
            Union(a, b);
            Union(b, c);
        }

        // Regroupe les triangles par racine d'union-find
        var trianglesByIsland = new Dictionary<int, List<int>>();
        for (int t = 0; t < tris.Length; t += 3)
        {
            int root = Find(canon[tris[t]]);
            if (!trianglesByIsland.TryGetValue(root, out var list))
            {
                list = new List<int>();
                trianglesByIsland[root] = list;
            }
            list.Add(tris[t]);
            list.Add(tris[t + 1]);
            list.Add(tris[t + 2]);
        }

        Undo.RegisterFullObjectHierarchyUndo(selected, "Split Mesh By Islands");

        int islandIndex = 0;
        foreach (var kvp in trianglesByIsland)
        {
            List<int> islandTris = kvp.Value;

            // Remappe vertex indices utilises dans cet ilot
            var remap = new Dictionary<int, int>();
            var newVerts = new List<Vector3>();
            var newNormals = new List<Vector3>();
            var newUvs = new List<Vector2>();
            var newTris = new List<int>();

            foreach (int origIndex in islandTris)
            {
                if (!remap.TryGetValue(origIndex, out int newIndex))
                {
                    newIndex = newVerts.Count;
                    remap[origIndex] = newIndex;
                    newVerts.Add(verts[origIndex]);
                    if (normals.Length > 0) newNormals.Add(normals[origIndex]);
                    if (uvs.Length > 0) newUvs.Add(uvs[origIndex]);
                }
                newTris.Add(newIndex);
            }

            Mesh islandMesh = new Mesh { name = $"{sourceMesh.name}_Island{islandIndex}" };
            islandMesh.SetVertices(newVerts);
            if (newNormals.Count > 0) islandMesh.SetNormals(newNormals);
            if (newUvs.Count > 0) islandMesh.SetUVs(0, newUvs);
            islandMesh.SetTriangles(newTris, 0);
            islandMesh.RecalculateBounds();
            if (newNormals.Count == 0) islandMesh.RecalculateNormals();

            GameObject child = new GameObject($"MirrorIsland_{islandIndex}");
            Undo.RegisterCreatedObjectUndo(child, "Split Mesh By Islands");
            child.transform.SetParent(selected.transform, false);

            var childMf = child.AddComponent<MeshFilter>();
            childMf.sharedMesh = islandMesh;

            var childMr = child.AddComponent<MeshRenderer>();
            childMr.sharedMaterial = new Material(sourceMaterial) { name = $"mirror_island_{islandIndex}" };

            islandIndex++;
        }

        // Desactive (ne supprime pas) le renderer d'origine pour eviter le double affichage
        mr.enabled = false;

        Debug.Log($"Split termine : {islandIndex} ilots crees sous {selected.name}. " +
                  "Le MeshRenderer d'origine a ete desactive (pas supprime).");
    }
}