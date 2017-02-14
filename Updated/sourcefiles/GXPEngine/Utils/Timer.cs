using System;
namespace GXPEngine
{
    public class Timer : GameObject
    {
        private int _time;
        private Action _onTimeout;

        public Timer(int timeOut, Action onTimeout) : base()
        {
            _onTimeout = onTimeout;
            _time = timeOut;
        }

        private void Update()
        {
            _time -= Time.deltaTime;
            if (_time <= 0)
            {
                Destroy();
                _onTimeout();
            }
        }
    }
}
