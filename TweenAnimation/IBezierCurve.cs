using UnityEngine;

namespace GameKit.TweenAnimation {
    public interface IBezierCurve
    {
        Vector3 GetPointAt(float delta);
    }
}