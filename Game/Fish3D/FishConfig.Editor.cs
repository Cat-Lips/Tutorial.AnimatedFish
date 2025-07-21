#if TOOLS
using System.Linq;
using F00F;
using Godot.Collections;

namespace Fish3D;

public partial class FishConfig
{
    public sealed override void _ValidateProperty(Dictionary property)
    {
        if (Editor.SetEnumHint(property, PropertyName.Asset, Assets.Keys.Prepend(None))) return;
        if (Editor.Show(property, PropertyName.MeshOrScene, Asset is not None)) return;
        if (Editor.Show(property, PropertyName.Flip, Asset is not None)) return;
        if (Editor.Show(property, PropertyName.Animate, MeshOrScene is MeshOrSceneEnum.Scene && Assets.HasAnimationPlayer(Asset))) return;
        if (Editor.Show(property, PropertyName.ApplyShader, Asset is not None)) return;
    }
}
#endif
