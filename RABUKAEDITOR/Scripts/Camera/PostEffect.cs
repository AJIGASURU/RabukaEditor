using UnityEngine;

[ExecuteInEditMode]
public class PostEffect : MonoBehaviour
{

    //[SerializeField]//これはプライベートでもインスペクタから入力できる。
    public Material _material;

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, _material);
    }
}
