namespace CrosswordWindows
{
    using UnityEngine;
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] private string windowName;

        public string WindowName => windowName;
        
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