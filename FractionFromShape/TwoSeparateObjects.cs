/*
using UnityEngine;

public class TwoSeparateObjects : MonoBehaviour
{
    public int numberOfSides;
    public float radius;

    public int numerator;
    private MeshFilter meshFilter1;
    private MeshFilter meshFilter2;
    private MeshRenderer meshRenderer1;
    private MeshRenderer meshRenderer2;

    void Start()
    {
        numerator = Random.Range(1, numberOfSides);
        Vector3[] vertices = new Vector3[numerator];

        for (int i = 0; i < numerator; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices[i] = pos;
        }

        Mesh mesh1 = new Mesh();
        mesh1.name = "Partial Polygon Mesh";
        mesh1.vertices = vertices;

        int numTriangles = (numerator - 2);
        int[] triangles = new int[numTriangles * 3];

        for (int i = 0; i < numTriangles; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh1.SetIndices(triangles, MeshTopology.Triangles, 0);

        mesh1.RecalculateNormals();
        mesh1.RecalculateBounds();

        meshFilter1 = gameObject.AddComponent<MeshFilter>();
        meshFilter1.sharedMesh = mesh1;

        meshRenderer1 = gameObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer1.sharedMaterial = material;

        Vector3[] vertices2 = new Vector3[numerator];

        for (int i = 0; i < numerator; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices2[i] = pos;
        }

        Mesh mesh2 = new Mesh();
        mesh2.name = "Full Polygon Mesh";
        mesh2.vertices = vertices2;

        int numTriangles2 = (numerator - 2);
        int[] triangles2 = new int[numTriangles2 * 3];

        for (int i = 0; i < numTriangles2; i++)
        {
            triangles2[i * 3] = 0;
            triangles2[i * 3 + 1] = i + 1;
            triangles2[i * 3 + 2] = i + 2;
        }

        mesh2.SetIndices(triangles2, MeshTopology.Triangles, 0);

        mesh2.RecalculateNormals();
        mesh2.RecalculateBounds();

        GameObject secondObject = new GameObject("Second Object");
        meshFilter2 = secondObject.AddComponent<MeshFilter>();
        meshFilter2.sharedMesh = mesh2;

        meshRenderer2 = secondObject.AddComponent<MeshRenderer>();
        Material material2 = new Material(Shader.Find("Sprites/Default"));
        meshRenderer2.sharedMaterial = material2;
    }
}

using UnityEngine;

public class TwoSeparateObjects : MonoBehaviour
{
    public int numberOfSides;
    public float radius;

    public int numerator;
    private MeshFilter meshFilter;
    private Color32[] colors;

    void Start()
    {
        numerator = Random.Range(1, numberOfSides);
        Vector3[] vertices = new Vector3[numberOfSides];

        for (int i = 0; i < numberOfSides; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices[i] = pos;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Polygon Mesh";
        mesh.vertices = vertices;

        // Create a triangle list to fill the polygon
        int[] triangles = new int[(numberOfSides - 2) * 3];
        for (int i = 0; i < (numberOfSides - 2); i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sharedMaterial = material;
        PartialPolygonGenerator();
    }

    void PartialPolygonGenerator()
    {
        Vector3[] vertices = new Vector3[numerator];

        for (int i = 0; i < numerator; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices[i] = pos;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Partial Polygon Mesh";
        mesh.vertices = vertices;

        int numTriangles = (numerator - 2);
        int[] triangles = new int[numTriangles * 3];

        for (int i = 0; i < numTriangles; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sharedMaterial = material;

        colors = new Color32[numerator];

        // Set the color for each vertex
        for (int i = 0; i < numerator; i++)
        {
            colors[i] = Color.
        }

        mesh.colors32 = colors;
    }
}
*/

/* USE THIS OR THE NEW ONE
    using UnityEngine;

    public class TwoSeparateObjects : MonoBehaviour
    {
        public int numberOfSides;
        public float radius;

        public int numerator;
        private MeshFilter meshFilter;
        private Color32[] colors;


        void Start()
        {
            numberOfSides = Random.Range(3, 9);
            numerator = Random.Range(1, numberOfSides);
            Vector3[] vertices = new Vector3[numberOfSides];

            for (int i = 0; i < numberOfSides; i++)
            {
                float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
                Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
                vertices[i] = pos;
            }

            Mesh mesh = new Mesh();
            mesh.name = "Polygon Mesh";
            mesh.vertices = vertices;

            // Create a triangle list to fill the polygon
            int[] triangles = new int[(numberOfSides - 2) * 3];
            for (int i = 0; i < (numberOfSides - 2); i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            Material material = new Material(Shader.Find("Sprites/Default"));
            meshRenderer.sharedMaterial = material;

            colors = new Color32[numberOfSides];

            // Set the color for each vertex
            for (int i = 0; i < numberOfSides; i++)
            {
                // Set the color for the first three triangles
                if (i < numerator)
                {
                    colors[i] = Color.red;
                }
                else
                {
                    colors[i] = Color.white;
                }
            }

            mesh.colors32 = colors;
        }
}
*/
/*
using UnityEngine;
using System.Linq;

public class PolygonGenerator : MonoBehaviour
{
    public int numberOfSides;
    public float radius;

    private MeshFilter meshFilter;

    void Start()
    {
        Vector3[] vertices = new Vector3[numberOfSides];

        for (int i = 0; i < numberOfSides; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices[i] = pos;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Polygon Mesh";
        mesh.vertices = vertices;

        mesh.SetIndices(Enumerable.Range(0, numberOfSides).ToArray(), MeshTopology.LineStrip, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard"));
        meshRenderer.sharedMaterial = material;
    }
}
*/

using UnityEngine;

public class TwoSeparateObjects : MonoBehaviour
{
    public int numberOfSides;
    public float radius;
    public float spawnX; // Variable for x spawn position
    public float spawnY; // Variable for y spawn position

    public int numerator;
    private MeshFilter meshFilter;
    private Color32[] colors;
    public Color32 calor = new Color32(255, 112, 67, 255);

    void Start()
    {
        numberOfSides = Random.Range(3, 9);
        numerator = Random.Range(1, numberOfSides);
        Vector3[] vertices = new Vector3[numberOfSides];

        for (int i = 0; i < numberOfSides; i++)
        {
            float radians = (Mathf.PI / 2) + i * ((2 * Mathf.PI) / numberOfSides);
            Vector3 pos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            vertices[i] = pos;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Polygon Mesh";
        mesh.vertices = vertices;

        // Create a triangle list to fill the polygon
        int[] triangles = new int[(numberOfSides - 2) * 3];
        for (int i = 0; i < (numberOfSides - 2); i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.sharedMaterial = material;

        colors = new Color32[numberOfSides];

        // Set the color for each vertex
        for (int i = 0; i < numberOfSides; i++)
        {
            // Set the color for the first three triangles
            if (i < numerator)
            {
                colors[i] = calor;
            }
            else
            {
                colors[i] = Color.white;
            }
        }

        mesh.colors32 = colors;

        // Set the spawn position based on spawnX and spawnY variables
        transform.position = new Vector3(spawnX, spawnY, 0f);
    }
}
