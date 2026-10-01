using System;
using System.Collections.Generic;
using RollicGames.Math.Runtime.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Interactive level view. Turns pointer and key input into grid positions for the active tool and draws the layers.</summary>
    public class LevelGridCanvas : VisualElement
    {
        private const float Padding = 32f;
        private const float MaxCellSize = 72f;
        private const int NoPointer = -1;

        public event Action<int2?> HoveredPositionChanged;

        private readonly LevelEditorSession _session;
        private readonly LevelEditorColors _colors;
        private readonly LevelObjectEditors _editors;
        private readonly IReadOnlyList<ILevelCanvasLayer> _layers;

        private ILevelEditorTool _tool;
        private int2? _hoveredPosition;
        private Vector2? _pointerPosition;
        private int _activePointerId = NoPointer;

        public LevelGridCanvas(LevelEditorSession session, LevelEditorColors colors, LevelObjectEditors editors, IReadOnlyList<ILevelCanvasLayer> layers)
        {
            _session = session;
            _colors = colors;
            _editors = editors;
            _layers = layers;

            AddToClassList("le-canvas");
            focusable = true;
            generateVisualContent += OnGenerateVisualContent;

            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            RegisterCallback<PointerCaptureOutEvent>(_ => _activePointerId = NoPointer);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
        }

        public void SetTool(ILevelEditorTool tool)
        {
            _tool = tool;
            MarkDirtyRepaint();
        }

        private LevelCanvasLayout CreateLayout()
        {
            return LevelCanvasLayout.Create(contentRect, _session.Grid.Width, _session.Grid.Height, Padding, MaxCellSize);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if(evt.button != 0 || _tool == null || !_session.IsOpen) return;

            Focus();
            _activePointerId = evt.pointerId;
            this.CapturePointer(evt.pointerId);
            _tool.OnPointerDown(UpdatePointer(evt.localPosition));
            evt.StopPropagation();
            MarkDirtyRepaint();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            var previousPosition = _hoveredPosition;
            var position = UpdatePointer(evt.localPosition);
            if(_activePointerId == evt.pointerId && previousPosition != position)
            {
                _tool?.OnPointerDrag(position);
            }

            MarkDirtyRepaint();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if(evt.pointerId != _activePointerId) return;

            _activePointerId = NoPointer;
            this.ReleasePointer(evt.pointerId);
            _tool?.OnPointerUp(UpdatePointer(evt.localPosition));
            MarkDirtyRepaint();
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            if(_activePointerId != NoPointer) return;

            _pointerPosition = null;
            SetHoveredPosition(null);
            MarkDirtyRepaint();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if(_tool == null || !_tool.OnKeyDown(evt.keyCode)) return;

            evt.StopPropagation();
            MarkDirtyRepaint();
        }

        private int2 UpdatePointer(Vector3 localPosition)
        {
            _pointerPosition = localPosition;
            var position = CreateLayout().ToGridPosition(localPosition);
            SetHoveredPosition(position);
            return position;
        }

        private void SetHoveredPosition(int2? position)
        {
            if(_hoveredPosition == position) return;

            _hoveredPosition = position;
            HoveredPositionChanged?.Invoke(position);
        }

        private void OnGenerateVisualContent(MeshGenerationContext meshGenerationContext)
        {
            if(!_session.IsOpen) return;

            var hover = _tool != null && _hoveredPosition.HasValue ? _tool.GetHover(_hoveredPosition.Value) : LevelEditorHover.None;
            var context = new LevelCanvasContext(_session.Grid, _colors, CreateLayout(), _editors, _session.Selected, _tool, hover, _pointerPosition);

            var painter = meshGenerationContext.painter2D;
            foreach(var layer in _layers)
            {
                layer.Draw(painter, context);
            }
        }
    }
}
