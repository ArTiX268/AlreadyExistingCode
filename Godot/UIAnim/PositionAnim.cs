using ArTiX.Tools;
using ArTiX.UI.Elements.Anim;
using Godot;

// Author : Aidan Bachelez

namespace Com.IsartDigital.Sokoban
{
	public partial class PositionAnim : UIAnim
	{
        [ExportSubgroup("Entering")]
        [Export(PropertyHint.Link)] private Vector2 enterOffset;

        [ExportSubgroup("Focus")]
        [Export(PropertyHint.Link)] private Vector2 focusOffset;

        [ExportSubgroup("Pressed")]
        [Export(PropertyHint.Link)] private Vector2 pressedOffset;

        [ExportSubgroup("Exiting")]
        [Export(PropertyHint.Link)] private Vector2 exitOffset;

        private Vector2 initialPos;

        protected override Tween EnterAnim()
        {
            tween = base.EnterAnim();

            tween.SetParallel();

            Utils.PositionAnim(ref tween, target, enterAnim.Duration, target.Position, enterAnim.Delay);
                initialPos = target.Position;
                target.Position = enterPoint != null ? enterPoint.Position : target.Position + enterOffset;

            return tween;
        }

        protected override void OnFocusEntered()
        {
            base.OnFocusEntered();
            Utils.PositionAnim(ref tween, target, focusEnteredAnim.Duration, initialPos + focusOffset, 0, focusEnteredAnim.Easing, focusEnteredAnim.Transition);
        }

        protected override void OnFocusExited()
        {
            base.OnFocusExited();
            Utils.PositionAnim(ref tween, target, focusExitedAnim.Duration, initialPos, 0, focusExitedAnim.Easing, focusExitedAnim.Transition);
        }

        protected override void OnPressedAnim()
        {
            base.OnPressedAnim();
            Utils.PositionAnim(ref tween, target, pressedAnim.Duration, initialPos + pressedOffset, pressedAnim.Delay);
        }

        protected override void OnReleasedAnim()
        {
            base.OnReleasedAnim();
            Utils.PositionAnim(ref tween, target, releasedAnim.Duration, initialPos, releasedAnim.Delay);
        }

        public override Tween ExitAnim()
        {
            tween = base.ExitAnim();

            Utils.PositionAnim(ref tween, target, exitAnim.Duration,
                exitPoint != null ? exitPoint.Position : initialPos + exitOffset,
                exitAnim.Delay);

            return tween;
        }
    }
}
