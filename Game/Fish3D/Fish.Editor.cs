#if TOOLS
using F00F;
using Godot;

namespace Fish3D;

public partial class Fish
{
    public sealed override void _Notification(int what)
    {
        if (Editor.OnPreSave(what))
        {
            this.ForEachChild(x => Editor.DoPreSaveReset(x, Node.PropertyName.Owner));
            return;
        }

        if (Editor.OnPostSave(what))
            Editor.DoPostSaveRestore();
    }
}
#endif
