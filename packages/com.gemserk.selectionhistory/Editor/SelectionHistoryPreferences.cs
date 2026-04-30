using UnityEngine;
using UnityEditor;

namespace Gemserk {
    
    [InitializeOnLoad]
    public static class SelectionHistoryPreferences {

        static bool prefsLoaded = false;

       // static int historySize;

        static bool autoremoveDestroyed;
        static bool autoremoveUnloaded;

        static bool autoRemoveDuplicated;

        private static bool drawFavorites = true;

        private static bool orderLastSelectedFirst = true;

        private static bool backgroundRecord;

        public static bool nativeKeyHandleDisabled;

        private static int pinMouseButton;
        private static bool pinModifierAlt;
        private static bool pinModifierShift;
        private static bool pinModifierControl;

        private static readonly string[] MouseButtonOptions = { "Left Click", "Right Click" };

        static SelectionHistoryPreferences()
        {
            // historySize = EditorPrefs.GetInt(SelectionHistoryWindowUtils.HistorySizePrefKey, defaultHistorySize);
            autoremoveDestroyed = EditorPrefs.GetBool(SelectionHistoryWindowUtils.HistoryAutomaticRemoveDestroyedPrefKey, true);
            autoremoveUnloaded = EditorPrefs.GetBool(SelectionHistoryWindowUtils.HistoryAutomaticRemoveUnloadedPrefKey, true);
            autoRemoveDuplicated = EditorPrefs.GetBool(SelectionHistoryWindowUtils.HistoryAllowDuplicatedEntriesPrefKey, false);
            drawFavorites = EditorPrefs.GetBool(SelectionHistoryWindowUtils.HistoryShowPinButtonPrefKey, true);
            orderLastSelectedFirst = EditorPrefs.GetBool(SelectionHistoryWindowUtils.OrderLastSelectedFirstKey, false);
            backgroundRecord = EditorPrefs.GetBool(SelectionHistoryWindowUtils.BackgroundRecordKey, false);
            nativeKeyHandleDisabled = EditorPrefs.GetBool(SelectionHistoryWindowUtils.NativeKeyHandleDisabledKey, false);

            pinMouseButton = SelectionHistoryWindowUtils.PinMouseButton;
            var pinMods = SelectionHistoryWindowUtils.PinModifiers;
            pinModifierAlt     = (pinMods & EventModifiers.Alt)     != 0;
            pinModifierShift   = (pinMods & EventModifiers.Shift)   != 0;
            pinModifierControl = (pinMods & EventModifiers.Control) != 0;

            prefsLoaded = true;
        }

        [SettingsProvider]
        public static SettingsProvider CreateSelectionHistorySettingsProvider() {
            var provider = new SettingsProvider("Selection History", SettingsScope.User) {
                label = "Selection History",
                guiHandler = (searchContext) =>
                {
                    var selectionHistory = SelectionHistoryAsset.instance.selectionHistory;
                    
                    if (selectionHistory != null)
                    {
                        selectionHistory.historySize =
                            EditorGUILayout.IntField("History Size", selectionHistory.historySize);
                    }
                    
                    autoremoveDestroyed = EditorGUILayout.Toggle("Auto Remove Destroyed", autoremoveDestroyed);
                    autoremoveUnloaded = EditorGUILayout.Toggle("Auto Remove Unloaded", autoremoveUnloaded);
                    autoRemoveDuplicated = EditorGUILayout.Toggle("Allow duplicated entries", autoRemoveDuplicated);
                    drawFavorites = EditorGUILayout.Toggle("Show Pin to favorites button", drawFavorites);
                    orderLastSelectedFirst = EditorGUILayout.Toggle("Order last selected first", orderLastSelectedFirst);
                    backgroundRecord = EditorGUILayout.Toggle("Record selection while window closed", backgroundRecord);
                    nativeKeyHandleDisabled = EditorGUILayout.Toggle("Disable native mouse handle (button 4 & 5)", nativeKeyHandleDisabled);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Pin Gesture", EditorStyles.boldLabel);
                    pinMouseButton = EditorGUILayout.Popup("Mouse Button", pinMouseButton, MouseButtonOptions);
                    EditorGUILayout.LabelField("Modifiers");
                    EditorGUI.indentLevel++;
                    pinModifierAlt     = EditorGUILayout.Toggle("Alt",        pinModifierAlt);
                    pinModifierShift   = EditorGUILayout.Toggle("Shift",      pinModifierShift);
                    pinModifierControl = EditorGUILayout.Toggle("Ctrl / Cmd", pinModifierControl);
                    EditorGUI.indentLevel--;

                    if (GUI.changed) {
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.HistoryAutomaticRemoveDestroyedPrefKey, autoremoveDestroyed);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.HistoryAutomaticRemoveUnloadedPrefKey, autoremoveUnloaded);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.HistoryAllowDuplicatedEntriesPrefKey, autoRemoveDuplicated);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.HistoryShowPinButtonPrefKey, drawFavorites);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.OrderLastSelectedFirstKey, orderLastSelectedFirst);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.BackgroundRecordKey, backgroundRecord);
                        EditorPrefs.SetBool(SelectionHistoryWindowUtils.NativeKeyHandleDisabledKey, nativeKeyHandleDisabled);

                        EditorPrefs.SetInt(SelectionHistoryWindowUtils.PinMouseButtonPrefKey, pinMouseButton);
                        var pinMods = EventModifiers.None;
                        if (pinModifierAlt)     pinMods |= EventModifiers.Alt;
                        if (pinModifierShift)   pinMods |= EventModifiers.Shift;
                        if (pinModifierControl) pinMods |= EventModifiers.Control;
                        EditorPrefs.SetInt(SelectionHistoryWindowUtils.PinModifiersPrefKey, (int)pinMods);

                        // var window = EditorWindow.GetWindow<SelectionHistoryWindow>();
                        // if (window != null)
                        // {
                        //     window.ReloadRootAndRemoveUnloadedAndDuplicated();
                        // }

                        SelectionHistoryAsset.instance.ForceSave();
                    }
                },

            };
            return provider;
        }
    }
}
