using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private string url;
        [SerializeField] private string assetfile;

        public void StartDownload()
        {
            StartCoroutine(DownloadAndImport());
        }
    
        private IEnumerator DownloadAndImport()
        {
            var www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            while (www.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }

            if (www.error != null)
            {
                Debug.Log("UnityWebRequest.error:" + www.error);
            }
            else if (www.downloadHandler.text == "" || www.downloadHandler.text.IndexOf("<!DOCTYPE", StringComparison.Ordinal) != -1)
            {
                Debug.Log("Uknown Format:" + www.downloadHandler.text);
            }
            else
            {
                ImportData(www.downloadHandler.text);
                Debug.Log("Imported Asset: " + assetfile);
            }
        }

        private void ImportData(string text)
        {
            var rows = CSVSerializer.ParseCSV(text);
            if (rows == null) return;
            var dd = AssetDatabase.LoadAssetAtPath<DialogueData>(assetfile);
            if (dd == null)
            {
                dd = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dd, assetfile);
            }
            dd.items = CSVSerializer.Deserialize<DialogueData.ItemText>(rows);

            EditorUtility.SetDirty(dd);
            AssetDatabase.SaveAssets();
        }
    }
}
