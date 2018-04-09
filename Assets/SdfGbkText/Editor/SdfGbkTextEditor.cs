using System.CodeDom;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

[CanEditMultipleObjects]
[CustomEditor(typeof(SdfGbkText),true)]
public class SdfGbkTextEditor:UnityEditor.UI.TextEditor {
	
	static List<int> gbkChars;
	
	static void Add(int start,int end){
		for(int i=start;i<=end;i++){
			gbkChars.Add(i);
		}
	}
	
	static void Add(int code){
		gbkChars.Add(code);
	}
	
	static void InitGBK(){
		if(gbkChars==null){
			gbkChars=new List<int>();
			Add(0x0020,0x007E);
			Add(0x00A4);
			Add(0x00A7,0x00A8);
			Add(0x00B0,0x00B1);
			Add(0x00B7);
			Add(0x00D7);
			Add(0x00E0,0x00E1);
			Add(0x00E8,0x00EA);
			Add(0x00EC,0x00ED);
			Add(0x00F2,0x00F3);
			Add(0x00F7);
			Add(0x00F9,0x00FA);
			Add(0x00FC);
			Add(0x0101);
			Add(0x0113);
			Add(0x011B);
			Add(0x012B);
			Add(0x0144);
			Add(0x0148);
			Add(0x014D);
			Add(0x016B);
			Add(0x01CE);
			Add(0x01D0);
			Add(0x01D2);
			Add(0x01D4);
			Add(0x01D6);
			Add(0x01D8);
			Add(0x01DA);
			Add(0x01DC);
			Add(0x0251);
			Add(0x0261);
			Add(0x02C7);
			Add(0x02C9,0x02CB);
			Add(0x02D9);
			Add(0x0391,0x03A1);
			Add(0x03A3,0x03A9);
			Add(0x03B1,0x03C1);
			Add(0x03C3,0x03C9);
			Add(0x0401);
			Add(0x0410,0x044F);
			Add(0x0451);
			Add(0x2010);
			Add(0x2013,0x2016);
			Add(0x2018,0x2019);
			Add(0x201C,0x201D);
			Add(0x2025,0x2026);
			Add(0x2030);
			Add(0x2032,0x2033);
			Add(0x2035);
			Add(0x203B);
			Add(0x2103);
			Add(0x2105);
			Add(0x2109);
			Add(0x2116);
			Add(0x2121);
			Add(0x2160,0x216B);
			Add(0x2170,0x2179);
			Add(0x2190,0x2193);
			Add(0x2196,0x2199);
			Add(0x2208);
			Add(0x220F);
			Add(0x2211);
			Add(0x2215);
			Add(0x221A);
			Add(0x221D,0x2220);
			Add(0x2223);
			Add(0x2225);
			Add(0x2227,0x222B);
			Add(0x222E);
			Add(0x2234,0x2237);
			Add(0x223D);
			Add(0x2248);
			Add(0x224C);
			Add(0x2252);
			Add(0x2260,0x2261);
			Add(0x2264,0x2267);
			Add(0x226E,0x226F);
			Add(0x2295);
			Add(0x2299);
			Add(0x22A5);
			Add(0x22BF);
			Add(0x2312);
			Add(0x2460,0x2469);
			Add(0x2474,0x249B);
			Add(0x2500,0x254B);
			Add(0x2550,0x2573);
			Add(0x2581,0x258F);
			Add(0x2593,0x2595);
			Add(0x25A0,0x25A1);
			Add(0x25B2,0x25B3);
			Add(0x25BC,0x25BD);
			Add(0x25C6,0x25C7);
			Add(0x25CB);
			Add(0x25CE,0x25CF);
			Add(0x25E2,0x25E5);
			Add(0x2605,0x2606);
			Add(0x2609);
			Add(0x2640);
			Add(0x2642);
			Add(0x3000,0x3003);
			Add(0x3005,0x3017);
			Add(0x301D,0x301E);
			Add(0x3021,0x3029);
			Add(0x3041,0x3093);
			Add(0x309B,0x309E);
			Add(0x30A1,0x30F6);
			Add(0x30FC,0x30FE);
			Add(0x3105,0x3129);
			Add(0x3220,0x3229);
			Add(0x3231);
			Add(0x32A3);
			Add(0x338E,0x338F);
			Add(0x339C,0x339E);
			Add(0x33A1);
			Add(0x33C4);
			Add(0x33CE);
			Add(0x33D1,0x33D2);
			Add(0x33D5);
			Add(0x4E00,0x9FA5);
			Add(0xF92C);
			Add(0xF979);
			Add(0xF995);
			Add(0xF9E7);
			Add(0xF9F1);
			Add(0xFA0C,0xFA0F);
			Add(0xFA11);
			Add(0xFA13,0xFA14);
			Add(0xFA18);
			Add(0xFA1F,0xFA21);
			Add(0xFA23,0xFA24);
			Add(0xFA27,0xFA29);
			Add(0xFE30,0xFE31);
			Add(0xFE33,0xFE44);
			Add(0xFE49,0xFE52);
			Add(0xFE54,0xFE57);
			Add(0xFE59,0xFE66);
			Add(0xFE68,0xFE6B);
			Add(0xFF01,0xFF5E);
			Add(0xFFE0,0xFFE5);
		}
	}
	
	static int fontSize=23;
	static int charPadding=2;
	static int textureSize=2048;
	static int[] charCounts=new int[]{5686,5400,5400,5400};
	static string outputFolder="Assets/SdfGbkText/Fonts/";
	
	static Color32[] textureColors;
	static List<CharacterInfo> charInfoList;
	
	static string fontName;
	static int charCount;
	static int[] charCodes;
	static int textureChannel;
	static byte[] textureBuffer;
	static FT_FaceInfo faceInfo;
	static FT_GlyphInfo[] glyphInfo;
	static bool renderCompleted;
	
	[MenuItem("Assets/Create SDF GBK Font",false,1)]
	static void CreateFont(){
		InitGBK();
		string path=AssetDatabase.GetAssetPath(Selection.activeObject);
		fontName=Path.GetFileNameWithoutExtension(path);
		int errorCode=TMPro_FontPlugin.Initialize_FontEngine();
		if(errorCode==0||errorCode==99){
			errorCode=TMPro_FontPlugin.Load_TrueType_Font(path);
			if(errorCode==0||errorCode==99){
				errorCode=TMPro_FontPlugin.FT_Size_Font(fontSize);
				if(errorCode==0||errorCode==99){
					EditorApplication.update+=RenderUpdate;
					textureColors=new Color32[textureSize*textureSize];
					charInfoList=new List<CharacterInfo>();
					CreateTexture(0);
				}
			}
		}
	}
	
	static void CreateTexture(int channel){
		renderCompleted=false;
		textureChannel=channel;
		int charStartIndex=0;
		for(int i=0;i<channel;i++){
			charStartIndex+=charCounts[i];
		}
		charCount=charCounts[channel];
		charCodes=new int[charCount];
		for(int i=0;i<charCount;i++){
			charCodes[i]=gbkChars[charStartIndex+i];
		}
		faceInfo=new FT_FaceInfo();
		glyphInfo=new FT_GlyphInfo[charCount];
		textureBuffer=new byte[textureSize*textureSize];
		ThreadPool.QueueUserWorkItem(SomeTask=>{
			TMPro_FontPlugin.Render_Characters(textureBuffer,textureSize,textureSize,charPadding,charCodes,charCount,FaceStyles.Normal,32,false,RenderModes.DistanceField16,0,ref faceInfo,glyphInfo);
			renderCompleted=true;
		});
	}
	
	static void RenderUpdate(){
		if(renderCompleted){
			if(textureChannel==0){
				for(int i=0;i<textureColors.Length;i++){
					textureColors[i].r=textureBuffer[i];
				}
			}
			if(textureChannel==1){
				for(int i=0;i<textureColors.Length;i++){
					textureColors[i].g=textureBuffer[i];
				}
			}
			if(textureChannel==2){
				for(int i=0;i<textureColors.Length;i++){
					textureColors[i].b=textureBuffer[i];
				}
			}
			if(textureChannel==3){
				for(int i=0;i<textureColors.Length;i++){
					textureColors[i].a=textureBuffer[i];
				}
			}
			
			var size=(float)textureSize;
			var ascender=faceInfo.ascender;
			var glyphCount=faceInfo.characterCount;
			
			var charSet=new HashSet<int>();
			
			for(int i=0;i<glyphCount;i++){
				var charInfo=new CharacterInfo();
				var id=glyphInfo[i].id;
				var x=(glyphInfo[i].x-charPadding);
				var y=(glyphInfo[i].y-charPadding);
				var width=(glyphInfo[i].width+charPadding+charPadding);
				var height=(glyphInfo[i].height+charPadding+charPadding);
				var xOffset=glyphInfo[i].xOffset;
				var yOffset=glyphInfo[i].yOffset-ascender;
				var xAdvance=glyphInfo[i].xAdvance;
				var uvMinX=Mathf.Round(x)/size;
				var uvMinY=Mathf.Round(size-y-height)/size;
				var uvMaxX=Mathf.Round(x+width)/size;
				var uvMaxY=Mathf.Round(size-y)/size;
				charInfoList.Add(charInfo);
				charInfo.uvTopLeft=new Vector2(uvMinX,uvMinY);
				charInfo.uvTopRight=new Vector2(uvMaxX,uvMinY);
				charInfo.uvBottomLeft=new Vector2(uvMinX,uvMaxY);
				charInfo.uvBottomRight=new Vector2(uvMaxX,uvMaxY);
				charInfo.minX=Mathf.RoundToInt(xOffset);
				charInfo.minY=Mathf.RoundToInt(yOffset);
				charInfo.maxX=Mathf.RoundToInt(xOffset+width);
				charInfo.maxY=Mathf.RoundToInt(yOffset-height);
				charInfo.advance=Mathf.RoundToInt(xAdvance);
				charInfo.index=id;
				charSet.Add(id);
			}
			
			for(int i=0;i<charCodes.Length;i++){
				if(!charSet.Contains(charCodes[i])){
					Debug.Log("Missing: "+(char)charCodes[i]+"|"+i+"|"+charCodes[i]+"|0x"+charCodes[i].ToString("X4"));
				}
			}
				
			Debug.Log("Glyph:"+glyphCount+", "+charCodes[0]+" - "+charCodes[charCodes.Length-1]);
			
			if(textureChannel<3){
				CreateTexture(textureChannel+1);
			}
			else{
				EditorUtility.ClearProgressBar();
				EditorApplication.update-=RenderUpdate;
				TMPro_FontPlugin.Destroy_FontEngine();
				
				var texturePath=outputFolder+fontName+".png";
				var materialPath=outputFolder+fontName+".mat";
				var fontPath=outputFolder+fontName+".fontsettings";
				
				var texture=new Texture2D(textureSize,textureSize,TextureFormat.RGBA32,false,true);
				texture.SetPixels32(textureColors);
				File.WriteAllBytes(texturePath,texture.EncodeToPNG());
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				var import=AssetImporter.GetAtPath(texturePath) as TextureImporter;
				import.textureCompression=TextureImporterCompression.Uncompressed;
				import.alphaIsTransparency=false;
				import.mipmapEnabled=false;
				AssetDatabase.ImportAsset(texturePath);
				
				var material=new Material(Shader.Find("SDF/GBK/Text"));
				material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
				AssetDatabase.CreateAsset(material,materialPath);
				
				var font=new Font(fontName);
				var mFont=new SerializedObject(font);
				mFont.FindProperty("m_FontSize").floatValue=faceInfo.pointSize;
				mFont.FindProperty("m_LineSpacing").floatValue=faceInfo.lineHeight;
				mFont.ApplyModifiedProperties();
				font.material=material;
				font.characterInfo=charInfoList.ToArray();
				AssetDatabase.CreateAsset(font,fontPath);
			}
		}
		else{
			float progress=TMPro_FontPlugin.Check_RenderProgress();
			string progressText="Channel: "+textureChannel+" , Char: "+(int)(progress*charCount)+"/"+charCount;
			EditorUtility.DisplayProgressBar("Create Font",progressText,progress);
		}
	}
	
	[MenuItem("GameObject/UI/SdfGbkText")]
	static void Create(){
		var gameObject=new GameObject("SdfGbkText");
		gameObject.transform.SetParent(Selection.activeTransform,false);
		gameObject.AddComponent<SdfGbkText>();
		Selection.activeGameObject=gameObject;
	}
	
	private SerializedProperty m_FontWeight;
	private SerializedProperty m_GradientEnalbed;
	private SerializedProperty m_GradientTopColor;
	private SerializedProperty m_GradientBottomColor;
	
	protected override void OnEnable(){
		base.OnEnable();
		this.m_FontWeight=this.serializedObject.FindProperty("m_FontWeight");
		this.m_GradientEnalbed=this.serializedObject.FindProperty("m_GradientEnalbed");
		this.m_GradientTopColor=this.serializedObject.FindProperty("m_GradientTopColor");
		this.m_GradientBottomColor=this.serializedObject.FindProperty("m_GradientBottomColor");
	}
	
	public override void OnInspectorGUI(){
		this.serializedObject.Update();
		EditorGUILayout.PropertyField(this.m_FontWeight);
		EditorGUILayout.PropertyField(this.m_GradientEnalbed);
		EditorGUILayout.PropertyField(this.m_GradientTopColor);
		EditorGUILayout.PropertyField(this.m_GradientBottomColor);
		this.serializedObject.ApplyModifiedProperties();
		base.OnInspectorGUI();
	}
}
