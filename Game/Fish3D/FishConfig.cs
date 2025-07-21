using System;
using System.Diagnostics;
using System.Linq;
using F00F;
using Godot;

namespace Fish3D;

[Tool, GlobalClass]
public partial class FishConfig : CustomResource
{
    private const string None = "<none>";
    public enum MeshOrSceneEnum { Mesh, Scene, MultiMesh }

    [Export] public string Asset { get; set => this.Set(ref field, value, notify: true); } = None;
    [Export] public MeshOrSceneEnum MeshOrScene { get; set => this.Set(ref field, value, notify: true); }
    [Export] public bool Flip { get; set => this.Set(ref field, value); }
    [Export] public bool Animate { get; set => this.Set(ref field, value); }
    [Export] public bool ApplyShader { get; set => this.Set(ref field, value); }

    [Export] private FishAssets Assets { get; set => this.Set(ref field, value ?? Utils.LoadRes<FishAssets>(), notify: true); } = Utils.LoadRes<FishAssets>();

    public void Init(Fish root)
    {
        this.SafeInit(root, () =>
        {
            root.RemoveChildren();
            if (string.IsNullOrEmpty(Asset)) return;

            InitMesh();
            InitShader();
            InitAnimation();

            void InitMesh()
            {
                switch (MeshOrScene)
                {
                    case MeshOrSceneEnum.Mesh: InitFromMesh(); return;
                    case MeshOrSceneEnum.Scene: InitFromScene(); return;
                    case MeshOrSceneEnum.MultiMesh: InitMultiMesh(); return;
                    default: throw new NotImplementedException();
                }

                void InitFromMesh()
                {
                    var mesh = Assets.GetMesh(Asset, out var xform);
                    if (mesh is null) return;

                    root.AddChild(mesh.AsMeshInstance(xform), own: true);
                }

                void InitFromScene()
                {
                    var scene = Assets.GetScene(Asset);
                    if (scene is null) return;

                    root.AddChild(scene, own: true);
                }

                void InitMultiMesh()
                {
                    //var mesh = Assets.GetMesh(Source);
                    //if (mesh is null) return;

                    //root.AddChild(mesh.AsMultiMeshInstance(), own: true);
                }
            }

            void InitShader()
            {
                if (!ApplyShader) return;

                var mesh = root.RecurseChildren<MeshInstance3D>(self: true).Single();
                mesh.MaterialOverlay = New.ShaderMaterial<Fish>();

                //for (var i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); ++i)
                //{
                //    var material = mesh.Mesh.SurfaceGetMaterial(i);
                //    var shader = New.ShaderMaterial<Fish>();
                //    shader.NextPass = material;
                //    mesh.Mesh.SurfaceSetMaterial(i, shader);
                //}
            }

            void InitAnimation()
            {
                if (!Animate) return;

                var ap = root.RecurseChildren<AnimationPlayer>().SingleOrDefault();
                if (ap is null) return;

                var anims = ap.GetAnimationList();
                if (anims.Length is 0) return;
                WarnAnimCount(this, anims);

                var anim = (StringName)anims[0];
                SetLoopMode();
                ap.Play(anim);

                void SetLoopMode()
                {
                    var cfg = ap.GetAnimation(anim);
                    if (cfg.LoopMode is Animation.LoopModeEnum.None)
                        cfg.LoopMode = Animation.LoopModeEnum.Linear;
                }

                [Conditional("DEBUG")]
                static void WarnAnimCount(FishConfig self, string[] anims)
                {
                    if (anims.Length is 1) return;
                    GD.PushWarning($"Playing first anim only [{nameof(self.Asset)}={self.Asset}, Anims={string.Join('|', anims)}]");
                }
            }
        });
    }
}
