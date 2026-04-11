using ArTiX.Sound;
using Godot;

namespace ArTiX.UI.Elements.Anim
{
    [GlobalClass]
    public partial class UIAnimParams : Resource
    {
        [Export] public float Delay { get; private set; } = 0.2f;
        [Export] public float Duration { get; private set; } = 1f;
        [Export] public Tween.EaseType Easing { get; private set; } = Tween.EaseType.InOut;
        [Export] public Tween.TransitionType Transition { get; private set; } = Tween.TransitionType.Quad;
        [Export] public ESoundType Sound { get; private set; }
    }
}