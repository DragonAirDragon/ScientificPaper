using UnityEngine;
using UnityEngine.Rendering;
public class SRPBatcherToogle : MonoBehaviour
{
    [SerializeField] private bool useSRPBatcher;

    void Awake(){
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
    }


}   
