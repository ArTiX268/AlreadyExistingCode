using Godot;
using System.Collections.Generic;

namespace ArTiX.Tools
{
    public static class Utils
    {
        private const string IMPORT_FILE_EXTENSION = ".import";

        public const string ADD_CHILD = "add_child";
        public const string QUEUE_FREE = "queue_free";

        // Tweens
        public const string TWEEN_POSITION = "position";
        public const string TWEEN_GLOBALPOSITION = "global_position";
        public const string TWEEN_SCALE = "scale";
        public const string TWEEN_ROTATION = "rotation";
        public const string TWEEN_VISIBLE = "visible";
        public const string TWEEN_MODULATE = "modulate";
        public const string TWEEN_WIDTH = "width";
        public const string TWEEN_VALUE = "value";
        public const string TWEEN_ZOOM = "zoom";
        public const string TWEEN_OFFSET = "offset";
        public const string TWEEN_VOLUME = "volume_db";

        public static float GetAngleTo(Node2D pNode, Vector2 pTargetPos)
        {
            Vector2 lPos = pTargetPos - pNode.GlobalPosition;
            return Mathf.Atan2(lPos.Y, lPos.X);
        }

        public static void RotateVector2I(ref Vector2I pVectorI, float pRotation)
        {
            Vector2 lVectorF = new Vector2(pVectorI.X, pVectorI.Y);
            lVectorF = lVectorF.Rotated(pRotation);

            pVectorI = new Vector2I(Mathf.RoundToInt(lVectorF.X), Mathf.RoundToInt(lVectorF.Y));
        }

        public static T GetRandomElementFromList<T>(List<T> pList)
        {
            int lMaxIndex = pList.Count - 1;
            RandomNumberGenerator lRand = new RandomNumberGenerator();
            return pList[lRand.RandiRange(0, lMaxIndex)];
        }

        public static T GetRandomElementFromArray<T>(T[] pList)
        {
            int lMaxIndex = pList.Length - 1;
            RandomNumberGenerator lRand = new RandomNumberGenerator();
            return pList[lRand.RandiRange(0, lMaxIndex)];
        }

        /// <summary>
        /// Returns all the files from the given type in the given directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pDirPath"></param>
        /// <returns></returns>
        public static List<T> GetAllFilesOfTypeInDir<T>(string pDirPath) where T : Resource
        {
            List<T> lList = new List<T>();
            DirAccess lDir = DirAccess.Open(pDirPath);
            if (lDir != null)
            {
                foreach (string lFileName in lDir.GetFiles())
                    if (lFileName.Substring(lFileName.Length - IMPORT_FILE_EXTENSION.Length) != IMPORT_FILE_EXTENSION)
                        lList.Add(ResourceLoader.Load<T>(pDirPath + lFileName));
            }
            else
                GD.Print("Folder does not exist.");

            return lList;
        }

        #region Animation

        public static Tween PositionAnim(ref Tween pTween, GodotObject pObject, float pDuration, Vector2 pTargetPos, float pDelay = 0, Tween.EaseType pEasing = Tween.EaseType.InOut, Tween.TransitionType pTransition = Tween.TransitionType.Quad)
        {
            pTween.TweenProperty(pObject, TWEEN_POSITION, pTargetPos, pDuration)
                .SetEase(pEasing).SetTrans(pTransition).SetDelay(pDelay);
            return pTween;
        }

        public static Tween PositionAnim(ref Tween pTween, GodotObject pObject, float pDuration, Vector2 pTargetPos, float pDelay = 0)
        {
            pTween.TweenProperty(pObject, TWEEN_POSITION, pTargetPos, pDuration).SetDelay(pDelay);
            return pTween;
        }

        public static Tween ScaleAnim(ref Tween pTween, GodotObject pObject, float pDuration, Vector2 pTargetScale, float pDelay = 0, Tween.EaseType pEasing = Tween.EaseType.InOut, Tween.TransitionType pTransition = Tween.TransitionType.Quad)
        {
            pTween.TweenProperty(pObject, TWEEN_SCALE, pTargetScale, pDuration)
                .SetEase(pEasing).SetTrans(pTransition).SetDelay(pDelay);
            return pTween;
        }

        public static Tween ScaleAnim(ref Tween pTween, GodotObject pObject, float pDuration, Vector2 pTargetScale, float pDelay)
        {
            pTween.TweenProperty(pObject, TWEEN_SCALE, pTargetScale, pDuration).SetDelay(pDelay);
            return pTween;
        }

        public static Tween RotationAnim(ref Tween pTween, GodotObject pObject, float pDuration, float pTargetRotation, float pDelay = 0, Tween.EaseType pEasing = Tween.EaseType.InOut, Tween.TransitionType pTransition = Tween.TransitionType.Quad)
        {
            pTween.TweenProperty(pObject, TWEEN_ROTATION, pTargetRotation, pDuration)
                .SetEase(pEasing).SetTrans(pTransition).SetDelay(pDelay);
            return pTween;
        }

        public static Tween RotationAnim(ref Tween pTween, GodotObject pObject, float pDuration, float pTargetRotation, float pDelay)
        {
            pTween.TweenProperty(pObject, TWEEN_ROTATION, pTargetRotation, pDuration).SetDelay(pDelay);
            return pTween;
        }

        #endregion
    }
}
