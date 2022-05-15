using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameKit.TweenAnimation
{
    public class BezierPathCurveAnimation : MonoBehaviour
    {
        [SerializeField] private List<Transform> _animatableObjects;
        [SerializeField, SerializeReference] private CircleBezierCurve _bezierCurve;

        public Tween MoveToStartCurvePosition(float duration)
        {
            var      delta    = 1f / _animatableObjects.Count;
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < _animatableObjects.Count; i++)
            {
                var tween = _animatableObjects[i].DOMove(_bezierCurve.GetPointAt(delta * i), duration);
                sequence.Join(tween);
            }

            return sequence;
        }

        public Tween MoveByCurvePosition(float duration, Direction direction)
        {
            var   startEnd = GetStartEndValue(direction);
            float counter  = startEnd.start;
            var   delta    = 1f / _animatableObjects.Count;
            Debug.LogError(_bezierCurve.GetPointAt(0));
            return DOTween.To(() => counter, (s) =>
                                      {
                                          counter = s;
                                          for (int i = 0; i < _animatableObjects.Count; i++)
                                          {
                                              var val = delta * i + counter;
                                              if (val > 1)
                                                  val -= 1;
                                              _animatableObjects[i].position =_bezierCurve.GetPointAt(val);
                                          }
                                      }, startEnd.end, duration).SetEase(Ease.Linear);
        }


        private (float start, float end) GetStartEndValue(Direction direction)
        {
            if (direction == Direction.Forward) return (0, 1);
            else return (1, 0);
        }

      public  enum Direction
        {
            Forward,
            Back
        }
    }
}