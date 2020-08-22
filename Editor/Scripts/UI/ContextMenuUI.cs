using UnityEditor;
using UnityEngine;

namespace Doubtech.CloneManager.UI
{
    public class ContextMenuUI
    {
        public static void DrawContextMenu(ContextMenuItem[] actions, GenericMenu.MenuFunction click = null) {
            Rect clickArea = GUILayoutUtility.GetLastRect();
            if (clickArea.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if(null != click) click();
                        break;
                    case EventType.ContextClick:
                        GenericMenu menu = new GenericMenu();
                        foreach (ContextMenuItem item in actions)
                        {
                            if (item.Label.text == "-")
                            {
                                menu.AddSeparator("");
                            }
                            else
                            {
                                menu.AddItem(item.Label, false, item.Action);
                            }
                        }

                        menu.ShowAsContext();
                        Event.current.Use();
                        break;
                }
            }
        }
    }
}