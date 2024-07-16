using System;
using MvcCore.Interfaces;
using UnityEngine;

namespace MvcCore.Abstracts
{
    public class View : MonoBehaviour, IView
    {
        public event Action ShowEvent;
        public event Action HideEvent;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            ShowEvent?.Invoke();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            HideEvent?.Invoke();
        }
    }
}