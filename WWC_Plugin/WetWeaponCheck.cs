using System;
using KSP.UI.Screens;
using BDArmory;
using System.Collections.Generic;
using UnityEngine;


namespace WetWeaponCheck
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WetWeaponCheck : MonoBehaviour
    {
        private const float WindowWidth = 140;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static WetWeaponCheck Fetch;
        public static bool GuiEnabled;
        public static bool HasAddedButton;
        private readonly float _incrButtonWidth = 26;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;
        private float _contentWidth;
        private bool _gameUiToggle;
        private float _windowHeight = 100;
        private Rect _windowRect;

        public string _guiDepth = String.Empty;
        private bool WWC;
        private bool CheckWWC = true;
        private bool HBC = false;

        private void Awake()
        {
            if (Fetch)
                Destroy(Fetch);

            Fetch = this;
        }

        private void Start()
        {
            _windowRect = new Rect(Screen.width - WindowWidth - 40, 100, WindowWidth, _windowHeight);
            AddToolbarButton();
            GameEvents.onHideUI.Add(GameUiDisable);
            GameEvents.onShowUI.Add(GameUiEnable);
            _gameUiToggle = true;
            _guiDepth = "5";
        }

        private void OnGUI()
        {
            if (GuiEnabled && _gameUiToggle)
                _windowRect = GUI.Window(320, _windowRect, GuiWindow, "");
        }

        private void checkDepth()
        {
            var guiDepth = float.Parse(_guiDepth);

            List<ModuleWWC> wwcParts = new List<ModuleWWC>(200);
            foreach (Part p in FlightGlobals.ActiveVessel.Parts)
            {
                wwcParts.AddRange(p.FindModulesImplementing<ModuleWWC>());
            }
            foreach (ModuleWWC wwcPart in wwcParts)
            {
                if (guiDepth >= 0)
                {
                    wwcPart.CutoffDepth = guiDepth * (-1);

                }
                else
                {
                    wwcPart.CutoffDepth = guiDepth;
                }
            }
        }

        #region GUI
        /// <summary>
        /// GUI
        /// </summary>
        private void GuiWindow(int windowId)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;
            _contentWidth = WindowWidth - 2 * LeftIndent;

            DrawTitle();
            DrawText(line);
            line++;
            DrawDepth(line);
            line++;
            SendDepth(line);
//            line++;
//            setWWC(line);

            _windowHeight = ContentTop + line * entryHeight + entryHeight + entryHeight;
            _windowRect.height = _windowHeight;
        }

        private void AddToolbarButton()
        {
            string textureDir = "WWC/Plugin/";

            if (!HasAddedButton)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture(textureDir + "WWC_icon", false); //texture to use for the button
                ApplicationLauncher.Instance.AddModApplication(EnableGui, DisableGui, Dummy, Dummy, Dummy, Dummy,
                    ApplicationLauncher.AppScenes.FLIGHT, buttonTexture);
                HasAddedButton = true;
            }
        }

        private void EnableGui()
        {
            GuiEnabled = true;
            Debug.Log("[Wet Weapon Check]: Showing WWC GUI");
        }

        private void DisableGui()
        {
            GuiEnabled = false;
            Debug.Log("[Wet Weapon Check]: Hiding WWC GUI");
        }

        private void GameUiEnable()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisable()
        {
            _gameUiToggle = false;
        }

        private void DrawTitle()
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Wet Weapon Check", titleStyle);
        }

        private void SendDepth(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);
            if (GUI.Button(saveRect, "Update Depth"))
            {
                checkDepth();
            }
        }


        private void DrawDepth(float line)
        {
            var leftLabel = new GUIStyle();
            leftLabel.alignment = TextAnchor.UpperLeft;
            leftLabel.normal.textColor = Color.white;

            GUI.Label(new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, 60, entryHeight), "Depth",
                leftLabel);
            float textFieldWidth = 55;
            var fwdFieldRect = new Rect((LeftIndent * 1.5f) + contentWidth - textFieldWidth,
                ContentTop + line * entryHeight, textFieldWidth, entryHeight);
            _guiDepth = GUI.TextField(fwdFieldRect, _guiDepth);
        }

        private void DrawText(float line)
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(0, ContentTop + line * entryHeight, WindowWidth, 20),
                "Set Cut-Off Depth", titleStyle);
        }

        private void setWWC(float line)
        {
            var saveRect = new Rect(LeftIndent * 1.5f, ContentTop + line * entryHeight, contentWidth * 0.9f, entryHeight);

            if (WWC)
            {
                if (GUI.Button(saveRect, "WWC   [On]"))
                {
                    WWC = false;
                }
            }
            else
            {
                if (GUI.Button(saveRect, "WWC  [Off]"))
                {
                    WWC = true;
                }
            }
        }

        #endregion

        private void Dummy()
        {
        }
    }
}