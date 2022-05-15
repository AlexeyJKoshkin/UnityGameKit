using System;
using UnityEngine;

namespace GameKit.TweenAnimation
{
    [Serializable]
    public class  CircleBezierCurveWrapper : IBezierCurve
    {
        [SerializeField]
        private CircleBezierCurve _bezierCurve;
        public Vector3 GetPointAt(float delta)
        {
            return _bezierCurve.GetPointAt(delta);
        }
    }
}