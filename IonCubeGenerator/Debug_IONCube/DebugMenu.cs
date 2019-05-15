#if DEBUG
using Common;
using IonCubeGenerator.Debug_IONCube.Patches;
using IonCubeGenerator.Mono;
using UnityEngine;

namespace IonCubeGenerator.Debug_IONCube
{
    public class DebugMenu : MonoBehaviour
    {
        private int showDebugMenuNo = 4;
        private bool isOpen = false;
        private static Rect SIZE = new Rect(5, 5, 500, 600);
        private bool showCursor = false;

        private void Start()
        {
            if (temp.Prefab != null)
            {
                _x = temp.Prefab.transform.localPosition.x.ToString();
                _y = temp.Prefab.transform.localPosition.y.ToString();
                _z = temp.Prefab.transform.localPosition.z.ToString();
            }
        }

        public void Open()
        {
            isOpen = true;
            showCursor = true;
        }

        public void Close()
        {
            isOpen = false;
            showCursor = false;
            UWE.Utils.alwaysLockCursor = true;
            UWE.Utils.lockCursor = true;
        }

        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Update()
        {
            if (isOpen == false)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                showCursor = !showCursor;
            }

            if (showCursor)
            {
                UWE.Utils.alwaysLockCursor = false;
                UWE.Utils.lockCursor = false;
            }
            else
            {
                UWE.Utils.alwaysLockCursor = true;
                UWE.Utils.lockCursor = true;
            }

            if (temp.Slot != null)
            {
                if (_toggle)
                {

                }
                else
                {
                    temp.Slot.SetActive(false);
                }

            }
        }

        private void OnGUI()
        {
            if (isOpen == false)
            {
                return;
            }

            Rect windowRect = GUILayout.Window(2352, SIZE, (id) =>
            {
                GUILayout.Box("P to show/hide cursor");

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Debug"))
                    showDebugMenuNo = 0;

                GUILayout.EndHorizontal();

                GUILayout.Space(10f);

                if (showDebugMenuNo == 0)
                {
                    AudioTestMenu();
                    //DrawDebugMenu();
                }
                else
                    GUILayout.Label("No Menu Selected");

            }, "FCS DEBUG WINDOW");
        }

        private void AudioTestMenu()
        {
            GUILayout.BeginHorizontal();

            FMODAsset waterFilterLoop = null;

            if (GUILayout.Button("Play Filter Audio"))
            {


                var machine = Resources.Load<GameObject>("Submarine/Build/FiltrationMachine");

                FMODAsset[] fmods = Resources.FindObjectsOfTypeAll<FMODAsset>();
                foreach (FMODAsset fmod in fmods)
                {
                    switch (fmod.name.ToLower())
                    {
                        case "water_filter_loop":
                            waterFilterLoop = fmod;
                            QuickLogger.Debug("Sound Located", true);
                            break;
                    }
                }


                if (waterFilterLoop != null)
                {
                    Utils.PlayFMODAsset(waterFilterLoop, MainCamera.camera.transform, 20f);
                }
                else
                {
                    QuickLogger.Error("WATER_FILTER_LOOP is null", true);
                }

            }

            GUILayout.EndHorizontal();
        }

        private void DrawDebugMenu()
        {

            GUILayout.BeginHorizontal();
            _x = GUILayout.TextField(_x);
            if (GUILayout.Button("Apply X"))
            {
                temp.Prefab.transform.localPosition = new Vector3(float.Parse(_x), temp.Prefab.transform.localPosition.y, temp.Prefab.transform.localPosition.z);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _y = GUILayout.TextField(_y);
            if (GUILayout.Button("Apply Y"))
            {
                temp.Prefab.transform.localPosition = new Vector3(temp.Prefab.transform.localPosition.x, float.Parse(_y), temp.Prefab.transform.localPosition.z);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _z = GUILayout.TextField(_z);
            if (GUILayout.Button("Apply Z"))
            {
                QuickLogger.Info($"{float.Parse(_z)} || {temp.Prefab.name}", true);
                temp.Prefab.transform.localPosition = new Vector3(temp.Prefab.transform.localPosition.x, temp.Prefab.transform.localPosition.y, float.Parse(_z));
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _toggle = GUILayout.Toggle(temp.Slot.activeSelf, "Prefab Visible");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.TextField(temp.AnimatiorState.normalizedTime.ToString());
            if (GUILayout.Button("Change Animation Open"))
            {
                temp.Animator.Play("Main", 0, 0.69f);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.TextField(temp.AnimatiorState.normalizedTime.ToString());
            if (GUILayout.Button("Change Animation Close"))
            {
                temp.Animator.Play("Main", 0, 0.7725137f);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.TextField(temp.AnimatiorState.normalizedTime.ToString());
            if (GUILayout.Button("Change Animation Done"))
            {
                temp.Animator.Play("Main", 0, 0.9937581f);
            }
            GUILayout.EndHorizontal();



            //GUILayout.BeginHorizontal();

            //if (GUILayout.Button("Play Animation"))
            //{
            //    Log.Info("Playing Animation");
            //}
            //GUILayout.EndHorizontal();

            //GUILayout.Space(10);

            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Stop Animation"))
            //{
            //    Log.Info("Stopping Animation");

            //}
            //GUILayout.EndHorizontal();

            //GUILayout.Space(10);


            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Get Deep harvester Count"))
            //    GetDeepHarvesterCount();
            //GUILayout.EndHorizontal();

        }


        public static DebugMenu main
        {
            get
            {
                if (_main == null)
                {
                    _main = Player.main.gameObject.AddComponent<DebugMenu>();
                }

                return _main;
            }
        }

        private static DebugMenu _main;
        private bool _toggle;
        private string _x;
        private string _y;
        private string _z;
        private readonly string _filterAudio;
    }
}
#endif