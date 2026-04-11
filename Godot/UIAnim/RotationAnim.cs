using ArTiX.Tools;
using ArTiX.UI.Elements.Anim;
using Godot;

// Author : Aidan Bachelez

namespace Com.IsartDigital.Sokoban
{
	public partial class RotationAnim : UIAnim
	{
        [Export(PropertyHint.Range, $"0,360,{Utils.RADIANS_AS_DEGREES}")] private float enteringRotationOffset;
        [Export(PropertyHint.Range, $"0,360,{Utils.RADIANS_AS_DEGREES}")] private float focusRotationOffset;
        [Export(PropertyHint.Range, $"0,360,{Utils.RADIANS_AS_DEGREES}")] private float pressedRotationOffset;
        [Export(PropertyHint.Range, $"0,360,{Utils.RADIANS_AS_DEGREES}")] private float exitRotationOffset;

        private float initialRotation;

        protected override Tween EnterAnim()
        {
            tween = base.EnterAnim();

            tween.SetParallel();

            Utils.RotationAnim(ref tween, target, enterAnim.Duration, target.Rotation, enterAnim.Delay);
                initialRotation = target.Rotation;
                target.Rotation = enterPoint != null ? enterPoint.Rotation : target.Rotation + enteringRotationOffset;

            return tween;
        }

        protected override void OnFocusEntered()
        {
            base.OnFocusEntered();
            Utils.RotationAnim(ref tween, target, focusEnteredAnim.Duration, initialRotation + focusRotationOffset, focusEnteredAnim.Delay);
        }

        protected override void OnFocusExited()
        {
            base.OnFocusExited();
            Utils.RotationAnim(ref tween, target, focusExitedAnim.Duration, initialRotation, focusExitedAnim.Delay);
        }

        protected override void OnPressedAnim()
        {
            base.OnPressedAnim();
            Utils.RotationAnim(ref tween, target, pressedAnim.Duration, initialRotation + pressedRotationOffset, pressedAnim.Delay);
        }

        protected override void OnReleasedAnim()
        {
            base.OnReleasedAnim();
            Utils.RotationAnim(ref tween, target, releasedAnim.Duration, initialRotation, releasedAnim.Delay);
        }

        public override Tween ExitAnim()
        {
            tween = base.ExitAnim();

            Utils.RotationAnim(ref tween, target, exitAnim.Duration,
                pTargetRotation: exitPoint != null ? exitPoint.Rotation : initialRotation + exitRotationOffset,
                exitAnim.Delay);

            return tween;
        }
    }
}
