using System;
using UnityEngine;
namespace VL.EntitySystem
{
    public class SharedEntityManager<TManager, TComponent> : IDisposable
        where TManager : class, IEntityManager<TComponent>, new()
        where TComponent : MonoBehaviour, IEntityComponent
    {
        private TManager _entityManager;
        private int _referenceCounter;
        public TManager Get()
        {
            if (_entityManager == null)
            {
                _entityManager = new TManager();
                foreach (var component in UnityEngine.Object.FindObjectsOfType<TComponent>())
                {
                    if (component.isActiveAndEnabled && !_entityManager.IsValidEntity(component.entity))
                        component.entity = _entityManager.CreateEntity(component);
                }
                EntityComponentEvent<TComponent>.onAdd += OnAdd;
                EntityComponentEvent<TComponent>.onRemove += OnRemove;
                EntityComponentEvent<TComponent>.onPropertyChange += OnPropertyChange;
            }
            ++_referenceCounter;
            return _entityManager;
        }
        public void Release(ref TManager manager)
        {
            if (manager == null || _referenceCounter == 0) return;
            if (--_referenceCounter == 0) Dispose();
            manager = null;
        }
        public void Dispose()
        {
            _entityManager.Dispose();
            _entityManager = null;
            _referenceCounter = 0;
            EntityComponentEvent<TComponent>.onAdd -= OnAdd;
            EntityComponentEvent<TComponent>.onRemove -= OnRemove;
            EntityComponentEvent<TComponent>.onPropertyChange -= OnPropertyChange;
        }
        private void OnAdd(TComponent component)
        {
            if (!_entityManager.IsValidEntity(component.entity))
                component.entity = _entityManager.CreateEntity(component);
        }
        private void OnRemove(TComponent component) { _entityManager.DestroyEntity(component.entity); }
        private void OnPropertyChange(TComponent component)
        {
            if (_entityManager.IsValidEntity(component.entity))
                _entityManager.UpdateEntityData(component.entity, component);
        }
    }
}
