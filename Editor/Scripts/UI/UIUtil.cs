using UnityEditor;
using UnityEngine;

namespace Doubtech.CloneManager.UI
{
    public class UIUtil
    {
        public readonly static GUIStyle borderlessButtonStyle;
        public readonly static GUIStyle clickableLabelStyle;
        
        static UIUtil()
        {
            clickableLabelStyle = new GUIStyle();
            var b = clickableLabelStyle.border;
            b.left = 0;
            b.top = 0;
            b.right = 0;
            b.bottom = 0;
            clickableLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            clickableLabelStyle.richText = true;
            
            borderlessButtonStyle = new GUIStyle();
            b = clickableLabelStyle.border;
            b.left = 0;
            b.top = 0;
            b.right = 0;
            b.bottom = 0;
            borderlessButtonStyle.padding = new RectOffset(2, 2, 2, 2);
            borderlessButtonStyle.normal.textColor =
                EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }
        
        public static bool UnityButton(string textureName, int width = -1, int height = -1, string tooltip = "")
        {
            GUIContent guiContent = EditorGUIUtility.IconContent(textureName, tooltip);
            return GUILayout.Button(guiContent,
                GUILayout.Width(width > 0 ? width : EditorGUIUtility.singleLineHeight),
                GUILayout.Height(height > 0 ? height : EditorGUIUtility.singleLineHeight));
        }
        public static bool BorderlessUnityButton(string textureName, int width = -1, int height = -1, string tooltip = "")
        {
            if(-1 == width) width = (int) EditorGUIUtility.singleLineHeight + 4;
            if(-1 == height) height = (int) EditorGUIUtility.singleLineHeight + 4;

            GUIContent guiContent = EditorGUIUtility.IconContent(textureName, tooltip);
            return GUILayout.Button(guiContent, borderlessButtonStyle, GUILayout.Width(width), GUILayout.Height(height));
        }
    }
}