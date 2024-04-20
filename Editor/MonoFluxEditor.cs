/*
Copyright (c) 2023 Xavier Arpa López Thomas Peter ('xavierarpa')

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using UniFlux.Core;

namespace UniFlux.Editor
{
    [CustomEditor(typeof(MonoFlux), true)]
    public partial class MonoFluxEditor : UnityEditor.Editor
    {
        private MethodInfo[] methods_subscribeAttrb;
        private Dictionary<MethodInfo, object[]> dic_method_parameters;
        private static bool showBox = true;
        private void OnEnable()
        {
            Type type = target.GetType();
            var methods = type.GetMethods((BindingFlags)(-1));
            #pragma warning disable CS0618
            methods_subscribeAttrb = methods.Where(m => m.GetCustomAttributes(typeof(FluxAttribute), true).Length > 0).ToArray();
            #pragma warning restore CS0618
            dic_method_parameters = methods_subscribeAttrb.Select(m => new { Method = m, Parameters = new object[m.GetParameters().Length] }).ToDictionary(mp => mp.Method, mp => mp.Parameters);
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(methods_subscribeAttrb.Length.Equals(0))
            {
                showBox = false;
            }
            else
            {
                if(GUILayout.Button( showBox ? "Close" : $"Open ({methods_subscribeAttrb.Length})", GUI.skin.box))
                {
                    showBox = !showBox;
                }
            }
            if(showBox)
            {
                _Draw();
            }
        }
        private void _Draw()
        {
            GUILayout.Space(20);
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true,
                fontSize = 12
            };
            GUIStyle style_title = new GUIStyle(EditorStyles.boldLabel)
            {
                richText = true
            };
            foreach (var item in methods_subscribeAttrb)
            {
                #pragma warning disable CS0618
                var atribute = item.GetCustomAttribute<FluxAttribute>();
                #pragma warning restore CS0618
                var parameters = item.GetParameters();
                var isParameters = parameters.Length > 0;
                var isErr_return = item.ReturnType != typeof(void);
                var isErr_static = item.IsStatic;
                var isError = isParameters || isErr_return || isErr_static;
                string key_color =  isError ? "yellow" : "white";
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label( $"<color={key_color}>{atribute.key}</color>", style_title);
                GUILayout.Label(item.ToString(), EditorStyles.whiteMiniLabel);
                GenerateButton(buttonStyle,item);
                EditorGUILayout.BeginVertical();
                GenerateParameters(item, parameters);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }
        // private void GenerateParameters(MethodInfo item, ParameterInfo[] parameters)
        // {
        //     // var args = dic_method_parameters[item];
        //     // for (int i = 0; i < parameters.Length; i++)
        //     // {
        //     //     EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        //     //     GUILayout.Label(parameters[i].ToString());
        //     //     EditorGUILayout.EndHorizontal();
        //     // }
        //     // dic_method_parameters[item] = args;
        // }
        private void GenerateParameters(MethodInfo item, ParameterInfo[] parameters)
        {
            // var args = dic_method_parameters[item];

            // for (int i = 0; i < parameters.Length; i++)
            // {
            //     var parameter = parameters[i];

            //     if (parameter.ParameterType.IsValueType)
            //     {
            //         // El parámetro es primitivo
            //         object defaultValue = Activator.CreateInstance(parameter.ParameterType);
            //         object value = args[i] ?? defaultValue;

            //         EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            //         GUILayout.Label(parameter.Name, GUILayout.Width(150));

            //         if (parameter.ParameterType == typeof(int))
            //         {
            //             value = EditorGUILayout.IntField((int)value);
            //         }
            //         else if (parameter.ParameterType == typeof(float))
            //         {
            //             value = EditorGUILayout.FloatField((float)value);
            //         }
            //         else if (parameter.ParameterType == typeof(bool))
            //         {
            //             value = EditorGUILayout.Toggle((bool)value);
            //         }
            //         else if (parameter.ParameterType == typeof(string))
            //         {
            //             value = EditorGUILayout.TextField((string)value);
            //         }
            //         else if (parameter.ParameterType == typeof(Vector2))
            //         {
            //             value = EditorGUILayout.Vector2Field("", (Vector2)value);
            //         }
            //         else if (parameter.ParameterType == typeof(Vector3))
            //         {
            //             value = EditorGUILayout.Vector3Field("", (Vector3)value);
            //         }
            //         else if (parameter.ParameterType == typeof(Vector4))
            //         {
            //             value = EditorGUILayout.Vector4Field("", (Vector4)value);
            //         }

            //         args[i] = Convert.ChangeType(value, parameter.ParameterType);
            //         EditorGUILayout.EndHorizontal();
            //     }
            //     else
            //     {
            //         // El parámetro no es primitivo
            //         GUILayout.Label(parameter.Name + " (no primitivo)", EditorStyles.whiteMiniLabel);
            //     }
            // }

            // dic_method_parameters[item] = args;
        }

        private void GenerateButton(GUIStyle buttonStyle, MethodInfo item)
        {
            GUI.enabled = Application.isPlaying;
            if (dic_method_parameters[item].Length.Equals(0) && GUILayout.Button($"Invoke!", buttonStyle)) 
            {
                if(item.ReturnType != typeof(void))
                {
                    var resp = item.Invoke(target, dic_method_parameters[item]);
                    Debug.Log(resp);
                }
                else
                {
                    item.Invoke(target, dic_method_parameters[item]);
                }
            }
            GUI.enabled = true;
        }
    }
}