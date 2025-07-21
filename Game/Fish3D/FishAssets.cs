using System.Collections.Generic;
using System.Linq;
using F00F;
using Godot;

namespace Fish3D;

[Tool, GlobalClass]
public partial class FishAssets : Resource
{
    [Export] private Mesh[] OBJ { get; set => this.Set(ref field, value, () => { ObjLookup = null; Keys = null; }); }
    [Export] private PackedScene[] FBX { get; set => this.Set(ref field, value, () => { FbxLookup = null; Keys = null; }); }

    private Dictionary<string, Mesh> ObjLookup { get => field ??= OBJ.ToDictionary(x => $"OBJ: {x.ResourceName()}"); set; }
    private Dictionary<string, PackedScene> FbxLookup { get => field ??= FBX.ToDictionary(x => $"FBX: {x.ResourceName()}"); set; }
    private static bool IsOBJ(string key) => key.StartsWith("OBJ"); private static bool IsFBX(string key) => key.StartsWith("FBX");

    public string[] Keys { get => field ??= [.. ObjLookup.Keys, .. FbxLookup.Keys]; private set; }

    public Mesh GetMesh(string key, out Transform3D xform)
    {
        if (IsOBJ(key)) return GetOBJMesh(key, out xform);
        if (IsFBX(key)) return GetFBXMesh(key, out xform);

        xform = Transform3D.Identity;
        return null;

        Mesh GetOBJMesh(string key, out Transform3D xform)
        {
            xform = Transform3D.Identity;
            return ObjLookup.Get(key);
        }

        Mesh GetFBXMesh(string key, out Transform3D xform)
        {
            var scene = FbxLookup.Get(key)?.Instantiate();
            if (scene is null) { xform = Transform3D.Identity; return null; }

            var source = scene.RecurseChildren<MeshInstance3D>().Single();
            xform = source.GlobalTransform();
            var mesh = source.Mesh;
            scene.Free();
            return mesh;
        }
    }

    public Node GetScene(string key)
    {
        return IsOBJ(key) ? GetOBJScene(key)
             : IsFBX(key) ? GetFBXScene(key)
             : null;

        MeshInstance3D GetOBJScene(string key)
            => ObjLookup.Get(key)?.AsMeshInstance();

        Node GetFBXScene(string key)
            => FbxLookup.Get(key)?.Instantiate();
    }

    public bool HasAnimationPlayer(string key)
    {
        var state = FbxLookup.Get(key)?.GetState();
        return state?.GetNodeName(state.GetNodeCount() - 1) == "AnimationPlayer";
    }
}
