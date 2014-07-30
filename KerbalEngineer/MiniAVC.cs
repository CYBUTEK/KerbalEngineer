// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

using LitJson;

using UnityEngine;

#endregion

namespace KerbalEngineer
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class MiniAVC : MonoBehaviour
    {
        #region Fields

        private AddonManager addonManager;
        private bool hasBeenCentred;
        private int windowId;
        private Rect windowPosition = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                Settings.Load();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->Awake");
            }
        }

        private void Start()
        {
            try
            {
                if (AssemblyLoader.loadedAssemblies.Any(a => a.name == "KSP-AVC"))
                {
                    Logger.Log("MiniAVC has been overridden by KSP-AVC!");
                    Destroy(this);
                    return;
                }

                if (!Settings.Instance.FirstRun && !Settings.Instance.AllowCheck)
                {
                    Logger.Log("MiniAVC has been disabled!");
                    Destroy(this);
                    return;
                }
                
                if (Settings.Instance.AllowCheck)
                {
                    this.addonManager = new AddonManager();
                }
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->Start");
            }
        }

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle titleStyle;

        private void InitialiseStyles()
        {
            try
            {
                this.titleStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    stretchWidth = true
                };

                this.labelStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    fontSize = 13,
                    alignment = TextAnchor.MiddleCenter,
                    stretchWidth = true
                };

                this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
                {
                    normal =
                    {
                        textColor = Color.white
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->InitialiseStyles");
            }
        }

        #endregion

        #endregion

        private void Update()
        {
            try
            {
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                {
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->Update");
            }
        }

        private void OnGUI()
        {
            try
            {
                if (Settings.Instance.FirstRun)
                {
                    this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.FirstRunWindow, Assembly.GetExecutingAssembly().GetName().Name + " - MiniAVC", HighLogic.Skin.window);
                    if (!this.hasBeenCentred && (this.windowPosition.width > 0 && this.windowPosition.height > 0))
                    {
                        this.windowPosition.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                        this.hasBeenCentred = true;
                    }
                    return;
                }

                if (this.addonManager == null || this.addonManager.IsLocked || !this.addonManager.HasIssues)
                {
                    return;
                }

                this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.VersionCheckWindow, this.addonManager.Addon.Name + " - MiniAVC", HighLogic.Skin.window);
                if (!this.hasBeenCentred && (this.windowPosition.width > 0 && this.windowPosition.height > 0))
                {
                    this.windowPosition.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    this.hasBeenCentred = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->OnGUI");
            }
        }

        private void VersionCheckWindow(int windowId)
        {
            try
            {
                if (this.addonManager.HasUpdateIssues)
                {
                    GUILayout.BeginVertical(HighLogic.Skin.box);
                    GUILayout.Label("AN UPDATE IS AVAILABLE", this.titleStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Installed: " + this.addonManager.Addon.AddonVersion, this.labelStyle, GUILayout.Width(150.0f));
                    GUILayout.Label("Available: " + this.addonManager.Addon.RemoteAddonData.AddonVersion, this.labelStyle, GUILayout.Width(150.0f));
                    GUILayout.EndHorizontal();
                    if (this.addonManager.Addon.RemoteAddonData.Download != null && this.addonManager.Addon.RemoteAddonData.Download.Length > 0)
                    {
                        if (GUILayout.Button("DOWNLOAD", this.buttonStyle))
                        {
                            Application.OpenURL(this.addonManager.Addon.RemoteAddonData.Download);
                        }
                    }
                    GUILayout.EndVertical();
                }

                if (this.addonManager.HasCompatibilityIssues)
                {
                    GUILayout.BeginVertical(HighLogic.Skin.box);
                    if (!this.addonManager.Addon.GameCompatibleMinimum)
                    {
                        if (this.addonManager.Addon.GameVersionMaximum == AddonData.DefaultMaximumVersion)
                        {
                            GUILayout.Label("Unsupported KSP version... Please use " + this.addonManager.Addon.GameVersionMinimum + " and above.", this.titleStyle, GUILayout.Width(300.0f));
                        }
                        else
                        {
                            GUILayout.Label("Unsupported KSP version... Please use " + this.addonManager.Addon.GameVersionMinimum + " - " + this.addonManager.Addon.GameVersionMinimum, this.titleStyle, GUILayout.Width(300.0f));
                        }
                    }
                    else if (!this.addonManager.Addon.GameCompatibleMaximum)
                    {
                        if (this.addonManager.Addon.GameVersionMinimum == AddonData.DefaultMinimumVersion)
                        {
                            GUILayout.Label("Unsupported KSP version... Please use " + this.addonManager.Addon.GameVersionMaximum + " and below.", this.titleStyle, GUILayout.Width(300.0f));
                        }
                        else
                        {
                            GUILayout.Label("Unsupported KSP version... Please use " + this.addonManager.Addon.GameVersionMinimum + " - " + this.addonManager.Addon.GameVersionMinimum, this.titleStyle, GUILayout.Width(300.0f));
                        }
                    }
                    else if (!this.addonManager.Addon.GameCompatibleVersion)
                    {
                        GUILayout.Label("Unsupported KSP version... Please use " + this.addonManager.Addon.GameVersion, this.titleStyle, GUILayout.Width(300.0f));
                    }
                    GUILayout.EndVertical();
                }

                if (GUILayout.Button("CLOSE", this.buttonStyle))
                {
                    Destroy(this);
                }

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->VersionCheckWindow");
            }
        }

        private void FirstRunWindow(int windowId)
        {
            try
            {
                GUILayout.BeginVertical(HighLogic.Skin.box);
                GUILayout.Label("Let this add-on check for updates?", this.titleStyle, GUILayout.Width(300.0f));
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("YES", this.buttonStyle, GUILayout.Width(200.0f)))
                {
                    this.addonManager = new AddonManager();
                    Settings.Instance.FirstRun = false;
                    Settings.Instance.AllowCheck = true;
                    Settings.Save();
                    this.hasBeenCentred = false;
                    this.windowPosition = new Rect(Screen.width, Screen.height, 0, 0);
                }
                if (GUILayout.Button("NO", this.buttonStyle))
                {
                    Settings.Instance.FirstRun = false;
                    Settings.Instance.AllowCheck = false;
                    Settings.Save();
                    Logger.Log("MiniAVC has been disabled!");
                    Destroy(this);
                }
                GUILayout.EndHorizontal();

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "MiniAVC->FirstRunWindow");
            }
        }

        #region Nested type: AddonData

        public class AddonData
        {
            #region Fields

            private readonly System.Version currentGameVersion = CurrentGameVersion;
            private readonly string filename = string.Empty;
            private readonly string json = string.Empty;
            private System.Version addonVersion = new System.Version();
            private string download = string.Empty;
            private System.Version gameVersion = CurrentGameVersion;
            private System.Version gameVersionMaximum = DefaultMaximumVersion;
            private System.Version gameVersionMinimum = DefaultMinimumVersion;
            private string name = string.Empty;
            private AddonData remoteAddonData;
            private string url = string.Empty;

            #endregion

            #region Constructors

            public AddonData(string json, string filename)
            {
                this.filename = filename;
                this.json = json;
                this.ParseJson();
                this.remoteAddonData = this;
            }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets the version filename including path.
            /// </summary>
            public string FileName
            {
                get { return this.filename; }
            }

            /// <summary>
            ///     Gets the add-on name.
            /// </summary>
            public string Name
            {
                get { return this.name; }
            }

            /// <summary>
            ///     Gets the url of the remote version file.
            /// </summary>
            public string Url
            {
                get { return this.url; }
            }

            /// <summary>
            ///     Gets the url of the download location.
            /// </summary>
            public string Download
            {
                get { return this.download; }
            }

            /// <summary>
            ///     Gets the addon version.
            /// </summary>
            public System.Version AddonVersion
            {
                get { return this.addonVersion; }
            }

            /// <summary>
            ///     Gets the game version for which the add-on was created to run on.
            /// </summary>
            public System.Version GameVersion
            {
                get { return this.gameVersion; }
            }

            /// <summary>
            ///     Gets the minimum game version for which the add-on was created to run on.
            /// </summary>
            public System.Version GameVersionMinimum
            {
                get { return this.gameVersionMinimum; }
            }

            /// <summary>
            ///     Gets the maximum game version for which the add-on was created to run on.
            /// </summary>
            public System.Version GameVersionMaximum
            {
                get { return this.gameVersionMaximum; }
            }

            /// <summary>
            ///     Gets the remote addon data.
            /// </summary>
            public AddonData RemoteAddonData
            {
                get { return this.remoteAddonData; }
                set { this.remoteAddonData = value; }
            }

            /// <summary>
            ///     Gets the raw json data.
            /// </summary>
            public string Json
            {
                get { return this.json; }
            }

            /// <summary>
            ///     Gets whether there is an update available.
            /// </summary>
            public bool UpdateAvailable
            {
                get { return this.addonVersion.CompareTo(this.remoteAddonData.addonVersion) < 0 && this.remoteAddonData.GameCompatible; }
            }

            /// <summary>
            ///     Gets whether the add-on is compatible with the current game version.
            /// </summary>
            public bool GameCompatible
            {
                get
                {
                    return this.GameVersionMinimum != DefaultMinimumVersion
                        ? this.GameCompatibleMinimum
                        : this.gameVersionMaximum != DefaultMaximumVersion
                            ? this.GameCompatibleMaximum
                            : this.GameCompatibleVersion;
                }
            }

            /// <summary>
            ///     Gets whether the add-on is compatible with only the current game version.
            /// </summary>
            public bool GameCompatibleVersion
            {
                get { return this.gameVersion.CompareTo(this.currentGameVersion) == 0; }
            }

            /// <summary>
            ///     Gets whether the add-on is compatible with a game version of the same or more than the minimum.
            /// </summary>
            public bool GameCompatibleMinimum
            {
                get { return this.gameVersionMinimum.CompareTo(this.currentGameVersion) <= 0; }
            }

            /// <summary>
            ///     Gets whether the add-on is compatible with a game version of the same of less than the maximum.
            /// </summary>
            public bool GameCompatibleMaximum
            {
                get { return this.gameVersionMaximum.CompareTo(this.currentGameVersion) >= 0; }
            }

            public static System.Version DefaultMinimumVersion
            {
                get { return new System.Version(); }
            }

            public static System.Version DefaultMaximumVersion
            {
                get { return new System.Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue); }
            }

            /// <summary>
            ///     Gets the current game version.
            /// </summary>
            public static System.Version CurrentGameVersion
            {
                get
                {
                    return Versioning.Revision == 0
                        ? new System.Version(Versioning.version_major, Versioning.version_minor)
                        : new System.Version(Versioning.version_major, Versioning.version_minor, Versioning.Revision);
                }
            }

            #endregion

            #region Private Methods

            private void ParseJson()
            {
                try
                {
                    var data = JsonMapper.ToObject(this.json);

                    this.SetPrimaryFields(data);
                    this.SetGameVersion(data);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "AddonData->ParseJson");
                }
            }

            private void SetPrimaryFields(JsonData data)
            {
                // NAME
                try
                {
                    this.name = (string)data["NAME"];
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"NAME\" = " + this.name);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"NAME\" is not valid or missing. (required field)");
                }

                // URL
                try
                {
                    this.url = this.CheckUrlCompatibility((string)data["URL"]);
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"URL\" = " + this.url);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"URL\" is not valid or missing. (required field)");
                }

                // DOWNLOAD
                try
                {
                    this.download = (string)data["DOWNLOAD"];
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"DOWNLOAD\" = " + this.download);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"DOWNLOAD\" is not valid or missing. (optional field)");
                }

                // VERSION
                try
                {
                    this.addonVersion = this.ParseVersion(data["VERSION"]);
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"VERSION\" = " + this.addonVersion);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"VERSION\" is not valid or missing. (required field)");
                }
            }

            private void SetGameVersion(JsonData data)
            {
                // KSP_VERSION
                try
                {
                    this.gameVersion = this.ParseVersion(data["KSP_VERSION"]);
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION\" = " + this.gameVersion);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION\" is not valid or missing. (optional field)");
                }

                // KSP_VERSION_MIN
                try
                {
                    this.gameVersionMinimum = this.ParseVersion(data["KSP_VERSION_MIN"]);
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MIN\" = " + this.gameVersionMinimum);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MIN\" is not valid or missing. (optional field)");
                }

                // KSP_VERSION_MAX
                try
                {
                    this.gameVersionMaximum = this.ParseVersion(data["KSP_VERSION_MAX"]);
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MAX\" = " + this.gameVersionMaximum);
                }
                catch
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " \"KSP_VERSION_MAX\" is not valid or missing. (optional field)");
                }
            }

            private System.Version ParseVersion(JsonData data)
            {
                // Data is not an object so it must be a string value.
                if (!data.IsObject)
                {
                    return new System.Version((string)data);
                }

                // Data is an object so it must contain MAJOR, MINOR, PATCH and BUILD values.
                try
                {
                    switch (data.Count)
                    {
                        case 2:
                            return new System.Version((int)data["MAJOR"], (int)data["MINOR"]);

                        case 3:
                            return (int)data["PATCH"] == 0
                                ? new System.Version((int)data["MAJOR"], (int)data["MINOR"])
                                : new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"]);

                        case 4:
                            return (int)data["PATCH"] == 0
                                ? new System.Version((int)data["MAJOR"], (int)data["MINOR"])
                                : (int)data["BUILD"] == 0
                                    ? new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"])
                                    : new System.Version((int)data["MAJOR"], (int)data["MINOR"], (int)data["PATCH"], (int)data["BUILD"]);

                        default:
                            return new System.Version();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(this.filename.Replace(UrlDir.ApplicationRootPath, string.Empty) + " a problem was encountered whilst parsing a version.");
                    throw new Exception(ex.Message, ex);
                }
            }

            private string CheckUrlCompatibility(string url)
            {
                try
                {
                    if (url.Contains("github.com"))
                    {
                        Logger.Log("Replaced github.com with raw.githubusercontent.com in URL.");
                        return url.Replace("github.com", "raw.githubusercontent.com");
                    }

                    return url;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "AddonManager->CheckUrlCompatibility");
                    return url;
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: AddonManager

        public class AddonManager
        {
            #region Fields

            private AddonData addon;
            private bool isLocked;

            #endregion

            #region Constructors

            public AddonManager()
            {
                // Populate addon asynchronously so that the UI thread is not blocked.
                new Thread(() =>
                {
                    try
                    {
                        this.isLocked = true;
                        Logger.Log("Starting MiniAVC.");
                        var file = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "version");
                        this.addon = new AddonData(File.ReadAllText(file), file);
                        if (this.addon.Url.Length > 0)
                        {
                            this.LoadRemoteAddonData(this.addon);
                        }
                        Logger.Log("Finished MiniAVC.");
                        this.isLocked = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex, "AddonManager->Awake");
                    }
                }).Start();
            }

            #endregion

            #region Properties

            /// <summary>
            ///     Gets all the addon data for addons that contain version information.
            /// </summary>
            public AddonData Addon
            {
                get { return this.addon; }
            }

            /// <summary>
            ///     Gets whether the addon data has been locked.
            /// </summary>
            public bool IsLocked
            {
                get { return this.isLocked; }
            }

            /// <summary>
            ///     Gets whether any update or compatibility issues have been found.
            /// </summary>
            public bool HasIssues
            {
                get { return this.HasUpdateIssues || this.HasCompatibilityIssues; }
            }

            /// <summary>
            ///     Gets whether any update issues have been found.
            /// </summary>
            public bool HasUpdateIssues
            {
                get { return this.addon.UpdateAvailable; }
            }

            /// <summary>
            ///     Gets whether any compatibility issues have been found.
            /// </summary>
            public bool HasCompatibilityIssues
            {
                get { return !this.addon.GameCompatible; }
            }

            #endregion

            #region Private Methods

            private void LoadRemoteAddonData(AddonData addon)
            {
                try
                {
                    // Fetch the remote json data.
                    var www = new WWW(addon.Url);

                    var timer = new Stopwatch();
                    timer.Start();
                    // Running in thread so blocking is not a concern.
                    while (!www.isDone)
                    {
                        if (timer.ElapsedMilliseconds > 2000)
                        {
                            addon.RemoteAddonData = addon;
                            Logger.Log(addon.FileName.Replace(UrlDir.ApplicationRootPath, String.Empty) + " fetching of " + addon.Url + " timed out.");
                            return;
                        }
                    }

                    Logger.Log(addon.FileName.Replace(UrlDir.ApplicationRootPath, String.Empty) + " fetched remote version file at " + addon.Url + ".");

                    // Create the remote addon data using the remote jason data.
                    addon.RemoteAddonData = new AddonData(www.text, addon.Url);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex, "AddonManager->LoadRemoteAddonData");
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: Settings

        public class Settings
        {
            private bool firstRun = true;
            public static Settings Instance { get; private set; }

            public bool FirstRun
            {
                get { return this.firstRun; }
                set { this.firstRun = value; }
            }

            public bool AllowCheck { get; set; }

            public static void Save()
            {
                using (var stream = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MiniAVC.xml"), FileMode.Create))
                {
                    new XmlSerializer(typeof(Settings)).Serialize(stream, Instance);
                    stream.Close();
                }
            }

            public static void Load()
            {
                try
                {
                    using (var stream = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MiniAVC.xml"), FileMode.Open))
                    {
                        Instance = new XmlSerializer(typeof(Settings)).Deserialize(stream) as Settings;
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Instance = new Settings();
                    if (!(ex is IsolatedStorageException))
                    {
                        Logger.Exception(ex, "MiniAVC->Settings->Load");
                    }
                }
            }
        }

        #endregion
    }
}