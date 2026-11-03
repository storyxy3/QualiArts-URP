using System;

namespace UnityEngine.Rendering.Universal
{
    internal static class InternalEngineBridge
    {
        internal static void AddOnLayerchangedCallback(Action callback)
        {
            // REF BUILD: SortingLayer.onLayerChanged removed in Unity 6 (2D renderer is not in the game)
        }

        internal static void RemoveOnLayerchangedCallback(Action callback)
        {
            // REF BUILD: see above
        }
    }
}
