using System;

namespace GameKit.CustomGameEditor
{
    public interface ICustomGameEditor
    {
        event Action OnFinishWorkingEvent;
        void StartWork();
        void StopWork();
    }
}