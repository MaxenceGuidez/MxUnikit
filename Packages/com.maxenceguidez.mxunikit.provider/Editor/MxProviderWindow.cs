using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MxUnikit.Provider.Editor
{
    public class MxProviderWindow : EditorWindow
    {
        private const long RefreshIntervalMs = 500;

        private Label _countLabel;
        private ToolbarButton _clearButton;
        private VisualElement _table;
        private ScrollView _rows;
        private VisualElement _emptyState;
        private Label _emptyHint;

        private readonly List<KeyValuePair<Type, object>> _snapshot = new List<KeyValuePair<Type, object>>();

        private static Color StripeColor => EditorGUIUtility.isProSkin
            ? new Color(1f, 1f, 1f, 0.03f)
            : new Color(0f, 0f, 0f, 0.04f);

        private static Color HeaderBackgroundColor => EditorGUIUtility.isProSkin
            ? new Color(0f, 0f, 0f, 0.2f)
            : new Color(0f, 0f, 0f, 0.07f);

        [MenuItem("Window/MxUnikit/Provider")]
        public static void ShowWindow()
        {
            MxProviderWindow window = GetWindow<MxProviderWindow>("MxProvider");
            window.minSize = new Vector2(340f, 180f);
        }

        private void CreateGUI()
        {
            BuildToolbar();
            BuildTable();
            BuildEmptyState();

            rootVisualElement.schedule.Execute(Refresh).Every(RefreshIntervalMs);
            Refresh();
        }

        #region Layout

        private void BuildToolbar()
        {
            Toolbar toolbar = new Toolbar();

            _countLabel = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleLeft,
                    paddingLeft = 6f,
                    opacity = 0.7f
                }
            };
            toolbar.Add(_countLabel);

            VisualElement spacer = new VisualElement { style = { flexGrow = 1f } };
            toolbar.Add(spacer);

            _clearButton = new ToolbarButton(OnClearClicked) { text = "Clear All" };
            toolbar.Add(_clearButton);

            rootVisualElement.Add(toolbar);
        }

        private void BuildTable()
        {
            _table = new VisualElement { style = { flexGrow = 1f } };

            VisualElement header = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    backgroundColor = HeaderBackgroundColor,
                    paddingTop = 4f,
                    paddingBottom = 4f,
                    paddingLeft = 8f,
                    paddingRight = 8f
                }
            };

            header.Add(MakeHeaderCell("Registered As"));
            header.Add(MakeHeaderCell("Instance"));
            header.Add(MakeHeaderCell("Reference"));

            _table.Add(header);

            _rows = new ScrollView { style = { flexGrow = 1f } };
            _table.Add(_rows);

            rootVisualElement.Add(_table);
        }

        private void BuildEmptyState()
        {
            _emptyState = new VisualElement
            {
                style =
                {
                    flexGrow = 1f,
                    justifyContent = Justify.Center,
                    alignItems = Align.Center
                }
            };

            Label labelTitle = new Label("No instances registered")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    opacity = 0.8f
                }
            };
            _emptyState.Add(labelTitle);

            _emptyHint = new Label { style = { opacity = 0.5f, marginTop = 4f } };
            _emptyState.Add(_emptyHint);

            rootVisualElement.Add(_emptyState);
        }

        private static Label MakeHeaderCell(string text)
        {
            Label label = new Label(text)
            {
                style =
                {
                    flexGrow = 1f,
                    flexBasis = 0f,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 10f,
                    opacity = 0.6f
                }
            };

            return label;
        }

        #endregion

        #region Refresh

        private void Refresh()
        {
            List<KeyValuePair<Type, object>> current = new List<KeyValuePair<Type, object>>(MxProvider.GetAll());
            current.Sort((a, b) => string.CompareOrdinal(a.Key.Name, b.Key.Name));

            bool isEmpty = current.Count == 0;

            _countLabel.text = isEmpty ? "No instances" : $"{current.Count} registered";
            _clearButton.SetEnabled(!isEmpty);
            _table.style.display = isEmpty ? DisplayStyle.None : DisplayStyle.Flex;
            _emptyState.style.display = isEmpty ? DisplayStyle.Flex : DisplayStyle.None;
            _emptyHint.text = Application.isPlaying
                ? "Call MxProvider.Register<T>() to add one."
                : "Enter Play Mode to inspect registered instances.";

            if (SameAsSnapshot(current)) return;

            _snapshot.Clear();
            _snapshot.AddRange(current);
            RebuildRows();
        }

        private bool SameAsSnapshot(List<KeyValuePair<Type, object>> current)
        {
            if (current.Count != _snapshot.Count) return false;

            for (int i = 0; i < current.Count; i++)
            {
                if (current[i].Key != _snapshot[i].Key) return false;
                if (!ReferenceEquals(current[i].Value, _snapshot[i].Value)) return false;
            }

            return true;
        }

        private void RebuildRows()
        {
            _rows.Clear();

            for (int i = 0; i < _snapshot.Count; i++)
            {
                _rows.Add(BuildRow(_snapshot[i], i));
            }
        }

        private static VisualElement BuildRow(KeyValuePair<Type, object> entry, int index)
        {
            VisualElement row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    minHeight = 22f,
                    paddingLeft = 8f,
                    paddingRight = 8f
                }
            };

            if (index % 2 == 1)
            {
                row.style.backgroundColor = StripeColor;
            }

            Type runtimeType = entry.Value.GetType();

            row.Add(MakeCell(entry.Key.Name, entry.Key.FullName, FontStyle.Bold));
            row.Add(MakeCell(runtimeType.Name, runtimeType.FullName, FontStyle.Normal));
            row.Add(MakeReferenceCell(entry.Value));

            return row;
        }

        private static Label MakeCell(string text, string tooltip, FontStyle fontStyle)
        {
            Label label = new Label(text)
            {
                tooltip = tooltip,
                style =
                {
                    flexGrow = 1f,
                    flexBasis = 0f,
                    unityFontStyleAndWeight = fontStyle,
                    overflow = Overflow.Hidden,
                    textOverflow = TextOverflow.Ellipsis,
                    whiteSpace = WhiteSpace.NoWrap
                }
            };

            return label;
        }

        private static VisualElement MakeReferenceCell(object instance)
        {
            VisualElement cell = new VisualElement { style = { flexGrow = 1f, flexBasis = 0f } };

            if (instance is UnityEngine.Object unityObject)
            {
                ObjectField field = new ObjectField { value = unityObject };
                field.RegisterValueChangedCallback(_ => field.SetValueWithoutNotify(unityObject));
                field.style.marginLeft = 0f;
                field.style.marginRight = 0f;
                cell.Add(field);
            }
            else
            {
                Label none = new Label("—") { style = { opacity = 0.4f } };
                cell.Add(none);
            }

            return cell;
        }

        #endregion

        private void OnClearClicked()
        {
            bool confirmed = EditorUtility.DisplayDialog("Clear Instances",
                "Are you sure you want to clear all registered instances?", "Yes", "Cancel");

            if (!confirmed) return;

            MxProvider.Clear();
            Refresh();
        }
    }
}
