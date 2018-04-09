using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SdfGbkText:Text{
	[SerializeField]
	[Range(0.0f,1.0f)]
	private float m_FontWeight=0f;
	public float fontWeight{
		get{
			return m_FontWeight;
		}
		set{
			m_FontWeight=value;
			this.SetVerticesDirty();
		}
	}
	
	[SerializeField]
	private bool m_GradientEnalbed=false;
	public bool gradientEnalbed{
		get{
			return m_GradientEnalbed;
		}
		set{
			m_GradientEnalbed=value;
			this.SetVerticesDirty();
		}
	}
	
	[SerializeField]
	private Color m_GradientTopColor=Color.yellow;
	public Color gradientTopColor{
		get{
			return m_GradientTopColor;
		}
		set{
			m_GradientTopColor=value;
			this.SetVerticesDirty();
		}
	}
	
	[SerializeField]
	private Color m_GradientBottomColor=Color.magenta;
	public Color gradientBottomColor{
		get{
			return m_GradientBottomColor;
		}
		set{
			m_GradientBottomColor=value;
			this.SetVerticesDirty();
		}
	}
	
	private static Color[] channelColors=new Color[]{
		new Color(1,0,0,0),new Color(0,1,0,0),new Color(0,0,1,0),new Color(0,0,0,1)
	};
	
	private static char[] channelRanges=new char[]{
		(char)24842,(char)30242,(char)35642
	};
	
	private static UIVertex[] tempVerts=new UIVertex[4];
	protected override void OnPopulateMesh(VertexHelper toFill){
		if(this.font==null){
			return;
		}
		var text=this.text;
		var fontWeight=m_FontWeight;
		var settings=GetGenerationSettings(rectTransform.rect.size);
		cachedTextGenerator.PopulateWithErrors(text,settings,gameObject);
		var verts=cachedTextGenerator.verts;
		var unitsPerPixel=1f/pixelsPerUnit;
		var roundingOffset=new Vector2(verts[0].position.x,verts[0].position.y)*unitsPerPixel;
		roundingOffset=PixelAdjustPoint(roundingOffset)-roundingOffset;
		var fontScale=transform.lossyScale.y*unitsPerPixel;
		int vertCount=verts.Count-4;
		toFill.Clear();
		if(roundingOffset!=Vector2.zero){
			for(int i=0;i<vertCount;++i){
				int tempVertsIndex=i&3;
				tempVerts[tempVertsIndex]=verts[i];
				SetVert(tempVertsIndex,text[i/4],unitsPerPixel,fontWeight,fontScale);
				tempVerts[tempVertsIndex].position.x+=roundingOffset.x;
				tempVerts[tempVertsIndex].position.y+=roundingOffset.y;
				if(tempVertsIndex==3){
					toFill.AddUIVertexQuad(tempVerts);
				}
			}
		}
		else{
			for(int i=0;i<vertCount;++i){
				int tempVertsIndex=i&3;
				tempVerts[tempVertsIndex]=verts[i];
				SetVert(tempVertsIndex,text[i/4],unitsPerPixel,fontWeight,fontScale);
				if(tempVertsIndex==3){
					toFill.AddUIVertexQuad(tempVerts);
				}
			}
		}
	}
	
	private void SetVert(int index,char textChar,float unitsPerPixel,float fontWeight,float fontScale){
		int channel=3;
		for(int i=0;i<3;i++){
			if(textChar<channelRanges[i]){
				channel=i;
				break;
			}
		}
		tempVerts[index].tangent=channelColors[channel];
		tempVerts[index].position*=unitsPerPixel;
		tempVerts[index].uv1.x=fontWeight;
		tempVerts[index].uv1.y=fontScale;
		if(m_GradientEnalbed){
			if(index>=2){
				tempVerts[index].color*=m_GradientTopColor;
			}
			else{
				tempVerts[index].color*=m_GradientBottomColor;
			}
		}
	}
}
