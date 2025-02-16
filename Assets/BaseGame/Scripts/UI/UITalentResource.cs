using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using UnityEngine.UI;

public class UITalentResource: MonoBehaviour
{
    private PlayerResourceData PlayerResourceDataCache { get; set; }
    private PlayerResourceData PlayerResourceData => PlayerResourceDataCache ??= PlayerResourceData.Instance;
    [field: SerializeField] public GameResource.Type ResourceType {get; private set;}
    [field: SerializeField] public TextMeshProUGUI TextResource {get; private set;}
    [field: SerializeField] public Button ButtonShowInfo {get; private set;}
    [field: SerializeField] public GameObject InformationGroup {get; private set;}
    private CancellationTokenSource CancellationTokenSource { get; set; }
    private void Awake()
    {
        PlayerResourceData.GetGameResource(ResourceType).ReactiveAmount.ReactiveProperty.Subscribe(OnResourceChange).AddTo(this);
        ButtonShowInfo.SetOnClickDestination(OnClickShowInfo);
        InformationGroup.SetActive(false);
    }
    private void OnResourceChange(BigNumber amount)
    {
        TextResource.text = amount.ToString();
    }
    private async UniTask OnClickShowInfo()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        
        InformationGroup.SetActive(true);
        CancellationTokenSource = new CancellationTokenSource();
        await UniTask.Delay(2000, cancellationToken: CancellationTokenSource.Token);
        InformationGroup.SetActive(false);
    }
    
    
}