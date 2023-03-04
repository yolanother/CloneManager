using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Doubtech.CloneManager.UI;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Doubtech.CloneManager
{
    public class CloneManagerEditorWindow : EditorWindow
    {
        private static CloneData clones = new CloneData();
        private Vector2 scrollView;

        [MenuItem("Tools/Clone Manager/Manager")]
        static void Init() {
            var window = (CloneManagerEditorWindow) GetWindow(typeof(CloneManagerEditorWindow));
            window.titleContent = new GUIContent("Clone Manager");
            window.Show();
        }
        
        [MenuItem("Tools/Clone Manager/Create Clone", false, 1)]
        static void CreateClone()
        {
            var parentProject = new DirectoryInfo(Application.dataPath).Parent;
            var dir = parentProject.Parent.FullName;
            var name = parentProject.Name + (clones.Count > 0 ? " (Clone " + clones.Count + ")" : " (Clone)");
            string path = EditorUtility.SaveFilePanel("Create Clone", dir, name, "");
            var clone = AddCloneData(path);
            if(null != clone) GenerateClone(clone);
        }

        [MenuItem("Tools/Clone Manager/Add Existing Clone", false, 1)]
        static void AddExistingClone()
        {
            var parentProject = new DirectoryInfo(Application.dataPath).Parent;
            var dir = parentProject.Parent.FullName;
            var path = EditorUtility.OpenFolderPanel("Add Existing Clone", dir, "");
            if (null != path)
            {
                AddCloneData(path);
            }
        }
        
        private static string ProjectPath => new DirectoryInfo(Application.dataPath).Parent.FullName;

        private static Clone AddCloneData(string path)
        {
            if (null != path)
            {
                if (clones.Count == 0)
                {
                    clones.master = new Clone()
                    {
                        path = ProjectPath,
                        isMaster = true,
                        symlinkProjectSettings = true
                    };
                }

                var clone = new Clone()
                {
                    path = path,
                    isMaster = false,
                    symlinkProjectSettings = true
                };
                clones.Add(clone);
                
                clone.symlinkProjectSettings = EditorUtility.DisplayDialog("Share Project Settings?",
                    "Do you want this clone to have its own project settings via symlink?", "Yes", "No");
                
                GetWindow(typeof(CloneManagerEditorWindow)).Repaint();

                string cloneData = JsonUtility.ToJson(clones);
                File.WriteAllText(Path.Combine(Application.dataPath, "clone.manager"), cloneData);
                return clone;
            }

            return null;
        }

        private GUIContent DirectoryLabel(string path)
        {
            return new GUIContent(new DirectoryInfo(path).Name, path);
        }

        private static string PathProjectSettings(Clone clone) => Path.Combine(clone.path, "ProjectSettings");
        private static string PathAssets(Clone clone) => Path.Combine(clone.path, "Assets");
        private static string PathLibrary(Clone clone) => Path.Combine(clone.path, "Library");
        private static string PathPackages(Clone clone) => Path.Combine(clone.path, "Packages");

        private void DrawClone(Clone clone)
        {
            var actions = new List<ContextMenuItem>();
            if (clone.path != ProjectPath)
            {
                actions.Add(new ContextMenuItem("Open in Unity", () => OpenInUnity(clone)));
            }

            actions.Add(new ContextMenuItem("Build", () => Build(clone)));

            
            if (!clone.symlinkProjectSettings)
            {
                actions.Add(new ContextMenuItem("-"));
                actions.Add(new ContextMenuItem("Synchronize Project Settings", () => SyncProjectSettings(clone)));
            }

            if (!clone.isMaster)
            {
                actions.Add(new ContextMenuItem("-"));
                if (File.Exists(clone.path))
                {
                    actions.Add(new ContextMenuItem("Delete Clone", () => Delete(clone)));
                }
                else
                {
                    actions.Add(new ContextMenuItem("Create Clone", () => GenerateClone(clone)));
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(new DirectoryInfo(clone.path).Name, clone.path), EditorStyles.label);
            ContextMenuUI.DrawContextMenu(actions.ToArray(), () => OpenInUnity(clone));
            if (UIUtil.BorderlessUnityButton("d_FolderOpened Icon", tooltip: "Open in Explorer"))
            {
                EditorUtility.RevealInFinder(clone.path);
            }
            GUILayout.EndHorizontal();
        }

        private delegate string PathGen(Clone clone);
        private static void ClonePath(PathGen pathGen, Clone clone, bool symlink = true)
        {
            if (symlink)
            {
                Junction.Create(pathGen(clone), pathGen(clones.master), false);
            }
            else
            {
                FileIO.ProcessXcopy(pathGen(clones.master), pathGen(clone));
            }
        }

        private static void GenerateClone(Clone clone)
        {
            if (!File.Exists(clone.path))
            {
                Directory.CreateDirectory(clone.path);
                
                // Create Main Project Files
                ClonePath(PathProjectSettings, clone, clone.symlinkProjectSettings);
                ClonePath(PathAssets, clone);
                ClonePath(PathPackages, clone);
                
                // Clone library to avoid long project import
                ClonePath(PathLibrary, clone, false);
            }
            else
            {
                Debug.LogError("Clone already exists at " + clone.path);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
            EditorGUILayout.TextField(projectPath, GUILayout.ExpandWidth(true));
            if (UIUtil.BorderlessUnityButton("d_FolderOpened Icon", tooltip: "Open in Explorer"))
            {
                EditorUtility.RevealInFinder(projectPath);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Primary", EditorStyles.boldLabel);
            if (clones.master != null)
            {
                DrawClone(clones.master);
            }
            else
            {
                EditorGUILayout.LabelField(DirectoryLabel(new DirectoryInfo(Application.dataPath).Parent.FullName));
            }

            scrollView = GUILayout.BeginScrollView(scrollView);
            EditorGUILayout.LabelField("Clones", EditorStyles.boldLabel);

            foreach (var clone in clones.clones)
            {
                if (!clone.isMaster)
                {
                    DrawClone(clone);
                }
            }
            GUILayout.EndScrollView();
        }

        private void OnProjectChange()
        {
            Load();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Clone Manager");
            Load();
        }

        private void OnFocus()
        {
            Load();
        }

        private void Load()
        {
            var path = Path.Combine(Application.dataPath, "clone.manager");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                clones = JsonUtility.FromJson<CloneData>(json);
            }
        }

        private void SyncProjectSettings(Clone clone)
        {
            if (!clone.isMaster && !clone.symlinkProjectSettings && EditorUtility.DisplayDialog("Sync Settings?", "This will replace this clone's current project settings with those from master. Are you sure?", "OK", "Cancel"))
            {
                var source = Path.Combine(clones.master.path, "ProjectSettings");
                var target = Path.Combine(clone.path, "ProjectSettings");
                Directory.Delete(target);
                FileIO.ProcessXcopy(source, target);
            }
        }

        private void Build(Clone clone)
        {
            if (clone.path != ProjectPath)
            {
                Build(clone.path);
            }
            else
            {
                Build();
            }
        }

        private static void Build(string path)
        {
            var arg = "-projectPath \"" + path + "\" -executeMethod CloneManagerEditorWindow.Build -quit -logFile /dev/stdout";
            Process.Start(EditorApplication.applicationPath, arg);            
        }

        public static void Build()
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(new BuildPlayerOptions());
        }

        private void Delete(Clone clone)
        {
            if (!clone.isMaster && EditorUtility.DisplayDialog("Delete Clone?", "This will completely remove this clone's project directory. Are you sure?", "Delete It!", "Cancel"))
            {
                Directory.Delete(clone.path);
            }
        }

        private void OpenInUnity(Clone clone)
        {
            if (clone.path != ProjectPath)
            {
                var arg = "-projectPath \"" + clone.path + "\"";
                Process.Start(EditorApplication.applicationPath, arg);
            }
        }
    }
}
