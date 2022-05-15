using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameKit.TweenAnimation
{
    public class BezierPathCurveAnimation : MonoBehaviour
    {
        [SerializeField] private List<AnimateableObject> _animatableObjects;
        [SerializeField] private float _homeMovingTime = 4;

        [SerializeField, SerializeReference] private IBezierCurve _bezierCurve;

        [SerializeField] private Direction _direction = Direction.Back;


        [ButtonGroup("Circle")]
        void MoveToCirclePosition()
        {
            var delta = 1f / _animatableObjects.Count;
            for (int i = 0; i < _animatableObjects.Count; i++)
            {
                _animatableObjects[i].MoveTo(_bezierCurve.GetPointAt(delta * i), 3);
            }
        }


        [ButtonGroup("Circle")]
        void MovingCircle()
        {
            var   startEnd = GetStartEndValue();
            float counter  = startEnd.start;
            var   delta    = 1f / _animatableObjects.Count;
            Debug.LogError(_bezierCurve.GetPointAt(0));
            DOTween.To(() => counter, (s) =>
                                      {
                                          counter = s;
                                          for (int i = 0; i < _animatableObjects.Count; i++)
                                          {
                                              var val = delta * i + counter;
                                              if (val > 1)
                                                  val -= 1;
                                              _animatableObjects[i].SetPos(_bezierCurve.GetPointAt(val));
                                          }
                                      }, startEnd.end, _homeMovingTime).SetEase(Ease.Linear);
        }


        private (float start, float end) GetStartEndValue()
        {
            if (_direction == Direction.Forward) return (0, 1);
            else return (1, 0);
        }


        [ButtonGroup("Random")]
        void SetRandom()
        {
            _animatableObjects.ForEach(e =>
                                       {
                                           Vector3 pos = new Vector3(Random.Range(-19, 70), Random.Range(-30, 30), 0);
                                           e.SetPos(pos);
                                       });
        }

        [ButtonGroup("Random")]
        void MoveHomePosition()
        {
            _animatableObjects.ForEach(e => e.MoveHoMe(_homeMovingTime));
        }

        enum Direction
        {
            Forward,
            Back
        }
    }
}