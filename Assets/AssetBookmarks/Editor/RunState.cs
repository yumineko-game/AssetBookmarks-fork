using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBookmarks.Editor
{
    public partial class AssetBookmarksWindow
    {
        private class RunState : IWindowState
        {
            private readonly Model _model;

            public RunState(Model model)
            {
                _model = model;
            }

            public void Dispose()
            {
            }

            public void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
            {
                var item = _model.Items[index];

                if (item is ProjectItem projectItem)
                {
                    var globalObjectId = projectItem.GlobalObjectID;
                    var path = AssetDatabase.GUIDToAssetPath(globalObjectId.assetGUID.ToString());
                    var name = Path.GetFileNameWithoutExtension(path);
                    var content = new GUIContent($" {projectItem.OpenType} {name}", AssetDatabase.GetCachedIcon(path));
                    var style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
                    if (GUI.Button(rect, content, style))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                        switch (projectItem.OpenType)
                        {
                            case OpenType.Open:
                                AssetDatabase.OpenAsset(asset);
                                break;

                            case OpenType.Focus:
                                EditorUtility.FocusProjectWindow();
                                EditorGUIUtility.PingObject(asset);
                                Selection.activeObject = asset;
                                EditorUtility.FocusProjectWindow();
                                break;

                            case OpenType.Finder:
                                EditorUtility.RevealInFinder(path);
                                break;

                            case OpenType.App:
                                EditorUtility.OpenWithDefaultApp(path);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (item is OutsideItem outsideItem)
                {
                    var path = outsideItem.Path;
                    var name = path;
                    var content = new GUIContent($" {name}");
                    if (GUI.Button(rect, content))
                    {
                        EditorUtility.OpenWithDefaultApp(path);
                    }
                }
            }

            public void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
            {
            }

            public IWindowState OnGui()
            {
                _model.ReorderableList.DoLayoutList();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit")) return new EditState(_model);
                return null;
            }
        }
    }
}