using UnityEngine;
using System.Collections;

public class AutoTransparent : MonoBehaviour
{
    private Shader[] m_OldShaders = null;
    private Color[] m_OldColors;

    private Renderer m_renderer;

    private Coroutine coroutine;

    void Start()
    {
        //init
        this.m_renderer = this.GetComponent<Renderer>();
        this.m_OldShaders = new Shader[this.m_renderer.materials.Length];
        this.m_OldColors = new Color[this.m_renderer.materials.Length];

        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {
            this.m_OldShaders[i] = this.m_renderer.materials[i].shader;
        }

        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {
            this.m_OldColors[i] = this.m_renderer.materials[i].color;
        }
    }

    //reset gameobject
    private IEnumerator reset_gameobject()
    {
        yield return new WaitForSeconds(1f);

        //set the shader and layer back
        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {
            this.m_renderer.materials[i].shader = this.m_OldShaders[i];
        }
        for (int i = 0; i < this.m_renderer.materials.Length; i++)
        {
            this.m_renderer.materials[i].color=this.m_OldColors[i];
        }
        this.gameObject.layer = 0;
    }

    //set the gameobject 
    public void set_obj_transparent_and_change_gamelayer(float alpha)
    {
        if (this.gameObject.layer != 1 && this.m_renderer != null)
        {

            for (int i = 0; i < this.m_renderer.materials.Length; i++)
            {
                this.m_renderer.materials[i].shader = Shader.Find("Transparent/Diffuse");

                Color C = this.m_renderer.materials[i].color;
                C.a = alpha;
                this.m_renderer.materials[i].color = C;
            }

            this.gameObject.layer = 1;
        }

        if (this.coroutine != null)
        {
            StopCoroutine(this.coroutine);
        }

        this.coroutine = StartCoroutine(this.reset_gameobject());
    }
}
