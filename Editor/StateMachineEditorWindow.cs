using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YekGames.StateMachineSystem.Abstraction;
using System.Linq;

public class StateMachineEditorWindow : EditorWindow
{
    private IStateMachine _StateMachine;
    private Dictionary<IState, Rect> _StateRects = new Dictionary<IState, Rect>();
    private Vector2 _StateSize = new Vector2(120, 50);
    private Vector2 _DragOffset;
    private IState _DraggingState;
    private Vector2 _ScrollPosition;

    // Exposed colors
    private static readonly Color ActiveStateColor = new Color(0.560f, 0.843f, 0.843f);
    private static readonly Color InactiveStateColor = Color.white;
    private static readonly Color TriggeredTransitionColor = new Color(0.305f, 0.796f, 0.552f);
    private static readonly Color DefaultTransitionColor = Color.white;

    private static readonly float OffsetAmount = 5f;

    public void SetStateMachine(IStateMachine sm)
    {
        _StateMachine = sm;
        _StateRects.Clear();

        if (_StateMachine != null && _StateMachine.States != null)
        {
            var states = _StateMachine.States.ToList();
            int stateCount = states.Count;

            if (stateCount > 0)
            {
                // Radial layout algorithm for better initial positioning
                float radius = Mathf.Min(position.width, position.height) / 3f;
                Vector2 center = new Vector2(position.width / 2f, position.height / 2f);

                for (int i = 0; i < stateCount; i++)
                {
                    float angle = i * Mathf.PI * 2f / stateCount;
                    float x = center.x + radius * Mathf.Cos(angle);
                    float y = center.y + radius * Mathf.Sin(angle);

                    _StateRects[states[i]] = new Rect(x - _StateSize.x / 2, y - _StateSize.y / 2, _StateSize.x, _StateSize.y);
                }
            }
        }

        Repaint();
    }

    private void OnGUI()
    {
        if (_StateMachine == null)
        {
            EditorGUILayout.LabelField("No State Machine assigned.");
            return;
        }

        Event e = Event.current;

        // Use a scroll view to handle a large number of states
        _ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);

        // Draw transitions first so they are behind the states
        DrawTransitions();

        // Draw states
        BeginWindows();

        // Use a copy of keys to avoid modifying collection during enumeration
        var keys = _StateRects.Keys.ToList();

        foreach (var state in keys)
        {
            var rect = _StateRects[state];
            Color oldColor = GUI.color;
            if (state == _StateMachine.CurrentState)
                GUI.color = ActiveStateColor;
            else
                GUI.color = InactiveStateColor;

            _StateRects[state] = GUI.Window(state.GetHashCode(), rect, DrawStateWindow, state.GetType().Name);

            GUI.color = oldColor;
        }

        EndWindows();

        // Handle dragging
        if (e.type == EventType.MouseDrag && _DraggingState != null)
        {
            _StateRects[_DraggingState] = new Rect(e.mousePosition - _DragOffset, _StateSize);
            Repaint();
        }
        else if (e.type == EventType.MouseUp)
        {
            _DraggingState = null;
        }

        EditorGUILayout.EndScrollView();

        // Keep repainting in play mode
        if (EditorApplication.isPlaying)
            Repaint();
    }

    private void DrawStateWindow(int id)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            _DraggingState = GetStateById(id);
            _DragOffset = Event.current.mousePosition;
            GUI.FocusControl(null);
        }

        GUI.DragWindow();
    }

    private IState GetStateById(int id)
    {
        foreach (var kvp in _StateRects)
        {
            if (kvp.Key.GetHashCode() == id)
                return kvp.Key;
        }
        return null;
    }

    private void DrawTransitions()
    {
        Handles.BeginGUI();

        foreach (var source in _StateMachine.Transitions)
        {
            foreach (var transition in source.Value)
            {
                var target = transition.TargetState;
                if (target == null || !_StateRects.ContainsKey(source.Key) || !_StateRects.ContainsKey(target))
                    continue;

                var sourceRect = _StateRects[source.Key];
                var targetRect = _StateRects[target];

                Vector2 startPos = sourceRect.center;
                Vector2 endPos = targetRect.center;

                // Detect if a reverse transition exists
                bool hasReverse = _StateMachine.Transitions.ContainsKey(target) &&
                                     _StateMachine.Transitions[target].Any(t => t.TargetState == source.Key);

                Vector3 offset = Vector3.zero;
                if (hasReverse)
                {
                    // Determine consistent ordering of states
                    bool sourceIsFirst = string.Compare(source.Key.GetType().Name, target.GetType().Name) < 0;

                    Vector2 fromPos = sourceIsFirst ? sourceRect.center : targetRect.center;
                    Vector2 toPos = sourceIsFirst ? targetRect.center : sourceRect.center;

                    Vector2 dir = (toPos - fromPos).normalized;
                    Vector2 perp = new Vector2(-dir.y, dir.x);

                    // If this edge goes from "first" to "second", offset is +offsetAmount, else -offsetAmount
                    offset = perp * (sourceIsFirst ? OffsetAmount : -OffsetAmount);
                }


                startPos += (Vector2)offset;
                endPos += (Vector2)offset;

                // Choose color
                Color lineColor = (_StateMachine.LastTriggeredTransition == transition) ? TriggeredTransitionColor : DefaultTransitionColor;
                Handles.color = lineColor;

                // Draw main line
                Handles.DrawLine(startPos, endPos);

                // Draw arrow in middle
                Vector2 midPoint = (startPos + endPos) * 0.5f;
                Vector2 dirArrow = (endPos - startPos).normalized;
                Vector2 perpArrow = new Vector2(-dirArrow.y, dirArrow.x);

                Vector2 arrowTip = midPoint + dirArrow * 12; // Adjust tip position to be closer to target
                Vector2 arrowLeft = arrowTip - dirArrow * 10 + perpArrow * 5;
                Vector2 arrowRight = arrowTip - dirArrow * 10 - perpArrow * 5;

                Handles.DrawAAConvexPolygon(arrowTip, arrowLeft, arrowRight);
            }
        }

        Handles.EndGUI();
    }
}