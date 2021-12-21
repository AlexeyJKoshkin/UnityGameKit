namespace GameKit.EditorContext {
    public interface IDIWrapper
    {
        void FlushBindings();
        void ResolveRoots();

        void BindAsSingle<T>();
        T Resolve<T>();
    }
}