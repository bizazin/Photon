namespace MvcCore.Abstracts
{
    public abstract class Handler<T> where T : View
    {
        protected T View;

        public void Setup(T view)
        {
            View = view;
            Initialize();
        }

        public void Show() => View.Show();
        public void Hide() => View.Hide();

        protected abstract void Initialize();
    }
}