using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using eBMasterData;

public class SampleScript : MonoBehaviour
{
    [Header("Document")]
    public UIDocument Document;

    private MasterData.MDData data;
    private Button button;
    private Label label;

    void Start()
    {
        data = Resources.Load<MasterData.MDData>("eBMasterData/Data");
        button = Document.rootVisualElement.Q<Button>();
        label = Document.rootVisualElement.Q<Label>();
        button.text = "Start";
        label.text = $"Ready: {data.AdrData.Length}";

        button.clicked += Download;
    }

    private async void Download()
    {
        button.SetEnabled(false);
        label.text = "Download";

        var settings = Resources.Load<Settings>(Paths.SettingsPath);
        var reader = new ReaderForRuntime((_, _, text) => { label.text = text; return false; });
        await reader.CreateFileList();
        await reader.ReadText();
        reader.ParseData();
        data.Convert2(reader.ParsedTables, reader.ParsedValues);
        label.text = "Done";
    }
}
