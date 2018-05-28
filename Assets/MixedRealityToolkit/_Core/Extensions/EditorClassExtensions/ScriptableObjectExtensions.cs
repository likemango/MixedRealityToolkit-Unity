﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Extensions.EditorClassExtensions
{
    /// <summary>
    /// Extensions for <see cref="ScriptableObject"/>s
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Creates, saves, and then opens a new asset for the target <see cref="ScriptableObject"/>.
        /// </summary>
        /// <param name="scriptableObject"><see cref="ScriptableObject"/> you want to create an asset file for.</param>
        /// <param name="fileName">Optional filename for the new asset.</param>
        public static void CreateAsset(this ScriptableObject scriptableObject, string fileName = null)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{scriptableObject.GetType().Name}" : fileName;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == string.Empty)
            {
                path = "Assets/Profiles";
            }
            else if (Path.GetExtension(path) != string.Empty)
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                Debug.Assert(assetPath != null);
                path = path.Replace(Path.GetFileName(assetPath), string.Empty);
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scriptableObject;
            EditorGUIUtility.PingObject(scriptableObject);
        }

        /// <summary>
        /// Gets all the scriptable object instances in the project.
        /// </summary>
        /// <typeparam name="T">The Type of <see cref="ScriptableObject"/> you're wanting to find instances of.</typeparam>
        /// <returns>An Array of instances for the type.</returns>
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            // FindAssets uses tags check documentation for more info
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            T[] instances = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                instances[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return instances;
        }
    }
}