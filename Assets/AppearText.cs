using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AppearText : BaseMeshEffect
{
    private enum DisplayType
    {
        NoFade,
        Fade,
    }

    [SerializeField]
    private float _characterAppearTime = 0.5f;
    [SerializeField]
    private bool _playOnAwake = true;
    [SerializeField]
    private DisplayType _displayType = DisplayType.NoFade;

    private List<UIVertex> _vertexList = new List<UIVertex>();
    private UIVertex _vertex;
    private Color _color;
    private int _index;
    private int _maxIndex;
    private float _time;
    private bool _playable;

    protected override void Awake()
    {
        if (!_playOnAwake) return;

        _playable = true;
        _index = 0;
        _maxIndex = -1;
        _time = 0f;
        graphic.SetVerticesDirty();
    }
    private void Update()
    {
        if (!_playable || _index >= _maxIndex)
            return;

        if (_time >= _characterAppearTime)
        {
            _index += 6;
            _time -= _characterAppearTime;
            graphic.SetVerticesDirty();
            return;
        }
        _time += Time.deltaTime;
        if (_displayType == DisplayType.Fade)
            graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vertex)
    {
        _vertexList.Clear();
        vertex.GetUIVertexStream(_vertexList);
        // 最大文字数
        if (_maxIndex < 0)
            _maxIndex = _vertexList.Count;

        var count = _vertexList.Count;
        
        for (var i = 0; i < count; i += 6)
        {
            // フェード処理
            if (_displayType == DisplayType.Fade && i < _index + 6 && i >= _index)
            {
                for (var j = 0; j < 6; j++)
                    SetAlpha(i + j, _time / _characterAppearTime);
                continue;
            }

            if (i < _index)
                continue;

            for (var j = 0; j < 6; j++)
                SetAlpha(i + j, 0f);
        }
        
        vertex.Clear();
        vertex.AddUIVertexTriangleStream(_vertexList);
    }

    private void SetAlpha(int index, float alpha)
    {
        _vertex = _vertexList[index];

        _color = _vertex.color;
        _color.a = alpha;
        _vertex.color = _color;
        _vertexList[index] = _vertex;
    }

    public void Play()
    {
        _playable = true;
    }
}