using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameKit.TweenAnimation 
{
    [Serializable]
    class CircleBezierCurve : BezierCurve
    {
        [SerializeField] private Transform _center;

        [SerializeField, MinValue(2), OnValueChanged("Rebuild")]
        private float _radius = 2;

        [SerializeField, Range(0, 360), OnValueChanged("Rebuild")]
        private float _angel = 2;

        [SerializeField, MinValue(3), OnValueChanged("Rebuild")]
        private int _segmentCount = 5;

        void Rebuild()
        {
            DestroyAll();
            var deltaAngel = 360 / _segmentCount;
            for (int i = 0; i < _segmentCount; i++)
            {
                var radian = (_angel + deltaAngel * i) * Mathf.Deg2Rad;
                var pos    = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
                pos *= _radius;
                pos += _center.position;
                this.AddPointAt(pos);
            }
            //добавляем еще одну точку, чтобы при движении начальная и конечная точка совпадали
            this.AddPointAt(this.GetPointAt(0)); 
        }

        
        [Button,ShowIf("pointCount", 0)]
        void GetAllChildrenPoints()
        {
            GetComponentsInChildren<BezierPoint>().ForEach(this.AddPoint);
            _segmentCount = this.pointCount;
        }

        private void Reset()
        {
            _center = this.transform;
        }


        private void DestroyAll()
        {
            var points = this.GetAnchorPoints();
            for (int i = 0; i < points.Length; i++)
            {
                this.RemovePoint(points[i]);
            }

            points.ForEach(e =>
                           {
                               DestroyImmediate(e.gameObject);
                               DestroyImmediate(e);
                           });
        }

    }
}