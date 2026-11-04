namespace VL.EntitySystem
{
    public static class EntityComponentEvent<TComponent>
    {
        public delegate void EntityComponentAction(TComponent component);
        public static event EntityComponentAction onAdd;
        public static event EntityComponentAction onRemove;
        public static event EntityComponentAction onPropertyChange;
        public static void InvokeAdd(IEntityComponent component) { onAdd?.Invoke((TComponent)component); }
        public static void InvokeRemove(IEntityComponent component) { onRemove?.Invoke((TComponent)component); }
        public static void InvokePropertyChange(IEntityComponent component) { onPropertyChange?.Invoke((TComponent)component); }
        public static void InvokeAdd<T>(IEntityComponent<T> component)
        {
            component.oldChunkKey = component.chunkKey;
            onAdd?.Invoke((TComponent)component);
        }
        public static void InvokeRemove<T>(IEntityComponent<T> component) { onRemove?.Invoke((TComponent)component); }
        public static void InvokePropertyChange<T>(IEntityComponent<T> component)
        {
            if (component.isChunkChanged)
            {
                InvokeRemove(component);
                InvokeAdd(component);
                return;
            }
            onPropertyChange?.Invoke((TComponent)component);
        }
    }
}
