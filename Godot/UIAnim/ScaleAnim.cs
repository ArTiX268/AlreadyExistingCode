using ArTiX.Tools;
using ArTiX.UI.Elements.Anim;
using Godot;

// Author : Aidan Bachelez

namespace Com.IsartDigital.Sokoban
{
	public partial class ScaleAnim : UIAnim
    {
        [ExportSubgroup("Entering")]
        [Export(PropertyHint.Link)] private Vector2 enterOffset;

        [ExportSubgroup("Focus")]
        [Export(PropertyHint.Link)] private Vector2 focusOffset;

        [ExportSubgroup("Focus")]
        [Export(PropertyHint.Link)] private Vector2 pressedOffset;

        [ExportSubgroup("Exiting")]
        [Export(PropertyHint.Link)] private Vector2 exitOffset;

        private Vector2 initialScale;

        public override void _Ready()
        {
            initialScale = target.Scale;
            base._Ready();
        }

        protected override Tween EnterAnim()
        {
            tween = base.EnterAnim();

            Utils.ScaleAnim(ref tween, target, enterAnim.Duration, target.Scale, enterAnim.Delay);

            target.Scale = enterPoint != null ? enterPoint.Scale : target.Scale + enterOffset;
            return tween;
        }

        protected override void OnFocusEntered()
        {
            base.OnFocusEntered();
            Utils.ScaleAnim(ref tween, target, focusEnteredAnim.Duration, initialScale + focusOffset, focusEnteredAnim.Delay);
        }

        protected override void OnFocusExited()
        {
            base.OnFocusExited();
            Utils.ScaleAnim(ref tween, target, focusExitedAnim.Duration, initialScale, focusExitedAnim.Delay);
        }

        protected override void OnPressedAnim()
        {
            base.OnPressedAnim();
            Utils.ScaleAnim(ref tween, target, pressedAnim.Duration, initialScale + pressedOffset, pressedAnim.Delay);
        }

        protected override void OnReleasedAnim()
        {
            base.OnReleasedAnim();
            Utils.ScaleAnim(ref tween, target, releasedAnim.Duration, initialScale, releasedAnim.Delay);
        }

        public override Tween ExitAnim()
        {
            tween = base.ExitAnim();

            Utils.ScaleAnim(ref tween, target, exitAnim.Duration, exitPoint != null ? exitPoint.Scale : initialScale + exitOffset, exitAnim.Delay);
            return tween;
        }
    }
}
