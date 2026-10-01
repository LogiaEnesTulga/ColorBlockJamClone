using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    /// <summary>Composition root of the level editor. Builds the services, tools and panels and switches between screens.</summary>
    public class LevelEditorWindow : EditorWindow
    {
        private const string WindowTitle = "Level Editor";

        private LevelEditorSession _session;
        private LevelSaveService _saveService;
        private LevelEditorHomePanel _homePanel;
        private LevelEditorWorkspace _workspace;

        [MenuItem("Tools/Level Editor")]
        public static void Open()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(1000f, 640f);
        }

        private void CreateGUI()
        {
            _session = new LevelEditorSession();
            var grid = _session.Grid;
            var colors = LevelEditorColors.Load();
            var repository = new LevelFileRepository();
            _saveService = new LevelSaveService(repository);

            var blockEditor = new LevelBlockEditor(grid);
            var doorEditor = new LevelDoorEditor(grid);
            var cellEditor = new LevelCellEditor(grid, blockEditor, doorEditor);
            var resizer = new LevelGridResizer(grid, cellEditor, doorEditor);
            var objectEditors = new LevelObjectEditors(new ILevelObjectEditor[] { blockEditor, doorEditor });
            var documentService = new LevelDocumentService(_session, repository, resizer);

            var tools = new List<ILevelEditorTool>
            {
                new LevelSelectTool(_session, objectEditors),
                new LevelCellTool(_session, cellEditor),
                new LevelBlockTool(_session, blockEditor),
                new LevelDoorTool(_session, doorEditor),
                new LevelMoveTool(_session, objectEditors),
                new LevelDeleteTool(_session, objectEditors, cellEditor),
            };

            var selectionDrawers = new ILevelSelectionDrawer[]
            {
                new LevelBlockSelectionDrawer(_session, blockEditor, colors),
                new LevelDoorSelectionDrawer(_session, doorEditor, colors),
            };

            var canvas = new LevelGridCanvas(_session, colors, objectEditors, LevelCanvasLayers.CreateEditorLayers());
            var inspector = new LevelInspectorPanel(_session, repository, _saveService, resizer, selectionDrawers);
            inspector.NotificationRequested += message => ShowNotification(new GUIContent(message), 1.5f);

            _homePanel = new LevelEditorHomePanel(repository, documentService, colors);
            _homePanel.LevelOpened += ShowWorkspace;

            _workspace = new LevelEditorWorkspace(_session, tools, canvas, inspector);
            _workspace.CloseRequested += TryShowHome;

            var root = rootVisualElement;
            var styleSheet = LevelEditorTheme.LoadStyleSheet();
            if(styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
            root.AddToClassList("le-root");
            root.Add(_homePanel);
            root.Add(_workspace);

            _session.Changed += OnSessionChanged;
            ShowHome();
        }

        public override void SaveChanges()
        {
            var result = _saveService.Save(_session);
            if(!result.IsSuccess)
            {
                EditorUtility.DisplayDialog("Level not saved", result.Message, "OK");
                return;
            }

            base.SaveChanges();
        }

        private void OnSessionChanged()
        {
            hasUnsavedChanges = _session.IsOpen && _session.IsDirty;
            saveChangesMessage = $"Level {_session.LevelId} has unsaved changes. Save before closing?";
            titleContent = new GUIContent(hasUnsavedChanges ? $"{WindowTitle}*" : WindowTitle);
        }

        private void TryShowHome()
        {
            if(_session.IsDirty)
            {
                var discard = EditorUtility.DisplayDialog("Unsaved changes",
                    $"Level {_session.LevelId} has unsaved changes. Discard them?", "Discard", "Keep Editing");
                if(!discard) return;
            }

            ShowHome();
        }

        private void ShowHome()
        {
            _workspace.Close();
            _session.Close();
            _homePanel.Refresh();
            _homePanel.style.display = DisplayStyle.Flex;
            _workspace.style.display = DisplayStyle.None;
        }

        private void ShowWorkspace()
        {
            _homePanel.style.display = DisplayStyle.None;
            _workspace.style.display = DisplayStyle.Flex;
            _workspace.Open();
        }
    }
}
