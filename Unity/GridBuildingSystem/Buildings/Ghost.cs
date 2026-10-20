using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class Ghost : MonoBehaviour
    {
        public static Ghost Create(Ghost ghost)
        {
            return Instantiate(ghost);
        }

        [SerializeField] private GhostSO datas;
        [SerializeField] private BuildingSO buildingDatas;

        private Vector2Int cellPos;
        private float targetRotation;

        private STweenHandle rotationTween;
        private STweenHandle positionTween;

        private STweenHandle colorTween;

        private void Start()
        {
            UpdateColor();
        }

        public void SetPosition(in Vector2Int cellPos)
        {
            positionTween.Kill();

            this.cellPos = cellPos;
            transform.TweenPosition(
                targetPos: GridBuildingSystem.Instance.ConvertCellPosToWorldPos(cellPos),
                animParams: datas.PositionAnim,
                tween: ref positionTween);

            UpdateColor();
        }

        public void SetRotation(in float rotation)
        {
            rotationTween.Kill();

            targetRotation = rotation;
            transform.TweenRotation(
                targetRota: Quaternion.Euler(x: 0, rotation, z: 0),
                animParams: datas.RotationAnim,
                tween: ref rotationTween);

            UpdateColor();
        }

        private void UpdateColor()
        {
            bool isValid = GridBuildingSystem.Instance.CanBuild(buildingDatas, cellPos, targetRotation);

            colorTween.Kill();

            foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
            {
                renderer.material.TweenColorProperty(this,
                    property: Utilities.COLOR,
                    targetColor: isValid ? datas.ValidColor : datas.UnvalidColor,
                    animParams: datas.ColorChangeAnim,
                    tween: ref colorTween
                );
            }
        }

        public void Destroy()
        {
            positionTween.Kill();
            rotationTween.Kill();
            colorTween.Kill();

            Destroy(gameObject);
        }
    }
}
