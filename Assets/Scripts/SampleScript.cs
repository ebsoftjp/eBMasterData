using UnityEngine;
using UnityEngine.UIElements;
using EbMasterData;

public class SampleScript : MonoBehaviour
{
    [Header("Document")]
    public UIDocument Document;

    private Button button;
    private Label label;

    void Start()
    {
        button = Document.rootVisualElement.Q<Button>();
        label = Document.rootVisualElement.Q<Label>();
        button.text = "Start";
        label.text = "Ready";

        button.clicked += Download;
    }

    private async void Download()
    {
        button.SetEnabled(false);
        label.text = "Download";

        var reader = new Reader((_, _, text) => { label.text = text; return false; });
        await reader.ReadText();
    }
}
