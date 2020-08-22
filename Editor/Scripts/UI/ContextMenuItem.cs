using UnityEditor;
using UnityEngine;

namespace Doubtech.CloneManager.UI
{
    public class ContextMenuItem {
        GUIContent label;
        GenericMenu.MenuFunction action;
        public ContextMenuItem(string label, string tooltip, GenericMenu.MenuFunction action = null) {
            this.label = new GUIContent(label, tooltip);
            this.action = action;
        }
        public ContextMenuItem(string label, GenericMenu.MenuFunction action = null) {
            this.label = new GUIContent(label);
            this.action = action;
        }
        public GUIContent Label {
            get {
                return label;
            }
        }
        public GenericMenu.MenuFunction Action {
            get {
                return action;
            }
        }
    }
}