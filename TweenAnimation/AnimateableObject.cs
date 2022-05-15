using System;
using DG.Tweening;
using UnityEngine;

namespace GameKit.TweenAnimation 
{
    [Serializable]
    public class AnimateableObject
    {
        [SerializeField]
        private Transform _homePoint;

        [SerializeField]
        private Transform _movingTransform;

        public Tween MoveHoMe(float duration)
        {
            return _movingTransform.DOMove(_homePoint.position, duration);
        }

        public void SetPos(Vector3 pos)
        {
            _movingTransform.position = pos;
        }

        public Tween MoveTo(Vector3 getPointAt, int duration)
        {
            return _movingTransform.DOMove(getPointAt, duration);
        }
    }
}