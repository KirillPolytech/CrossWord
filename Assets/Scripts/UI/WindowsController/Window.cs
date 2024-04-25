namespace CrosswordWindows
{
    using UnityEngine;
    public abstract class Window : MonoBehaviour
    {
        protected Canvas _canvas;

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public virtual void Open()
        {
            _canvas.enabled = true;
        }

        public virtual void Close()
        {
            _canvas.enabled = false;
        }
    }
}