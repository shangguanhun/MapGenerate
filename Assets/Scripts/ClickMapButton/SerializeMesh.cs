using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class SerializeMesh
{
    private static string _meshSrc = "Assets/Resources/mesh/";
    public static void MeshToFile(MeshFilter mf, string filename, float scale)
    {
        using (StreamWriter streamWriter = new StreamWriter(_meshSrc + filename))
        {
            streamWriter.Write(MeshToString(mf, scale));
            streamWriter.Flush();
        }
    }

    public static string MeshToString(MeshFilter mf, float scale)
    {
        Mesh mesh = mf.mesh;
        Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
        Vector2 textureOffset = mf.GetComponent<Renderer>().material.GetTextureOffset("_MainTex");
        Vector2 textureScale = mf.GetComponent<Renderer>().material.GetTextureScale("_MainTex");
        StringBuilder stringBuilder = new StringBuilder();
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vector = vertices[i];
            stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale, vector.z * scale, vector.y * scale));
        }
        
        for (int k = 0; k < mesh.subMeshCount; k++)
        {
            int[] triangles = mesh.GetTriangles(k);
            for (int l = 0; l < triangles.Length; l += 3)
            {
                stringBuilder.Append(string.Format("t {0} {1} {2}\n", triangles[l], triangles[l + 1], triangles[l + 2]));
            }
        }
        stringBuilder.Append("\n");
        return stringBuilder.ToString();
    }

    public static void GetMeshByName(Mesh mesh, string meshName)
    {
        using (StreamReader streamReader = new StreamReader(_meshSrc + meshName))
        {
            string str = null;
            string []strs = null;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            str = streamReader.ReadLine();
            while (str != null)
            {
                strs = str.Split(' ');
                if (strs[0] == 'v'.ToString())
                {
                    vertices.Add(new Vector3(float.Parse(strs[1]), float.Parse(strs[2]), float.Parse(strs[3])));
                }
                if(strs[0] == 't'.ToString())
                {
                    triangles.Add(int.Parse(strs[1]));
                    triangles.Add(int.Parse(strs[2]));
                    triangles.Add(int.Parse(strs[3]));
                }
                str = streamReader.ReadLine();
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }
    }
}