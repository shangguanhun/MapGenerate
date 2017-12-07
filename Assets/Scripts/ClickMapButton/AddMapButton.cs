using UnityEngine;

public class AddMapButton: MonoBehaviour
{
    private GameObject buttonObject;
    void Start()
    {
        buttonObject = Resources.Load("BaseMeshObject") as GameObject;
        foreach (ProvinceNode pn in XmlLoader.GetAllProvinceNode())
        {
            GameObject go= GameObject.Instantiate(buttonObject);
            go.name = pn.provinceName;
            SerializeMesh.GetMeshByName(go.GetComponent<MeshFilter>().mesh, pn.meshSrc);
            if (pn.offset == -1)
            {
                go.transform.localScale = new Vector3(1, -1, 1);
            }
            go.transform.parent = transform;
            go.AddComponent<MeshCollider>();
        }

        transform.position = new Vector3(-500, 0, -10);
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        transform.localScale = new Vector3(1, 1, -1);
    }
}