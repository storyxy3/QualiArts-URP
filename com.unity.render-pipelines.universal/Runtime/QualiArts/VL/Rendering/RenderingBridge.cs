using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL.Rendering
{
    public static class RenderingBridge
    {
        public enum EventType { PreDepth, GBuffer, MotionVector, LitLighting, AfterGBuffer, ActorDecal, Last }
        public delegate void RenderEvent(ScriptableRenderContext context, ref RenderingData renderingData);
        private static readonly List<RenderEvent>[] _renderEvents = new List<RenderEvent>[(int)EventType.Last];
        internal static void Execute(EventType type, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var events = _renderEvents[(int)type];
            if (events == null) return;
            foreach (var renderEvent in events) renderEvent?.Invoke(context, ref renderingData);
            events.Clear();
        }
        internal static void ClearEvents()
        {
            foreach (var events in _renderEvents) events?.Clear();
        }
        public static void AddRenderEvent(EventType type, RenderEvent renderEvent)
        {
            var events = _renderEvents[(int)type];
            if (events == null)
            {
                events = new List<RenderEvent>();
                _renderEvents[(int)type] = events;
            }
            if (events.Count < 32) events.Add(renderEvent);
            else Debug.LogError($"{type} 登録数が32個以上です Executeを呼び出して処理済みにしているか確認してください");
        }
    }
}
