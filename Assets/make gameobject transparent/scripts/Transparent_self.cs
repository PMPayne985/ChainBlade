using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparent_self : MonoBehaviour
{

    [Header("fade time")]
    public float fade_time;

    [Header("Need to be transparent")]
    public bool is_need_transparent;

    [Header("What should be the value of transparency 0~1")]
    [Range(0, 1)]
    public float alpha;

    //renderer Shader Color  of this gameobject
    private Renderer m_renderer;
    private Shader[] m_OldShaders;
    private Color[] m_OldColors;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (this.is_need_transparent)
        {
            if (this.m_renderer.material.shader != Shader.Find("Transparent/Diffuse"))
            {

                for (int i = 0; i < this.m_renderer.materials.Length; i++)
                {
                    this.m_renderer.materials[i].shader = Shader.Find("Transparent/Diffuse");
                }
            }



            for (int i = 0; i < this.m_renderer.materials.Length; i++)
            {
                if (this.m_renderer.materials[i].color.a != this.alpha)
                {
                    Color C = this.m_renderer.materials[i].color;
                    C.a = alpha;
                    this.m_renderer.materials[i].color = C;
                }
            }         
        }
        else
        {
            for (int i = 0; i < this.m_renderer.materials.Length; i++)
            {
                if (this.m_renderer.materials[i].shader != this.m_OldShaders[i])
                {
                    this.m_renderer.materials[i].shader = this.m_OldShaders[i];
                    this.m_renderer.materials[i].color = this.m_OldColors[i];
                }
            }      
        }
    }

    public void fade_in()
    {
        StartCoroutine(this.fade(this.fade_time, 0, 1));
    }

    public void fade_out()
    {
        StartCoroutine(this.fade(this.fade_time, 1, 0));
    }

    //fade
    private IEnumerator fade(float time, float from, float to)
    {
        this.is_need_transparent = true;

        float duration = time;

        float elaspedTime = 0f;
        while (elaspedTime <= duration)
        {
            elaspedTime += Time.deltaTime;
            this.alpha = Mathf.Lerp(from, to, elaspedTime / duration);
            yield return null;
        }
        this.alpha = to;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
