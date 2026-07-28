// Keeps the shipped credits file in step with the canonical one at the repo root.
// CREDITS.txt lives at the project root so GitHub can serve it; StreamingAssets is what
// actually ends up in the build, so the root copy is mirrored there before every build.
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Signal.DevEditor
{
    public sealed class CreditsBuildSync : IPreprocessBuildWithReport
    {
        private const string FileName = "CREDITS.txt";
        private const string ShippedAssetPath = "Assets/StreamingAssets/" + FileName;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) => Sync(false);

        [MenuItem("Tools/Tasting Menu/Sync Credits To StreamingAssets")]
        public static void SyncFromMenu() => Sync(true);

        private static void Sync(bool verbose)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string source = Path.Combine(projectRoot, FileName);

            if (!File.Exists(source))
            {
                Debug.LogWarning($"[Credits] {FileName} is missing from the project root — the build will ship whatever is already in StreamingAssets.");
                return;
            }

            string destination = Path.Combine(projectRoot, ShippedAssetPath);
            string text = File.ReadAllText(source);

            if (File.Exists(destination) && File.ReadAllText(destination) == text)
            {
                if (verbose) Debug.Log($"[Credits] {ShippedAssetPath} is already up to date.");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.WriteAllText(destination, text);
            AssetDatabase.ImportAsset(ShippedAssetPath, ImportAssetOptions.ForceUpdate);
            Debug.Log($"[Credits] Copied {FileName} from the project root to {ShippedAssetPath}.");
        }
    }
}
