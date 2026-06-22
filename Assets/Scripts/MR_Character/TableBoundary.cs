using UnityEngine;

public class TableBoundary : MonoBehaviour
{
    [SerializeField] MeshRenderer m_TableSurface;
    [SerializeField] float m_WallHeight = 0.5f;
    [SerializeField] float m_WallThickness = 0.05f;

    private GameObject[] m_Walls = new GameObject[4];
    private const string BOUNDARY_HOLDER_NAME = "TableBoundaries";

    void Awake()
    {
        GenerateBoundaries();
    }

    [ContextMenu("Generate Boundaries")]
    public void GenerateBoundaries()
    {
        if (m_TableSurface == null)
        {
            Debug.LogWarning("Table Surface MeshRenderer is not assigned!");
            return;
        }

        // Find or create holder
        Transform holder = transform.Find(BOUNDARY_HOLDER_NAME);
        if (holder != null)
        {
            if (Application.isPlaying)
            {
                // In play mode we just use it
            }
            else
            {
                // In editor, destroy old ones to refresh
                DestroyImmediate(holder.gameObject);
                holder = null;
            }
        }

        if (holder == null)
        {
            GameObject holderObj = new GameObject(BOUNDARY_HOLDER_NAME);
            holder = holderObj.transform;
            holder.SetParent(transform);
            holder.localPosition = Vector3.zero;
            holder.localRotation = Quaternion.identity;
            holder.localScale = Vector3.one;
        }

        Bounds bounds = m_TableSurface.localBounds;
        Vector3 scale = m_TableSurface.transform.localScale;
        
        // Calculate actual dimensions in local space of this (TableTop)
        // Since Plane is a child of TableTop and TableTop has scale (1,1,1)
        // We use the Plane's local scale * bounds
        float width = bounds.size.x * scale.x;
        float depth = bounds.size.z * scale.z;
        float centerY = bounds.center.y * scale.y;

        // North (Z+)
        SetupWall(holder, "Wall_North", new Vector3(0, centerY + m_WallHeight / 2f, depth / 2f), new Vector3(width, m_WallHeight, m_WallThickness));
        // South (Z-)
        SetupWall(holder, "Wall_South", new Vector3(0, centerY + m_WallHeight / 2f, -depth / 2f), new Vector3(width, m_WallHeight, m_WallThickness));
        // East (X+)
        SetupWall(holder, "Wall_East", new Vector3(width / 2f, centerY + m_WallHeight / 2f, 0), new Vector3(m_WallThickness, m_WallHeight, depth));
        // West (X-)
        SetupWall(holder, "Wall_West", new Vector3(-width / 2f, centerY + m_WallHeight / 2f, 0), new Vector3(m_WallThickness, m_WallHeight, depth));
    }

    private void SetupWall(Transform parent, string name, Vector3 localPos, Vector3 size)
    {
        Transform wallTrans = parent.Find(name);
        GameObject wall;
        if (wallTrans == null)
        {
            wall = new GameObject(name);
            wall.transform.SetParent(parent);
        }
        else
        {
            wall = wallTrans.gameObject;
        }

        wall.transform.localPosition = localPos;
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = Vector3.one; // We set size in the collider

        BoxCollider bc = wall.GetComponent<BoxCollider>();
        if (bc == null) bc = wall.AddComponent<BoxCollider>();
        bc.size = size;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_TableSurface == null) return;
        
        Gizmos.color = Color.green;
        Bounds bounds = m_TableSurface.localBounds;
        Vector3 scale = m_TableSurface.transform.localScale;
        Vector3 size = new Vector3(bounds.size.x * scale.x, m_WallHeight, bounds.size.z * scale.z);
        Vector3 center = m_TableSurface.transform.localPosition + new Vector3(0, m_WallHeight / 2f, 0);
        
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(center, size);
    }
}
