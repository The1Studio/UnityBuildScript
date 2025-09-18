using GameFoundation.BuildScripts.Runtime;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
internal sealed class BuildInfo : MonoBehaviour
{
    private TMP_Text infoText;

    private void Awake()
    {
        this.GetComponent<TMP_Text>().text = $"v{GameVersion.Version} - build: {GameVersion.BuildNumber}";
    }
}