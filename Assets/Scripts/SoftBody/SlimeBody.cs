using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(SlimeBody))]
public class SlimeBodyEditor : Editor
{
    private SlimeBody _slimeBody;

    private Mesh _centerMesh;
    private Material _centerMeshMaterial;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _slimeBody = (SlimeBody)target;

        EditorGUILayout.BeginHorizontal();

        //메쉬 필터 세팅
        Mesh mesh = null;        
        var meshGoMeshFilter = _slimeBody.MeshGo == null ? null : _slimeBody.MeshGo.GetComponent<MeshFilter>();
        if (meshGoMeshFilter != null)
        {
            mesh = meshGoMeshFilter.sharedMesh;
            _centerMesh = null;
        }
        else
        {
            mesh = _centerMesh;
        }

        _centerMesh = EditorGUILayout.ObjectField(mesh, typeof(Mesh), false, GUILayout.Width(128), GUILayout.Height(128)) as Mesh;

        //메쉬 렌더러 세팅
        Material meshMat = null;        
        var meshGoMeshRenderer = _slimeBody.MeshGo == null ? null : _slimeBody.MeshGo.GetComponent<MeshRenderer>();
        if (meshGoMeshRenderer)
        {
            meshMat = meshGoMeshRenderer.sharedMaterial;
            _centerMeshMaterial = null;
        }
        else
        {
            meshMat = _centerMeshMaterial;
        }

        _centerMeshMaterial = EditorGUILayout.ObjectField(meshMat, typeof(Material), false, GUILayout.Width(128), GUILayout.Height(128)) as Material;

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Set SlimeBody"))
        {   
            SetMesh();    
            SetPoints();
        }
        

    }

    private void SetMesh()
    {
        var meshGo = new GameObject("Mesh");
        meshGo.transform.SetParent(_slimeBody.transform);
        meshGo.transform.localPosition = Vector3.zero;

        var mf = meshGo.AddComponent<MeshFilter>();
        mf.mesh = _centerMesh;

        var mr = meshGo.AddComponent<MeshRenderer>();
        mr.material = _centerMeshMaterial;

        _slimeBody.MeshGo = meshGo;
        _slimeBody.MeshFilter = mf;
    }

    /// <summary>
    /// 슬라임 바디 자동 세팅
    /// </summary>
    private void SetPoints()
    {
        //바디 중심 세팅
        var bodyCenter = new GameObject("Center");
        bodyCenter.transform.SetParent(_slimeBody.transform);
        bodyCenter.transform.localPosition = Vector3.zero;
        bodyCenter.AddComponent<SpriteRenderer>();

        var bodyCenterRigidBody = bodyCenter.AddComponent<Rigidbody2D>();

        _slimeBody.BodyCenter = bodyCenter;

        //각 모서리 생성
        _slimeBody.Points.Clear();
        var mesh = _slimeBody.MeshFilter.sharedMesh;
        var targetPoint = mesh.vertexCount;
        for(var i = 0; i < targetPoint; ++i)
        {
            var point = new GameObject($"Point_{i}");

            point.transform.SetParent(bodyCenter.transform);
            point.transform.localPosition = mesh.vertices[i];

            point.AddComponent<SpriteRenderer>();
            //TODO - 이미지 세팅 필요

            point.AddComponent<CircleCollider2D>();
            point.AddComponent<Rigidbody2D>();

            _slimeBody.Points.Add(point);
        }

        //각 모서리 세팅
        for (var i = 0; i < targetPoint; ++i)
        {
            var point = _slimeBody.Points[i];
            var leftPoint = _slimeBody.Points[(i - 1 + targetPoint) % targetPoint];
            var rightPoint = _slimeBody.Points[(i + 1) % targetPoint];

            var dj = point.AddComponent<DistanceJoint2D>();
            dj.connectedBody = bodyCenterRigidBody;

            var hj = point.AddComponent<HingeJoint2D>();
            hj.connectedBody = bodyCenterRigidBody;
            hj.useLimits = true;
            
            var sj1 = point.AddComponent<SliderJoint2D>();
            sj1.connectedBody = leftPoint.GetComponent<Rigidbody2D>();

            var sj2 = point.AddComponent<SliderJoint2D>();
            sj2.connectedBody = bodyCenterRigidBody;

            var sj3 = point.AddComponent<SliderJoint2D>();
            sj3.connectedBody = rightPoint.GetComponent<Rigidbody2D>();
        }
    }
}
#endif

public class SlimeBody : MonoBehaviour
{
    [SerializeField]
    private GameObject _bodyCenter;
    public GameObject BodyCenter 
    { 
        get => _bodyCenter; 
        set => _bodyCenter = value; 
    }

    [SerializeField]
    private GameObject _meshGo;
    public GameObject MeshGo
    {
        get => _meshGo;
        set => _meshGo = value;
    }

    [SerializeField]
    private List<GameObject> _points = new();
    public List<GameObject> Points => _points;

    [SerializeField]
    private MeshFilter _meshFilter;
    public MeshFilter MeshFilter
    {
        get => _meshFilter;
        set => _meshFilter = value;
    }

    private Mesh _mesh;
    private Vector3[] _vertices;

    private void Awake()
    {
        if (_meshFilter != null)
        {
            _mesh = _meshFilter.mesh;
            _vertices = new Vector3[_mesh.vertexCount];
        }        
    }

    private void Update()
    {
        if (_mesh == null) 
            return;

        _meshGo.transform.localPosition = _bodyCenter.transform.localPosition;
        for (var i = 0; i < _mesh.vertexCount; ++i)
        {
            _vertices[i] = _points[i].transform.localPosition;
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateBounds();
    }
}
