using Godot;
using System;
using System.Collections.Generic;

public class VoxelChunk : StaticBody
{
    private Vector3[] _vertices = new Vector3[8];
    private int[] _top = {2, 3, 7, 6};
    private int[] _bottom = {0, 4, 5, 1};
    private int[] _left = {6, 4, 0, 2};
    private int[] _right = {3, 1, 5, 7};
    private int[] _front = {7, 5, 4, 6};
    private int[] _back = {2, 0, 1, 3};
    private List<int[]> _faces = new List<int[]>();
    
    public Vector3 Dimension
    {
        get { return new Vector3(16, 16, 16); }        
    }
    
    public Vector2 TextureAtlasSize
    {
        // Arbitrary, may change later
        get { return new Vector2(3, 2); }        
    }
    
    private bool[,,] _blocks;
    
    private SurfaceTool _st = new SurfaceTool();
    private ArrayMesh _mesh;
    private MeshInstance _meshInst;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // according to .NET, by default initialized to 0
        _blocks = new bool[(int)Dimension.x, (int)Dimension.y, (int)Dimension.z];
        
        _vertices[0] = new Vector3(0, 0, 0);
        _vertices[1] = new Vector3(1, 0, 0);
        _vertices[2] = new Vector3(0, 1, 0);
        _vertices[3] = new Vector3(1, 1, 0);
        _vertices[4] = new Vector3(0, 0, 1);
        _vertices[5] = new Vector3(1, 0, 1);
        _vertices[6] = new Vector3(0, 1, 1);
        _vertices[7] = new Vector3(1, 1, 1);
        
        _faces.Add(_top);
        _faces.Add(_bottom);
        _faces.Add(_left);
        _faces.Add(_right);
        _faces.Add(_back);
        _faces.Add(_front);
        
        Update();
    }
    
    public void Update()
    {
        if (_meshInst != null)
        {
            _meshInst.CallDeferred("queue_free");
            _meshInst = null;
        }
        
        _mesh = new ArrayMesh();
        _meshInst = new MeshInstance();
        
        _st.Begin(Mesh.PrimitiveType.Triangles);
        
        // generate the blocks
        for (int x = 0; x < Dimension.x; x++)
        {
            for (int y = 0; y < Dimension.y; y++)
            {
                for (int z = 0; z < Dimension.z; z++)
                {
                    CreateBlock(x, y, z);
                }
            }
        }
        
        _st.GenerateNormals(false);
        _st.Commit(_mesh);
        _meshInst.Mesh = _mesh;
        
        AddChild(_meshInst);
        _meshInst.CreateTrimeshCollision();
    }
    
    private void CreateBlock(int x, int y, int z)
    {
        foreach (int[] face in _faces)
        {
            if (GenerateBlock(x, y, z))
            {
                CreateFace(face, x, y, z);
            }
        }
    }
    
    private void CreateFace(int[] face, int x, int y, int z)
    {
        Vector3 offset = new Vector3(x, y, z);
        var a = _vertices[face[0]] + offset;
        var b = _vertices[face[1]] + offset;
        var c = _vertices[face[2]] + offset;
        var d = _vertices[face[3]] + offset;
        
        _st.AddTriangleFan(new Vector3[]{a, b, c});
        _st.AddTriangleFan(new Vector3[]{a, c, d});
    }
    
    // Temporary. In the future it will read from a save file.
    private bool GenerateBlock(int x, int y, int z)
    {
        // checkerboard
        return (x + y + z) % 2 == 1;
        
        // skyscraper
        // return (y % 3) == 0;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
