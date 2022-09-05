using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace GameKit.Editor 
{
    
    public class GenericSelector<T> : OdinSelector<T>
    {
        private static readonly System.Reflection.FieldInfo InspectorNameAttribute_displayName;
    
        private static readonly string title = typeof(T).Name.SplitPascalCase();
        private float maxEnumLabelWidth;
        private ulong curentMouseOverValue;
        private bool wasMouseDown;

        private ISelectionGUI<T> _popUpOdinSelector;

        public GenericSelector(ISelectionGUI<T> popUpSelector)
        {
            _popUpOdinSelector = popUpSelector;
            this.maxEnumLabelWidth = Mathf.Max(this.maxEnumLabelWidth,
                                               SirenixGUIStyles.Label.CalcSize(new GUIContent(this.Title + "                      ")).x);
        }

        /// <summary>
        /// By default, the enum type will be drawn as the title for the selector. No title will be drawn if the string is null or empty.
        /// </summary>
        public override string Title => GlobalConfig<GeneralDrawerConfig>.Instance.DrawEnumTypeTitle ? title : null;


        /// <summary>Populates the tree with all enum values.</summary>
        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Selection.SupportsMultiSelect = false;
            tree.Config.DrawSearchToolbar      = true;

            _popUpOdinSelector.GetItemsWithContent().ForEach(e =>
                                                             {
                                                                 tree.Add(e.Item2.text, e.Item1);
                                                             });
            tree.EnumerateTree().ForEach(x => x.OnDrawItem += this.DrawEnumItem);
            tree.EnumerateTree().ForEach(x => x.OnDrawItem += this.DrawEnumInfo);
        }

        private void DrawEnumInfo(OdinMenuItem obj)
        {
           
            if (!(obj.Value is T enumMember)) return;
            GUI.DrawTexture(obj.Rect.Padding(5f, 3f).AlignRight(16f).AlignCenterY(16f), (Texture) EditorIcons.ConsoleInfoIcon);
            GUI.Label(obj.Rect, new GUIContent("", obj.SmartName));
        }

        private void DrawEnumItem(OdinMenuItem obj)
        {
            if (Event.current.type == UnityEngine.EventType.MouseDown && obj.Rect.Contains(Event.current.mousePosition))
            {
                obj.Select();
                Event.current.Use();
                this.wasMouseDown = true;
            }

            if (this.wasMouseDown) GUIHelper.RequestRepaint();

            if (this.wasMouseDown && Event.current.type == UnityEngine.EventType.MouseDrag && obj.Rect.Contains(Event.current.mousePosition))
                obj.Select();

            if (Event.current.type != UnityEngine.EventType.MouseUp) return;
            this.wasMouseDown = false;
            if (!obj.IsSelected || !obj.Rect.Contains(Event.current.mousePosition)) return;
            obj.MenuTree.Selection.ConfirmSelection();
        }

        /// <summary>
        /// When ShowInPopup is called, without a specified window width, this method gets called.
        /// Here you can calculate and give a good default width for the popup.
        /// The default implementation returns 0, which will let the popup window determine the width itself. This is usually a fixed value.
        /// </summary>
        protected override float DefaultWindowWidth() => Mathf.Clamp(this.maxEnumLabelWidth + 50f, 160f, 400f);

        /// <summary>Gets the currently selected enum value.</summary>
        public override IEnumerable<T> GetCurrentSelection()
        {
            yield return _popUpOdinSelector.CurrentValue;
        }

        /// <summary>Selects an enum.</summary>
        public override void SetSelection(T selected)
        {
            _popUpOdinSelector.SetCurrent(selected);
        }
    }
}