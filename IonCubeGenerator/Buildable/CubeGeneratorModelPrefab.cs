namespace IonCubeGenerator.Buildable
{
    using System.IO;
    using System.Reflection;
    using Common;
    using FCStudioHelpers;
    using UnityEngine;
    // using Logger = QModManager.Utility.Logger;

    internal partial class CubeGeneratorBuildable
    {
        private AssetBundle _assetBundle;
        private GameObject _ionCubeGenPrefab;

        private bool GetPrefabs()
        {
            // == Get the prefab == //
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = Path.Combine(executingLocation, "Assets");
            string bundlePath = Path.Combine(folderPath, "ioncubegeneratorbundle");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);

            //If the result is null return false.
            if (assetBundle == null)
            {
                QuickLogger.Error($"AssetBundle is Null!");
                return false;
            }

            _assetBundle = assetBundle;
            // Logger.Log(Logger.Level.Debug, $"AssetBundle Set");
            //We have found the asset bundle and now we are going to continue by looking for the model.
            GameObject ionCubeGenPrefab = assetBundle.LoadAsset<GameObject>("IonCubeGenerator");

            //If the prefab isn't null lets add the shader to the materials
            if (ionCubeGenPrefab != null)
            {
                _ionCubeGenPrefab = ionCubeGenPrefab;

                //Lets apply the material shader
                ApplyShaders(_ionCubeGenPrefab);

                // Logger.Log(Logger.Level.Debug, $"IonCubeGen Prefab Found!");
            }
            else
            {
                QuickLogger.Error($"IonCubeGen Prefab Not Found!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies the shader to the materials of the reactor
        /// </summary>
        /// <param name="prefab">The prefab to apply shaders.</param>
        private void ApplyShaders(GameObject prefab)
        {
            #region SystemLights_BaseColor
            MaterialHelpers.ApplyEmissionShader("SystemLights_BaseColor", "SystemLights_OnMode_Emissive", prefab, _assetBundle, new Color(0.08235294f, 1f, 1f));
            MaterialHelpers.ApplyNormalShader("SystemLights_BaseColor", "SystemLights_Norm", prefab, _assetBundle);
            MaterialHelpers.ApplyAlphaShader("SystemLights_BaseColor", prefab);
            #endregion

            #region FCS_SUBMods_GlobalDecals
            MaterialHelpers.ApplyAlphaShader("FCS_SUBMods_GlobalDecals", prefab);
            MaterialHelpers.ApplyNormalShader("FCS_SUBMods_GlobalDecals", "FCS_SUBMods_GlobalDecals_Norm", prefab, _assetBundle);

            #endregion

            #region precursor_crystal_cube
            MaterialHelpers.ApplyPrecursorShader("precursor_crystal_cube", "precursor_crystal_cube_normal", "precursor_crystal_cube_spec", prefab, _assetBundle, 3f);
            #endregion

            #region Precursor_Tech1
            MaterialHelpers.ApplyEmissionShader("Precursor_Tech1", "Precursor_Tech1_Emissive", prefab, _assetBundle, new Color(0f, 4.521441f, 0.0311821f));
            MaterialHelpers.ApplyNormalShader("Precursor_Tech1", "Precursor_Tech1_Norm", prefab, _assetBundle);
            MaterialHelpers.ApplyMetallicShader("Precursor_Tech1", "Precursor_Tech1_Spec", prefab, _assetBundle, 0.2f);
            #endregion

            #region BaseCol1
            MaterialHelpers.ApplyMetallicShader("BaseCol1", "BaseCol1_Metallic", prefab, _assetBundle, 0.2f);
            MaterialHelpers.ApplyNormalShader("BaseCol1", "BaseCol1_Norm", prefab, _assetBundle);
            #endregion

            #region BaseCol1_Dark
            MaterialHelpers.ApplyMetallicShader("BaseCol1_Dark", "BaseCol1_Metallic", prefab, _assetBundle, 0.2f);
            MaterialHelpers.ApplyNormalShader("BaseCol1_Dark", "BaseCol1_Norm", prefab, _assetBundle);
            #endregion
        }
    }
}
