using ArTiX.Sound;
using ArTiX.Tools;
using Godot;
using System;
using System.Collections.Generic;

// Author : Aidan Bachelez

namespace ArTiX.UI.Elements.Anim
{
	public abstract partial class UIAnim : Node
    {
        [Export] protected Control target;

        [ExportSubgroup("Entering")]
        [Export] protected Control enterPoint;
        [Export] protected UIAnimParams enterAnim;
        [Export] private bool hidenBeforeStarting;
        [Export] private bool grabFocusWhenFinishedAnim;

        [ExportSubgroup("Focus")]
        [Export] protected Control focusPoint;
        [Export] protected UIAnimParams focusEnteredAnim;
        [Export] protected UIAnimParams focusExitedAnim;

        [ExportSubgroup("Pressed")]
        [Export] private bool isButtonAnim;
        [Export] protected UIAnimParams pressedAnim;
        [Export] protected UIAnimParams releasedAnim;

        [ExportSubgroup("Exit")]
        [Export] protected Control exitPoint;
        [Export] protected UIAnimParams exitAnim;
        [Export] private bool hidenWhenFinishingAnim;
        [Export] public bool IsTheLastAnim { get; private set; }

        private static List<UIAnim> animations = new List<UIAnim>();

        public event Action EnterAnimFinished;
        public event Action ExitAnimFinished;

        protected Tween tween;

        public static void LaunchExitAnim(out UIAnim pLastAnim)
        {
            pLastAnim = null;
            foreach (UIAnim lAnim in animations)
            {
                if (lAnim.IsTheLastAnim) pLastAnim = lAnim;
                lAnim.ExitAnim();
            }
        }

        public override void _Ready()
        {
            target ??= GetParent() as Control;
            EnterAnim();

            target.FocusEntered += OnFocusEntered;
            target.FocusExited += OnFocusExited;

            if (isButtonAnim && target is Button button)
            {
                button.Pressed += OnPressedAnim;
                button.ButtonUp += OnReleasedAnim;
            }

            target.Visible = !hidenBeforeStarting;

            animations.Add(this);
        }

        protected virtual Tween EnterAnim()
        {
            RecreateTween(enterAnim);

            if (hidenBeforeStarting)
                tween.TweenProperty(target, Utils.TWEEN_VISIBLE, true, 0).SetDelay(enterAnim.Delay);

            if (grabFocusWhenFinishedAnim)
                tween.Finished += target.GrabFocus;

            tween.Finished += () => EnterAnimFinished?.Invoke();

            if (enterAnim.Delay > 0)
            {
                Timer lAnimTimer = new Timer
                {
                    Autostart = true,
                    WaitTime = enterAnim.Delay,
                    OneShot = true
                };
                lAnimTimer.Timeout += StartedEnterAnim;
                lAnimTimer.Timeout += lAnimTimer.QueueFree;

                AddChild(lAnimTimer);
            }

            return tween;
        }

        protected virtual void StartedEnterAnim()
        {
            SoundManager.GetInstance().PlaySfx(enterAnim.Sound);
        }

        protected virtual void FinishEnteringAnim()
        {

        }

        protected virtual void OnFocusEntered() => RecreateTween(focusEnteredAnim);
        protected virtual void OnFocusExited() => RecreateTween(focusExitedAnim);
        protected virtual void OnPressedAnim() => RecreateTween(pressedAnim);
        protected virtual void OnReleasedAnim() => RecreateTween(releasedAnim);

        public virtual Tween ExitAnim()
        {
            target.FocusMode = Control.FocusModeEnum.None;

            if (isButtonAnim && target is Button)
                (target as Button).Disabled = true;

            RecreateTween(exitAnim);

            if (hidenWhenFinishingAnim)
                tween.Finished += () => target.Visible = false;

            tween.Finished += () => ExitAnimFinished?.Invoke();

            return tween;
        }

        private void RecreateTween(Tween.EaseType easing, Tween.TransitionType transition)
        {
            tween?.Kill();
            tween = CreateTween().SetEase(easing).SetTrans(transition);
        }

        private void RecreateTween(UIAnimParams animParams) => RecreateTween(animParams.Easing, animParams.Transition);

        protected override void Dispose(bool pDisposing)
        {
            animations.Remove(this);
            base.Dispose(pDisposing);
        }
    }
}
