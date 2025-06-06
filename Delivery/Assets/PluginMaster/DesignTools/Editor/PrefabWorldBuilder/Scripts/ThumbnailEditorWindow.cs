﻿/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

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
    public abstract class ThumbnailEditorWindow : UnityEditor.EditorWindow
    {
        public const string UNDO_CMD = "Edit Thumbnail";
        [SerializeField] private PaletteManager _paletteManager = null;

        protected static BrushSettings _brush = null;
        public static int _settingsIdx = 0;

        protected Texture2D _thumbnail = null;
        [SerializeField] protected ThumbnailSettings _settings = null;

        private GUIStyle _nextBtnStyle = null;
        private GUIStyle _prevBtnStyle = null;

        private static ThumbnailEditorWindow _instance = null;
        public static void ShowWindow(BrushSettings brush, int brushIdx)
        {
            _brush = brush;
            _settingsIdx = brushIdx;
            if (_instance == null) _instance = brush is MultibrushSettings
                    ? (ThumbnailEditorWindow)GetWindow<ThumbnailEditorCommon>(true,
                    "Thumbnail Editor - " + (brush as MultibrushSettings).name)
                    : (ThumbnailEditorWindow)GetWindow<SubThumbnailEditor>(true,
                    "Thumbnail Editor - " + (brush as MultibrushItemSettings).prefab.name);
            else _instance.Initialize(brush);
            _instance.Repaint();
        }

        protected virtual void Initialize(BrushSettings brush)
        {
            if (_thumbnail == null) _thumbnail = new Texture2D(ThumbnailUtils.SIZE, ThumbnailUtils.SIZE);
            _settings = new ThumbnailSettings(brush.thumbnailSettings);
            titleContent = new GUIContent(brush is MultibrushSettings
                ? "Thumbnail Editor - " + (brush as MultibrushSettings).name
                : "Thumbnail Editor - " + (brush as MultibrushItemSettings).prefab.name);
        }

        protected abstract void InitializePreview();

        protected virtual void OnEnable()
        {
            if (_brush == null) return;
            _paletteManager = PaletteManager.instance;
            _thumbnail = new Texture2D(ThumbnailUtils.SIZE, ThumbnailUtils.SIZE);
            _settings = new ThumbnailSettings(_brush.thumbnailSettings);
            Initialize(_brush);
            UnityEditor.Undo.undoRedoPerformed += OnUndoPerformed;
            _nextBtnStyle = new GUIStyle();
            _nextBtnStyle.normal.background = Resources.Load<Texture2D>("Sprites/Next");
            _nextBtnStyle.fixedWidth = 10;
            _nextBtnStyle.fixedHeight = 38;
            _prevBtnStyle = new GUIStyle(_nextBtnStyle);
            _prevBtnStyle.normal.background = Resources.Load<Texture2D>("Sprites/Prev");
        }

        private void OnDisable() => UnityEditor.Undo.undoRedoPerformed -= OnUndoPerformed;

        private void OnUndoPerformed()
        {
            Initialize(_brush);
            Repaint();
        }

        public static void ThumbnailSettingsGUI(ThumbnailSettings settings)
        {
            UnityEditor.EditorGUIUtility.labelWidth = 110;
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                settings.backgroudColor = UnityEditor.EditorGUILayout.ColorField("Background color", settings.backgroudColor);
            }
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                settings.lightColor = UnityEditor.EditorGUILayout.ColorField("Light color", settings.lightColor);
                settings.lightIntensity
                    = UnityEditor.EditorGUILayout.Slider("Light intensity", settings.lightIntensity, 0.1f, 2);
            }
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                settings.zoom = UnityEditor.EditorGUILayout.Slider("Zoom", settings.zoom, 0.5f, 10);
                settings.targetEuler = UnityEditor.EditorGUILayout.Vector3Field("Rotation", settings.targetEuler);
            }
        }

        protected virtual void SettingsGUI(ThumbnailSettings settings)
        {
            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    settings.useCustomImage = UnityEditor.EditorGUILayout.ToggleLeft("Use a custom image",
                        settings.useCustomImage);
                    if (check.changed && !settings.useCustomImage) InitializePreview();
                }
                using (new UnityEditor.EditorGUI.DisabledGroupScope(!settings.useCustomImage))
                {
                    if (GUILayout.Button("Load image"))
                    {
                        var filePath = UnityEditor.EditorUtility.OpenFilePanelWithFilters("Load thumbnail image...",
                            Application.dataPath, new string[] { "Image files", "png,jpg,jpeg" });
                        if (System.IO.File.Exists(filePath))
                            _thumbnail = ThumbnailUtils.ScaleImage(filePath);
                    }
                }
            }
            using (new UnityEditor.EditorGUI.DisabledGroupScope(settings.useCustomImage))
            {
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    ThumbnailSettingsGUI(settings);
                    if (check.changed) OnSettingsChange();
                }
            }
        }

        private void Buttons()
        {
            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Factory Reset"))
                {
                    _settings = new ThumbnailSettings();
                    if (_brush.isAsset2D) _settings.Copy(ThumbnailSettings.GetDefaultTAsset2DThumbnailSettings());
                    OnSettingsChange();
                    InitializePreview();
                }
                if (GUILayout.Button("Cancel")) Close();
                if (GUILayout.Button("Appy")) OnApply();
            }
        }

        protected virtual void PreviewMouseEvents(Rect previeRect)
        {
            if (!previeRect.Contains(Event.current.mousePosition)) return;
            if (_settings.useCustomImage) return;
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 1
                && Event.current.delta != Vector2.zero)
            {
                if (!Event.current.control && !Event.current.shift)
                {
                    var rot = Quaternion.Euler(_settings.targetEuler);
                    _settings.targetEuler = (Quaternion.AngleAxis(Event.current.delta.y, Vector3.left)
                        * Quaternion.AngleAxis(Event.current.delta.x, Vector3.down) * rot).eulerAngles;
                    OnSettingsChange();
                    Event.current.Use();
                }
                else if (Event.current.control && !Event.current.shift)
                {
                    var delta = Event.current.delta / 128;
                    delta.y = -delta.y;
                    _settings.targetOffset = Vector2.Min(Vector2.one,
                        Vector2.Max(_settings.targetOffset + delta, -Vector2.one));
                    OnSettingsChange();
                    Event.current.Use();
                }
                else if (!Event.current.control && Event.current.shift)
                {
                    Vector3 centerDir = (previeRect.center - Event.current.mousePosition) * 5 / 128;
                    centerDir.z = 5;
                    var rot = Quaternion.LookRotation(centerDir);
                    var euler = rot.eulerAngles;
                    _settings.lightEuler = new Vector2(-euler.x, euler.y);
                    OnSettingsChange();
                    Event.current.Use();
                }
            }
            if (Event.current.isScrollWheel)
            {
                var scrollSign = Mathf.Sign(Event.current.delta.y);
                _settings.zoom += scrollSign * 0.1f;
                OnSettingsChange();
                Event.current.Use();
            }
        }

        private void OnGUI()
        {
            if (_brush == null)
            {
                Close();
                return;
            }
            UnityEditor.EditorGUIUtility.wideMode = true;
            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(GUIContent.none, _prevBtnStyle)) ShowPrev();
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Label(new GUIContent(_thumbnail));
                var rect = GUILayoutUtility.GetLastRect();
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(GUIContent.none, _nextBtnStyle)) ShowNext();
                    GUILayout.FlexibleSpace();
                }
                PreviewMouseEvents(rect);
                GUILayout.FlexibleSpace();
            }
            SettingsGUI(_settings);
            Buttons();
        }
        protected abstract void OnSettingsChange();
        protected abstract void OnApply();
        protected abstract void ShowNext();
        protected abstract void ShowPrev();
    }

    public class ThumbnailEditorCommon : ThumbnailEditorWindow
    {
        [System.Serializable]
        private class SubThumbnailData
        {
            public MultibrushItemSettings multibrushItem = null;
            public ThumbnailSettings settings = null;
            public Texture2D texture = null;
            public bool include = true;
            public GameObject prefab = null;
            public bool overwrite = false;
        }
        [SerializeField]
        private System.Collections.Generic.List<SubThumbnailData> _subThumbnails
            = new System.Collections.Generic.List<SubThumbnailData>();
        protected override void OnEnable()
        {
            base.OnEnable();
            maxSize = minSize = new Vector2(300, 444);
        }
        protected override void Initialize(BrushSettings brush)
        {
            _brush = brush;
            var brushSettings = brush as MultibrushSettings;
            if (brushSettings == null)
            {
                Close();
                SubThumbnailEditor.ShowWindow(brush, _settingsIdx);
                return;
            }
            base.Initialize(brush);
            InitializePreview();
        }

        protected override void InitializePreview()
        {
            if (_settings.useCustomImage)
            {
                _brush.LoadThumbnailFromFile();
                ThumbnailUtils.CopyTexture(_brush.thumbnail,out _thumbnail);
                return;
            }
            var brushSettings = _brush as MultibrushSettings;
            _subThumbnails.Clear();
            var brushItems = (_brush as MultibrushSettings).items;
            foreach (var item in brushItems)
            {
                if (item.prefab == null) continue;
                var subThumbnail = new SubThumbnailData();
                subThumbnail.multibrushItem = item;
                subThumbnail.settings = new ThumbnailSettings(item.thumbnailSettings);
                subThumbnail.texture = new Texture2D(ThumbnailUtils.SIZE, ThumbnailUtils.SIZE);
                subThumbnail.include = item.includeInThumbnail;
                subThumbnail.prefab = item.prefab;
                subThumbnail.overwrite = item.overwriteSettings;
                _subThumbnails.Add(subThumbnail);
                if (item.thumbnailSettings.useCustomImage)
                {
                    item.LoadThumbnailFromFile();
                    subThumbnail.texture = item.thumbnail;
                }
                else ThumbnailUtils.UpdateThumbnail(subThumbnail.overwrite ? subThumbnail.settings : _settings,
                        subThumbnail.texture, subThumbnail.prefab, subThumbnail.multibrushItem.thumbnailPath, false);
            }
            var included = GetIncluded();
            ThumbnailUtils.UpdateThumbnail(_settings, _thumbnail, included, brushSettings.thumbnailPath, false);
        }

        private Texture2D[] GetIncluded()
        {
            var included = new System.Collections.Generic.List<Texture2D>();
            foreach (var item in _subThumbnails)
            {
                if (!item.include) continue;
                included.Add(item.texture);
            }
            return included.ToArray();
        }

        protected override void OnApply()
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, UNDO_CMD);
            _brush.thumbnailSettings = _settings;
            if (_settings.useCustomImage) _brush.SetCustomThumbnailTexture(_thumbnail, savePng: true);
            else
            {
                foreach (var subThumbnail in _subThumbnails) subThumbnail.multibrushItem.thumbnailSettings = subThumbnail.settings;
                ThumbnailUtils.UpdateThumbnail(_brush as MultibrushSettings, updateItemThumbnails: true, savePng: true);
            }
            PaletteManager.selectedPalette.Save();
            PrefabPalette.instance.OnPaletteChange();

        }

        protected override void OnSettingsChange()
        {
            foreach (var subThumbnail in _subThumbnails)
            {
                if (subThumbnail.multibrushItem.overwriteThumbnailSettings) continue;
                ThumbnailUtils.UpdateThumbnail(subThumbnail.overwrite ? subThumbnail.settings : _settings,
                    subThumbnail.texture, subThumbnail.prefab, subThumbnail.multibrushItem.thumbnailPath, savePng: false);
            }
            var included = GetIncluded();
            ThumbnailUtils.UpdateThumbnail(_settings, _thumbnail, included, _brush.thumbnailPath, savePng: false);
        }

        protected override void ShowNext()
        {
            var brushCount = PaletteManager.selectedPalette.brushCount;
            _settingsIdx = (_settingsIdx + 1) % brushCount;
            Initialize(PaletteManager.selectedPalette.GetBrush(_settingsIdx));
        }

        protected override void ShowPrev()
        {
            var brushCount = PaletteManager.selectedPalette.brushCount;
            _settingsIdx = (_settingsIdx + brushCount - 1) % brushCount;
            Initialize(PaletteManager.selectedPalette.GetBrush(_settingsIdx));
        }
    }

    public class SubThumbnailEditor : ThumbnailEditorWindow
    {
        [SerializeField] private MultibrushItemSettings _itemClone = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            maxSize = minSize = new Vector2(300, 470);
        }

        protected override void PreviewMouseEvents(Rect previewRect)
        {
            if (_itemClone.overwriteThumbnailSettings) base.PreviewMouseEvents(previewRect);
        }

        protected override void SettingsGUI(ThumbnailSettings settings)
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    var overwrite = UnityEditor.EditorGUILayout.ToggleLeft("Overwrite common settings",
                        _itemClone.overwriteThumbnailSettings);
                    if (check.changed)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(this, UNDO_CMD);
                        _itemClone.overwriteThumbnailSettings = overwrite;
                        _settings.Copy(_itemClone.thumbnailSettings);
                        ThumbnailUtils.UpdateThumbnail(_settings, _thumbnail, _itemClone.prefab,
                            thumbnailPath: null, savePng: false);
                    }
                }
            }
            using (new UnityEditor.EditorGUI.DisabledGroupScope(!_itemClone.overwriteThumbnailSettings))
                base.SettingsGUI(settings);
        }

        protected override void Initialize(BrushSettings brush)
        {
            _brush = brush;
            var item = brush as MultibrushItemSettings;
            var nullItem = item == null;
            if (!nullItem) nullItem = item.prefab == null;
            if (nullItem)
            {
                Close();
                ThumbnailEditorCommon.ShowWindow(brush, _settingsIdx);
                return;
            }
            _itemClone = item.Clone() as MultibrushItemSettings;
            base.Initialize(brush);
            InitializePreview();
        }

        protected override void InitializePreview()
        {
            if (_settings.useCustomImage)
            {
                _brush.LoadThumbnailFromFile();
                ThumbnailUtils.CopyTexture(_brush.thumbnail,out _thumbnail);
                return;
            }
            var item = _brush as MultibrushItemSettings;
            ThumbnailUtils.UpdateThumbnail(_settings, _thumbnail, item.prefab, item.thumbnailPath, savePng: false);
        }
        protected override void OnApply()
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, UNDO_CMD);
            _brush.thumbnailSettings = _settings;
            var item = _brush as MultibrushItemSettings;
            item.overwriteThumbnailSettings = _itemClone.overwriteThumbnailSettings;
            if (_settings.useCustomImage) _brush.SetCustomThumbnailTexture(_thumbnail, savePng: true);
            else ThumbnailUtils.UpdateThumbnail(item, savePng: true, updateParent: false);
            ThumbnailUtils.UpdateThumbnail(item.parentSettings, updateItemThumbnails: false, savePng: true);
            PaletteManager.selectedPalette.Save();
        }

        protected override void OnSettingsChange()
        {
            if (_itemClone.prefab == null) return;
            ThumbnailUtils.UpdateThumbnail(_settings, _thumbnail,
                _itemClone.prefab, _itemClone.thumbnailPath, savePng: false);
        }

        protected override void ShowNext()
        {
            var itemCount = PaletteManager.selectedBrush.itemCount;
            _settingsIdx = (_settingsIdx + 1) % itemCount;
            Initialize(PaletteManager.selectedBrush.GetItemAt(_settingsIdx));
        }

        protected override void ShowPrev()
        {
            var itemCount = PaletteManager.selectedBrush.itemCount;
            _settingsIdx = (_settingsIdx + itemCount - 1) % itemCount;
            Initialize(PaletteManager.selectedBrush.GetItemAt(_settingsIdx));
        }
    }
}
