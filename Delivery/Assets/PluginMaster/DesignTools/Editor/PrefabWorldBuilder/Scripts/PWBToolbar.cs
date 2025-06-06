﻿/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;


namespace PluginMaster
{
    public class PWBToolbar : UnityEditor.EditorWindow
    {
        #region WINDOW
        private GUISkin _skin = null;
        private GUIStyle _btnStyle = null;
        private bool _wasDocked = false;

        private static PWBToolbar _instance = null;
        public static PWBToolbar instance => _instance;

        private Vector2 _mainScrollPosition = Vector2.zero;

        private const int MIN_SIZE = 32;
        private const int WIDTH = 556;
        private const int HEIGHT = 520;

        [UnityEditor.MenuItem("Tools/Plugin Master/Prefab World Builder/Toolbar...", false, 1100)]
        public static void ShowWindow()
        {
#if UNITY_2021_2_OR_NEWER
            if (!UnityEditor.EditorUtility.DisplayDialog("Toolbar Overlays",
                "PWB tools are available as overlay panels in the scene view window (in Unity 2021.2 or higher). ",
                "Open toolbar anyway", "cancel")) return;
#endif
            var isANewInstance = _instance == null;
            _instance = GetWindow<PWBToolbar>("Tools");
            if (isANewInstance) _instance.position = new Rect(_instance.position.x, _instance.position.y, WIDTH, MIN_SIZE);
        }

        public static void RepaintWindow()
        {
            if (_instance == null) return;
            _instance.Repaint();
        }

        public static void CloseWindow()
        {
            if (_instance != null) _instance.Close();
        }

        private void OnEnable()
        {
            _instance = this;
            _skin = Resources.Load<GUISkin>("PWBSkin");
            if (_skin == null) return;
            UnityEngine.Assertions.Assert.IsNotNull(_skin);
            _btnStyle = _skin.GetStyle("ToggleButton");
            _foldoutButtonStyle = new GUIStyle(_btnStyle);
            LoadToolIcons();
            LoadSnapIcons();
            LoadSelectionToolIcons();
            _axisButtonStyle = _skin.GetStyle("AxisButton");

            _radialAxisButtonStyle = new GUIStyle(_axisButtonStyle);
            _radialAxisButtonStyle.fixedWidth = 12;

            _buttonWithAxesStyle = new GUIStyle(_btnStyle);
            _buttonWithAxesStyle.margin.right = _buttonWithAxesStyle.margin.bottom = 0;

            _simpleBtnStyle = new GUIStyle(_btnStyle);
            _simpleBtnStyle.onNormal = _simpleBtnStyle.normal;

            minSize = new Vector2(MIN_SIZE, MIN_SIZE);
            _wasDocked = !isDocked;
            PWBIO.controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            PWBIO.UpdateOctree();

            ToolManager.OnToolChange += OnToolChange;
            SnapManager.settings.OnDataChanged += Repaint;
        }

        private void OnDisable()
        {
            ToolManager.OnToolChange -= OnToolChange;
            SnapManager.settings.OnDataChanged -= Repaint;
            ToolManager.DeselectTool();
        }

        private void OnDestroy()
        {
            if (PWBCore.staticData.closeAllWindowsWhenClosingTheToolbar) PWBIO.CloseAllWindows(false);
        }
        private void OnGUI()
        {
            if (_skin == null)
            {
                Close();
                return;
            }
#if UNITY_2019_1_OR_NEWER
            UpdateShortcutsTooltips();
#endif
            bool widthGreaterThanHeight = position.width > position.height;
            UpdateFoldoutButtonStyle();
            using (var scrollView = new UnityEditor.EditorGUILayout.ScrollViewScope(_mainScrollPosition, false, false,
                widthGreaterThanHeight ? GUI.skin.horizontalScrollbar : GUIStyle.none,
                widthGreaterThanHeight ? GUIStyle.none : GUI.skin.verticalScrollbar, GUIStyle.none))
            {
                _mainScrollPosition = scrollView.scrollPosition;
                using (position.width > position.height
                    ? (GUI.Scope)new GUILayout.HorizontalScope(_skin.box)
                    : (GUI.Scope)new GUILayout.VerticalScope(_skin.box))
                {
                    _axisButtonStyle.fixedHeight = widthGreaterThanHeight ? 24 : 12;
                    _radialAxisButtonStyle.fixedHeight = _axisButtonStyle.fixedHeight;
                    ToolsGUI();
                    GUILayout.Space(5);
                    SnapGUI();
                    GUILayout.Space(5);
                    if (GUILayout.Button(_helpIcon, _btnStyle)) PWBCore.OpenDocFile();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void Update()
        {
            if (_wasDocked && !isDocked)
            {
                var size = position.width >= position.height ? new Vector2(WIDTH, MIN_SIZE) : new Vector2(MIN_SIZE, HEIGHT);
                position = new Rect(position.position, size);
                _wasDocked = false;
            }
            else if (!_wasDocked && isDocked) _wasDocked = true;
        }

        private bool isDocked
        {
            get
            {
                var isDockedMethod = typeof(UnityEditor.EditorWindow).GetProperty("docked",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static).GetGetMethod(true);
                return (bool)isDockedMethod.Invoke(this, null);
            }
        }

        private void UpdateFoldoutButtonStyle()
        {
            if (position.width >= position.height)
            {
                _foldoutButtonStyle.fixedWidth = 16;
                _foldoutButtonStyle.fixedHeight = 24;
            }
            else
            {
                _foldoutButtonStyle.fixedWidth = 24;
                _foldoutButtonStyle.fixedHeight = 16;
            }
        }

#if UNITY_2019_1_OR_NEWER
        private void UpdateShortcutsTooltips()
        {
            void UpdateTooltipShortcut(GUIContent button, string tooltip, string keyCombination)
            {
                if (keyCombination != string.Empty) button.tooltip = tooltip + " ... " + keyCombination;
            }
            UpdateTooltipShortcut(_floorIcon, "Floor", PWBSettings.shortcuts.toolbarFloorToggle.combination.ToString());
            UpdateTooltipShortcut(_wallIcon, "Wall", PWBSettings.shortcuts.toolbarWallToggle.combination.ToString());
            UpdateTooltipShortcut(_pinIcon, "Pin", PWBSettings.shortcuts.toolbarPinToggle.combination.ToString());
            UpdateTooltipShortcut(_brushIcon, "Brush", PWBSettings.shortcuts.toolbarBrushToggle.combination.ToString());
            UpdateTooltipShortcut(_eraserIcon, "Eraser", PWBSettings.shortcuts.toolbarEraserToggle.combination.ToString());
            UpdateTooltipShortcut(_physicsIcon, "Gravity Brush",
                PWBSettings.shortcuts.toolbarGravityToggle.combination.ToString());
            UpdateTooltipShortcut(_extrudeIcon, "Extrude", PWBSettings.shortcuts.toolbarExtrudeToggle.combination.ToString());
            UpdateTooltipShortcut(_lineIcon, "Line", PWBSettings.shortcuts.toolbarLineToggle.combination.ToString());
            UpdateTooltipShortcut(_shapeIcon, "Shape", PWBSettings.shortcuts.toolbarShapeToggle.combination.ToString());
            UpdateTooltipShortcut(_tilingIcon, "Tiling", PWBSettings.shortcuts.toolbarTilingToggle.combination.ToString());
            UpdateTooltipShortcut(_selectionIcon, "Selection",
                PWBSettings.shortcuts.toolbarSelectionToggle.combination.ToString());
            UpdateTooltipShortcut(_selectionIcon, "Circle Selection",
                PWBSettings.shortcuts.toolbarCircleSelectToggle.combination.ToString());
            UpdateTooltipShortcut(_mirrorIcon, "Mirror", PWBSettings.shortcuts.toolbarMirrorToggle.combination.ToString());
            UpdateTooltipShortcut(_lockGridIcon, "Lock the grid origin in place",
                PWBSettings.shortcuts.gridToggleLock.combination.ToString());
            UpdateTooltipShortcut(_unlockGridIcon, "Unlock the grid origin",
                PWBSettings.shortcuts.gridToggleLock.combination.ToString());
        }
#endif
        #endregion

        #region TOOLS
        private GUIContent _floorIcon = null;
        private GUIContent _wallIcon = null;

        private GUIContent _pinIcon = null;
        private GUIContent _brushIcon = null;
        private GUIContent _eraserIcon = null;
        private GUIContent _physicsIcon = null;
        private GUIContent _extrudeIcon = null;
        private GUIContent _lineIcon = null;
        private GUIContent _shapeIcon = null;
        private GUIContent _tilingIcon = null;
        private GUIContent _selectionIcon = null;
        private GUIContent _circleSelectIcon = null;
        private GUIContent _mirrorIcon = null;
        private GUIContent _replaceIcon = null;
        private GUIContent _helpIcon = null;
        private GUIContent _propertiesIcon = null;

        private bool _toolChanged = false;
        private void OnToolChange(ToolManager.PaintTool prevTool)
        {
            _toolChanged = true;
            PWBIO.OnToolChange(prevTool);
        }

        private void LoadToolIcons()
        {
            _floorIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Floors"), "Floor");
            _wallIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Walls"), "Wall");

            _pinIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Pin"), "Pin");
            _brushIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Brush"), "Brush");
            _eraserIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Eraser"), "Eraser");
            _physicsIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/GravityTool"), "Gravity Brush");
            _extrudeIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Extrude"), "Extrude");
            _lineIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Line"), "Line");
            _shapeIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Shape"), "Shape");
            _tilingIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Tiling"), "Tiling");
            _selectionIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Selection"), "Selection");
            _circleSelectIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/CircleSelect"), "Circle Select");
            _mirrorIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Mirror"), "Mirror");
            _replaceIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Replace"), "Replacer");
            _helpIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Help"), "Documentation");
            _propertiesIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ToolProperties"), "Tool Properties");
        }

        private void ToolsGUI()
        {
            using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
            {
                var newtool = ToolManager.tool;

                var floorSelected = newtool == ToolManager.PaintTool.FLOOR;
                newtool = (GUILayout.Toggle(floorSelected, _floorIcon, _btnStyle)
                ? ToolManager.PaintTool.FLOOR : (floorSelected ? ToolManager.PaintTool.NONE : newtool));

                var wallSelected = newtool == ToolManager.PaintTool.WALL;
                newtool = (GUILayout.Toggle(wallSelected, _wallIcon, _btnStyle)
                ? ToolManager.PaintTool.WALL : (wallSelected ? ToolManager.PaintTool.NONE : newtool));

                GUILayout.Space(5);

                var pinSelected = newtool == ToolManager.PaintTool.PIN;
                newtool = (GUILayout.Toggle(pinSelected, _pinIcon, _btnStyle)
                ? ToolManager.PaintTool.PIN : (pinSelected ? ToolManager.PaintTool.NONE : newtool));

                var brushSelected = newtool == ToolManager.PaintTool.BRUSH;
                newtool = (GUILayout.Toggle(brushSelected, _brushIcon, _btnStyle)
                ? ToolManager.PaintTool.BRUSH : (brushSelected ? ToolManager.PaintTool.NONE : newtool));

                var gravitySelected = newtool == ToolManager.PaintTool.GRAVITY;
                newtool = (GUILayout.Toggle(gravitySelected, _physicsIcon, _btnStyle)
                ? ToolManager.PaintTool.GRAVITY : (gravitySelected ? ToolManager.PaintTool.NONE : newtool));

                var lineSelected = newtool == ToolManager.PaintTool.LINE;
                newtool = (GUILayout.Toggle(lineSelected, _lineIcon, _btnStyle)
                ? ToolManager.PaintTool.LINE : (lineSelected ? ToolManager.PaintTool.NONE : newtool));

                var shapeSelected = newtool == ToolManager.PaintTool.SHAPE;
                newtool = (GUILayout.Toggle(shapeSelected, _shapeIcon, _btnStyle)
                ? ToolManager.PaintTool.SHAPE : (shapeSelected ? ToolManager.PaintTool.NONE : newtool));

                var tilingSelected = newtool == ToolManager.PaintTool.TILING;
                newtool = (GUILayout.Toggle(tilingSelected, _tilingIcon, _btnStyle)
                ? ToolManager.PaintTool.TILING : (tilingSelected ? ToolManager.PaintTool.NONE : newtool));

                var replaceSelected = newtool == ToolManager.PaintTool.REPLACER;
                newtool = (GUILayout.Toggle(replaceSelected, _replaceIcon, _btnStyle)
                ? ToolManager.PaintTool.REPLACER : (replaceSelected ? ToolManager.PaintTool.NONE : newtool));

                var eraserSelected = newtool == ToolManager.PaintTool.ERASER;
                newtool = (GUILayout.Toggle(eraserSelected, _eraserIcon, _btnStyle)
                ? ToolManager.PaintTool.ERASER : (eraserSelected ? ToolManager.PaintTool.NONE : newtool));
                
                GUILayout.Space(5);
                if (GUILayout.Button(_propertiesIcon, _btnStyle)) ToolProperties.ShowWindow();
                GUILayout.Space(5);

                var selectionSelected = newtool == ToolManager.PaintTool.SELECTION;
                newtool = (GUILayout.Toggle(selectionSelected, _selectionIcon, _buttonWithAxesStyle)
                ? ToolManager.PaintTool.SELECTION : (selectionSelected ? ToolManager.PaintTool.NONE : newtool));
                GUILayout.Space(1);
                bool TRSChanged = false;
                using (new UnityEditor.EditorGUI.DisabledGroupScope(!selectionSelected))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        using (var checkTRS = new UnityEditor.EditorGUI.ChangeCheckScope())
                        {
                            SelectionToolManager.settings.move = GUILayout.Toggle(SelectionToolManager.settings.move,
                                _tIcon, _axisButtonStyle);
                            SelectionToolManager.settings.rotate = GUILayout.Toggle(SelectionToolManager.settings.rotate,
                                _rIcon, _axisButtonStyle);
                            SelectionToolManager.settings.scale = GUILayout.Toggle(SelectionToolManager.settings.scale,
                                _sIcon, _axisButtonStyle);
                            if (checkTRS.changed)
                            {
                                TRSChanged = true;
                                UnityEditor.SceneView.RepaintAll();
                                ToolProperties.RepainWindow();
                            }
                        }
                    }
                }

                var circleSelectSelected = newtool == ToolManager.PaintTool.CIRCLE_SELECT;
                newtool = (GUILayout.Toggle(circleSelectSelected, _circleSelectIcon, _btnStyle)
                ? ToolManager.PaintTool.CIRCLE_SELECT : (circleSelectSelected ? ToolManager.PaintTool.NONE : newtool));

                var extrudeSelected = newtool == ToolManager.PaintTool.EXTRUDE;
                newtool = (GUILayout.Toggle(extrudeSelected, _extrudeIcon, _btnStyle)
                ? ToolManager.PaintTool.EXTRUDE : (extrudeSelected ? ToolManager.PaintTool.NONE : newtool));

                var mirrorSelected = newtool == ToolManager.PaintTool.MIRROR;
                newtool = (GUILayout.Toggle(mirrorSelected, _mirrorIcon, _btnStyle)
                ? ToolManager.PaintTool.MIRROR : (mirrorSelected ? ToolManager.PaintTool.NONE : newtool));

                if ((check.changed || _toolChanged) && !TRSChanged)
                {
                    _toolChanged = false;
                    ToolManager.tool = newtool;
                }
            }
        }

        #endregion

        #region SELECTION TOOL
        private GUIContent _tIcon = null;
        private GUIContent _rIcon = null;
        private GUIContent _sIcon = null;

        private void LoadSelectionToolIcons()
        {
            _tIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/T"));
            _rIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/R"));
            _sIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/S"));
        }
        #endregion

        #region SNAP
        private GUIContent _showGridIcon = null;
        private GUIContent _enableSnappingIcon = null;
        private GUIContent _lockGridIcon = null;
        private GUIContent _unlockGridIcon = null;
        private GUIContent _snapSettingsIcon = null;
        private GUIContent _gridIcon = null;
        private GUIContent _radialGridIcon = null;
        private GUIContent _xIcon = null;
        private GUIContent _yIcon = null;
        private GUIContent _zIcon = null;
        private GUIStyle _axisButtonStyle = null;
        private GUIStyle _buttonWithAxesStyle = null;
        private GUIStyle _simpleBtnStyle = null;
        private GUIStyle _radialAxisButtonStyle = null;
        private GUIContent _cIcon = null;
        private bool _showGridTools = true;
        private GUIContent _showGridToolsIcon = null;
        private GUIContent _hideGridToolsIcon = null;
        private GUIContent _showGridToolsHIcon = null;
        private GUIContent _hideGridToolsHIcon = null;
        private GUIStyle _foldoutButtonStyle = null;
        private GUIContent _boundsSnappingIcon = null;
        private void LoadSnapIcons()
        {
            _showGridIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ShowGrid"), "Show grid");
            _enableSnappingIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/SnapOn"), "Enable snapping");
            _lockGridIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/LockGrid"),
                "Lock the grid origin in place");
            _unlockGridIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/UnlockGrid"),
                "Unlock the grid origin");
            _snapSettingsIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/SnapSettings"),
                "Grid and Snapping Settings");
            _gridIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Grid"), "Grid");
            _radialGridIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/RadialGrid"), "Radial Grid");
            _xIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/X"));
            _yIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Y"));
            _zIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/Z"));
            _cIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/C"));
            _showGridToolsIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ShowGridTools"));
            _hideGridToolsIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/HideGridTools"));
            _showGridToolsHIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/ShowGridToolsH"));
            _hideGridToolsHIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/HideGridToolsH"));
            _boundsSnappingIcon = new GUIContent(Resources.Load<Texture2D>("Sprites/BoundsSnapping"),
                "Enable bounds snapping");
        }

        private void SnapGUI()
        {
            var foldoutIcon = position.width > position.height
                    ? _showGridTools ? _hideGridToolsHIcon : _showGridToolsHIcon
                    : _showGridTools ? _hideGridToolsIcon : _showGridToolsIcon;
            if (!_showGridTools) if (GUILayout.Button(foldoutIcon, _foldoutButtonStyle)) _showGridTools = true;
            if (!_showGridTools) return;
            var settings = SnapManager.settings;
            using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
            {
                settings.radialGridEnabled = GUILayout.Toggle(settings.radialGridEnabled,
                    settings.radialGridEnabled ? _gridIcon : _radialGridIcon, _simpleBtnStyle);
                if (check.changed) SnapSettingsWindow.RepaintWindow();
            }
            settings.snappingEnabled = GUILayout.Toggle(settings.snappingEnabled,
                _enableSnappingIcon, _buttonWithAxesStyle);
            GUILayout.Space(1);
            using (new UnityEditor.EditorGUI.DisabledGroupScope(!settings.snappingEnabled))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (settings.radialGridEnabled)
                    {
                        settings.snapToRadius = GUILayout.Toggle(settings.snapToRadius,
                            _rIcon, position.width > position.height ? _axisButtonStyle : _radialAxisButtonStyle);
                        settings.snapToCircunference = GUILayout.Toggle(settings.snapToCircunference,
                            _cIcon, position.width > position.height ? _axisButtonStyle : _radialAxisButtonStyle);
                    }
                    else
                    {
                        settings.snappingOnX = GUILayout.Toggle(settings.snappingOnX,
                            _xIcon, _axisButtonStyle);
                        SnapManager.settings.snappingOnY = GUILayout.Toggle(settings.snappingOnY,
                            _yIcon, _axisButtonStyle);
                        settings.snappingOnZ = GUILayout.Toggle(settings.snappingOnZ,
                            _zIcon, _axisButtonStyle);
                    }
                }
            }

            settings.visibleGrid = GUILayout.Toggle(settings.visibleGrid,
                _showGridIcon, _buttonWithAxesStyle);
            GUILayout.Space(1);
            using (new UnityEditor.EditorGUI.DisabledGroupScope(!settings.visibleGrid))
            {
                using (new GUILayout.HorizontalScope())
                {
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        var showGridX = GUILayout.Toggle(settings.gridOnX, _xIcon, _axisButtonStyle);
                        if (check.changed && showGridX) settings.gridOnX = showGridX;
                    }
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        var showGridY = GUILayout.Toggle(settings.gridOnY, _yIcon, _axisButtonStyle);
                        if (check.changed && showGridY) settings.gridOnY = showGridY;
                    }
                    using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                    {
                        var showGridZ = GUILayout.Toggle(settings.gridOnZ, _zIcon, _axisButtonStyle);
                        if (check.changed && showGridZ) settings.gridOnZ = showGridZ;
                    }
                }
            }

            using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
            {
                settings.lockedGrid = GUILayout.Toggle(settings.lockedGrid,
                    settings.lockedGrid ? _lockGridIcon : _unlockGridIcon, _btnStyle);
                if (check.changed) SnapSettingsWindow.RepaintWindow();
            }
            settings.boundsSnapping = GUILayout.Toggle(settings.boundsSnapping, _boundsSnappingIcon, _btnStyle);
            if (GUILayout.Button(_snapSettingsIcon, _btnStyle)) SnapSettingsWindow.ShowWindow();
            if (GUILayout.Button(foldoutIcon, _foldoutButtonStyle)) _showGridTools = false;
        }
        #endregion
    }
}