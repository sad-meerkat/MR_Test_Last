using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Templates.MRTTabletopAssets;

namespace Unity.AI.Assistant.PlayModeTest
{
    [InitializeOnLoad]
    internal static class PlayModeTestRunner
    {
        private const string StateKey = "PlayModeTest.State";
        private const string ResultKey = "PlayModeTest.Result";
        private const string ScriptPathKey = "PlayModeTest.ScriptPath";
        private const string SentinelLog = "PLAY_MODE_TEST_COMPLETE";

        private static readonly int WaitFrames = SessionState.GetInt("PlayModeTest.WaitFrames", 10);
        private static readonly float TestTimeout = SessionState.GetFloat("PlayModeTest.TestTimeout", 15.0f);

        private static List<string> _capturedLogs = new List<string>();
        private const int MaxCapturedLogs = 50;

        static PlayModeTestRunner()
        {
            string state = SessionState.GetString(StateKey, "Idle");
            switch (state)
            {
                case "WaitingForCompile":
                    EditorApplication.delayCall += () => {
                        SessionState.SetString(StateKey, "EnteringPlayMode");
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        EditorApplication.isPlaying = true;
                    };
                    break;
                case "EnteringPlayMode":
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    if (EditorApplication.isPlaying) {
                        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                        SessionState.SetString(StateKey, "InPlayMode");
                        EditorApplication.update += WaitFramesThenRun;
                    }
                    break;
                case "InPlayMode":
                    if (EditorApplication.isPlaying) EditorApplication.update += WaitFramesThenRun;
                    break;
                case "Done":
                    EditorApplication.delayCall += SelfDestruct;
                    break;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                SessionState.SetString(StateKey, "InPlayMode");
                EditorApplication.update += WaitFramesThenRun;
            }
        }

        private static int _frameCount = 0;
        private static bool _setupDone = false;
        private static bool _testDone = false;
        private static double _testStartTime = 0;

        private static void WaitFramesThenRun()
        {
            _frameCount++;
            if (_frameCount < WaitFrames) return;
            if (_testDone) return;

            if (!_setupDone)
            {
                _setupDone = true;
                Application.logMessageReceived += OnLogMessage;
                _testStartTime = EditorApplication.timeSinceStartup;
                try { Setup(); }
                catch (System.Exception e) { FinishTest(true, "Setup error: " + e.Message); return; }
                return;
            }

            float elapsed = (float)(EditorApplication.timeSinceStartup - _testStartTime);
            bool timedOut = elapsed >= TestTimeout;
            try
            {
                bool complete = Tick(elapsed);
                if (complete || timedOut) FinishTest(timedOut && !complete, timedOut ? "Timed out" : null);
            }
            catch (System.Exception e) { FinishTest(true, "Tick error: " + e.Message); }
        }

        private static void FinishTest(bool isError, string errorMessage)
        {
            _testDone = true;
            EditorApplication.update -= WaitFramesThenRun;
            Application.logMessageReceived -= OnLogMessage;
            string resultJson = (isError && errorMessage != null) ? 
                JsonUtility.ToJson(new TestResult { success = false, error = errorMessage, logs = _capturedLogs.ToArray() }) : 
                GetResult();
            SessionState.SetString(ResultKey, resultJson);
            SessionState.SetString(StateKey, "Done");
            EditorApplication.isPlaying = false;
        }

        private static void OnLogMessage(string message, string stackTrace, LogType type)
        {
            if (_capturedLogs.Count < MaxCapturedLogs) _capturedLogs.Add("[" + type + "] " + message);
        }

        private static void SelfDestruct()
        {
            string scriptPath = SessionState.GetString(ScriptPathKey, "");
            if (!string.IsNullOrEmpty(scriptPath) && AssetDatabase.AssetPathExists(scriptPath)) AssetDatabase.DeleteAsset(scriptPath);
            SessionState.EraseString(StateKey);
            SessionState.EraseString(ScriptPathKey);
        }

        [System.Serializable]
        public class TestResult
        {
            public bool success;
            public string error;
            public string[] logs;
            public Vector3 charStartPos;
            public Vector3 charEndPos;
            public Vector3 swordStartPos;
            public Vector3 swordEndPos;
            public string swordParent;
            public bool swordActive;
            public string charName;
            public string swordName;
        }

        private static TableCharacter _character;
        private static GameObject _sword;
        private static Vector3 _charStart;
        private static Vector3 _swordStart;

        private static void Setup()
        {
            _character = Object.FindAnyObjectByType<TableCharacter>();
            if (_character == null)
            {
                Debug.Log("[Test] No TableCharacter found, trying to summon one.");
                var summoner = Object.FindAnyObjectByType<TableCharacterSummoner>();
                if (summoner != null) summoner.Summon();
                _character = Object.FindAnyObjectByType<TableCharacter>();
            }

            if (_character == null) throw new System.Exception("Could not find or summon TableCharacter");

            Debug.Log("[Test] Found character: " + _character.name);
            
            var field = typeof(TableCharacter).GetField("m_SwordVisual", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) _sword = (GameObject)field.GetValue(_character);

            if (_sword == null) Debug.LogWarning("[Test] m_SwordVisual is null on character!");
            
            _character.SetSwordActive(true);
            _character.Move(new Vector2(1, 0)); 
            
            _charStart = _character.transform.position;
            if (_sword != null) _swordStart = _sword.transform.position;
            
            Debug.Log("[Test] Setup complete. Character Pos: " + _charStart + ", Sword Pos: " + (_sword != null ? _swordStart.ToString() : "N/A"));
        }

        private static bool Tick(float elapsed)
        {
            if (_character != null) _character.Move(new Vector2(1, 0));
            return elapsed >= 3.0f;
        }

        private static string GetResult()
        {
            var result = new TestResult
            {
                success = true,
                charName = _character != null ? _character.name : "null",
                charStartPos = _charStart,
                charEndPos = _character != null ? _character.transform.position : Vector3.zero,
                swordName = _sword != null ? _sword.name : "null",
                swordStartPos = _swordStart,
                swordEndPos = _sword != null ? _sword.transform.position : Vector3.zero,
                swordParent = (_sword != null && _sword.transform.parent != null) ? _sword.transform.parent.name : "None",
                swordActive = _sword != null && _sword.activeInHierarchy,
                logs = _capturedLogs.ToArray()
            };
            return JsonUtility.ToJson(result);
        }
    }
}
