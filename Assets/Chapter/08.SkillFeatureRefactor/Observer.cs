using System;
using System.Collections.Generic;


namespace SkillFeatureStudy
{
    public class Observer<T>
    {
        private readonly List<Action<T>> _listeners = new List<Action<T>>();

        public void Subscribe(Action<T> listener)
        {
            _listeners.Add(listener);
        }


        public void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        public void Notify(T value)
        {
            foreach (var listener in _listeners)
            {
                listener(value);
            }
        }
    }

}
