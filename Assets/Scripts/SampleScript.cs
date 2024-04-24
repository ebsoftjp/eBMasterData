using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using EbMasterData;

public class SampleScript : MonoBehaviour
{
    [Header("Document")]
    public UIDocument Document;

    private MasterData.MDData data;
    private Button button;
    private Label label;

    void Start()
    {
        data = Resources.Load<MasterData.MDData>("EbMasterData/Data");
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
        var parser = new Parser(settings.LineSplitString, settings.FieldSplitString);
        var reader = new ReaderForRuntime((_, _, text) => { label.text = text; return false; });
        await reader.CreateFileList();
        await reader.ReadText();
        data.Convert2(
            reader.data2.Select(v => v.Name).ToArray(),
            reader.data2.Select(v => parser.Exec(v.Text).Skip(settings.HeaderLines).ToArray()).ToArray());
        label.text = "Done";
    }
}
