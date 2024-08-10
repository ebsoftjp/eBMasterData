using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

        // random debug
        var rand = new System.Random(2);
        var list = Enumerable.Repeat(0, 100000).Select(_ => MD.AdrData.ArrayAt("Three").RandomRateAt(rand).OrderText).ToArray();
        Debug.Log(string.Join("\n", MD.AdrData.Select(v => $"[{v.OrderText}] {list.Count(l => l == v.OrderText)}")));
        //Debug.Log(string.Join("\n", list));
    }

    private async void Download()
    {
        button.SetEnabled(false);

        await eBMasterData.Utility.Reload(MD.Tables, (_, _, text) =>
        {
            label.text = text;
            return false;
        });
        label.text = "Done";
    }
}
