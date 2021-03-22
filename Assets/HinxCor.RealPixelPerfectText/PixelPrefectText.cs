using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Drawing;
using UnityEngine;
using System.Drawing.Text;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Font = System.Drawing.Font;
using Color = System.Drawing.Color;
using UnityColor = UnityEngine.Color;
using System.Runtime.InteropServices;

public class PixelPrefectText : RawImage
{
    static Font defaultFont;


    static PixelPrefectText()
    {
        PrivateFontCollection pcol = new PrivateFontCollection();
        if (File.Exists("font.ttf"))
            pcol.AddFontFile(Path.Combine(Environment.CurrentDirectory, "font.ttf"));
        else
        if (File.Exists("font.ttc"))
            pcol.AddFontFile(Path.Combine(Environment.CurrentDirectory, "font.ttc"));
        else
            pcol.AddFontFile(@"C:\Windows\Fonts\msyh.ttc");
        var fml = pcol.Families[0];
        defaultFont = new Font(fml, 14, GraphicsUnit.Pixel);
    }



    public string Text { get => m_text; set => m_text = value; }

    [SerializeField]
    [TextArea(5, 10)]
    protected string m_text = string.Empty;
    [SerializeField] BrushesColor brushColor = BrushesColor.White;
    [SerializeField] TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
    [SerializeField] private string m_fontfile = @"C:\Windows\Fonts\msyh.ttc";
    [SerializeField] float emSize = 12;
    [SerializeField] GraphicsUnit unit = GraphicsUnit.Pixel;
    [SerializeField] bool fixColor = true;
    [SerializeField] bool userCustomCurve = false;
    [SerializeField]
    AnimationCurve alphaMapCurve = new AnimationCurve()
    {
        keys = new[] {
            new Keyframe(0,0),
            new Keyframe(1,1)
        }
    };

    private Font m_font;
    private Vector2 m_MesuredSize;

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        m_UpdateDrawingTexture();
        base.OnValidate();
    }

#endif

    private void m_UpdateDrawingTexture()
    {
        string txtToDraw = Text;
        if (string.IsNullOrEmpty(txtToDraw))
            txtToDraw = " ";
        var font = getFont();
        if (font == null)
            return;
        try
        {
            m_DrawDetailsWith(txtToDraw, font);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Unable draw text :" + txtToDraw);
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);

            m_DrawDetailsWith(txtToDraw, defaultFont);
        }
    }

    private void m_DrawDetailsWith(string txtToDraw, Font font)
    {
        Bitmap bmp = new Bitmap(1, 1);
        using (var graphic = System.Drawing.Graphics.FromImage(bmp))
        {
            graphic.TextRenderingHint = TextRenderingHint;
            var size = graphic.MeasureString(txtToDraw, font);
            this.m_MesuredSize = new Vector2(size.Width, size.Height);
        }
        bmp = new Bitmap((int)m_MesuredSize.x, (int)m_MesuredSize.y);

        string colorstr = brushColor.ToString();
        var syscolor = Color.FromName(colorstr);
        var brush = new SolidBrush(syscolor);

        using (var graphic = System.Drawing.Graphics.FromImage(bmp))
        {
            graphic.TextRenderingHint = TextRenderingHint;
            graphic.DrawString(txtToDraw, font, brush, PointF.Empty);
            graphic.Flush();
        }

        if (fixColor)
        {
            ClearImageColor(bmp);
        }



        var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var pngdat = ms.ToArray();

        if (texture != null)
            DestroyImmediate(texture);
        var t = new Texture2D(1, 1);
        t.LoadImage(pngdat);
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;
        t.name = "___sys_create_txt_bmp";
        texture = t;
        SetNativeSize();
    }

    private void ClearImageColor(Bitmap bmp)
    {
        FastBitmapLib.FastBitmap fbmp = new FastBitmapLib.FastBitmap(bmp);
        fbmp.Lock();
        for (int x = 0; x < bmp.Width; x++)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                var pix = fbmp.GetPixel(x, y);
                var npix = PixChange(pix);
                fbmp.SetPixel(x, y, npix);
            }
        }
        fbmp.Unlock();
    }

    private Color PixChange(Color pix)
    {
        if (pix.A == 0)
            return pix;
        var s = pix.GetSaturation();
        var b = pix.GetBrightness();
        //var h = pix.GetHue();
        //var uColor = UnityColor.HSVToRGB(h, s, b);
        //var ba = b;
        var sa = 1 - s;
        var ta = sa > b ? sa : b;
        if (userCustomCurve)
            ta = alphaMapCurve.Evaluate(ta);
        ta = Mathf.Clamp01(ta);
        return Color.FromArgb((int)(255 * ta), Color.White);
    }

    [DllImport("shlwapi.dll")]
    public static extern int ColorHLSToRGB(int H, int L, int S);

    private Font getFont()
    {
        //if (m_font == null)
        updateFontFile();
        return m_font;
    }


    private void updateFontFile()
    {
        var col = new PrivateFontCollection();
        col.AddFontFile(m_fontfile);
        var fml = col.Families[0];
        m_font = new Font(fml, emSize, unit);
    }


    enum BrushesColor
    {
        AliceBlue,
        PaleGoldenrod,
        Orchid,
        OrangeRed,
        Orange,
        OliveDrab,
        Olive,
        OldLace,
        Navy,
        NavajoWhite,
        Moccasin,
        MistyRose,
        MintCream,
        MidnightBlue,
        MediumVioletRed,
        MediumTurquoise,
        MediumSpringGreen,
        MediumSlateBlue,
        LightSkyBlue,
        LightSlateGray,
        LightSteelBlue,
        LightYellow,
        Lime,
        LimeGreen,
        PaleGreen,
        Linen,
        Maroon,
        MediumAquamarine,
        MediumBlue,
        MediumOrchid,
        MediumPurple,
        MediumSeaGreen,
        Magenta,
        PaleTurquoise,
        PaleVioletRed,
        PapayaWhip,
        SlateGray,
        Snow,
        SpringGreen,
        SteelBlue,
        Tan,
        Teal,
        SlateBlue,
        Thistle,
        Transparent,
        Turquoise,
        Violet,
        Wheat,
        White,
        WhiteSmoke,
        Tomato,
        LightSeaGreen,
        SkyBlue,
        Sienna,
        PeachPuff,
        Peru,
        Pink,
        Plum,
        PowderBlue,
        Purple,
        Silver,
        Red,
        RoyalBlue,
        SaddleBrown,
        Salmon,
        SandyBrown,
        SeaGreen,
        SeaShell,
        RosyBrown,
        Yellow,
        LightSalmon,
        LightGreen,
        DarkRed,
        DarkOrchid,
        DarkOrange,
        DarkOliveGreen,
        DarkMagenta,
        DarkKhaki,
        DarkGreen,
        DarkGray,
        DarkGoldenrod,
        DarkCyan,
        DarkBlue,
        Cyan,
        Crimson,
        Cornsilk,
        CornflowerBlue,
        Coral,
        Chocolate,
        AntiqueWhite,
        Aqua,
        Aquamarine,
        Azure,
        Beige,
        Bisque,
        DarkSalmon,
        Black,
        Blue,
        BlueViolet,
        Brown,
        BurlyWood,
        CadetBlue,
        Chartreuse,
        BlanchedAlmond,
        DarkSeaGreen,
        DarkSlateBlue,
        DarkSlateGray,
        HotPink,
        IndianRed,
        Indigo,
        Ivory,
        Khaki,
        Lavender,
        Honeydew,
        LavenderBlush,
        LemonChiffon,
        LightBlue,
        LightCoral,
        LightCyan,
        LightGoldenrodYellow,
        LightGray,
        LawnGreen,
        LightPink,
        GreenYellow,
        Gray,
        DarkTurquoise,
        DarkViolet,
        DeepPink,
        DeepSkyBlue,
        DimGray,
        DodgerBlue,
        Green,
        Firebrick,
        ForestGreen,
        Fuchsia,
        Gainsboro,
        GhostWhite,
        Gold,
        Goldenrod,
        FloralWhite,
        YellowGreen,
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(PixelPrefectText)), CanEditMultipleObjects]
    class PixelPrefectText_EditorPanel : Editor
    {
        SerializedProperty m_text_prop;
        SerializedProperty m_color_prop;
        SerializedProperty m_brushColor;
        SerializedProperty m_TextRenderingHint;
        SerializedProperty m_fontfile;
        SerializedProperty m_FontSize;
        SerializedProperty m_fontSizeUnit;
        SerializedProperty m_fix_color;
        SerializedProperty m_userCustomCurve;
        SerializedProperty m_alphaMapCurve;

        GUIContent m_text_prop_prefix;
        GUIContent m_color_prop_prefix;
        GUIContent m_brushColor_prefix;
        GUIContent m_TextRenderingHint_prefix;
        GUIContent m_fix_color_prefix;
        GUIContent m_useCustomCurve;
        /*
         bool userCustomCurve = false;
             alphaMapCurve
             */

        private void OnEnable()
        {
            m_text_prop = serializedObject.FindProperty("m_text");
            m_color_prop = serializedObject.FindProperty("m_Color");
            m_TextRenderingHint = serializedObject.FindProperty("TextRenderingHint");
            m_brushColor = serializedObject.FindProperty("brushColor");
            m_fontfile = serializedObject.FindProperty("m_fontfile");
            m_FontSize = serializedObject.FindProperty("emSize");
            m_fontSizeUnit = serializedObject.FindProperty("unit");
            m_fix_color = serializedObject.FindProperty("fixColor");
            m_userCustomCurve = serializedObject.FindProperty("userCustomCurve");
            m_alphaMapCurve = serializedObject.FindProperty("alphaMapCurve");

            m_color_prop_prefix = new GUIContent("Color", "Color for this text");
            m_text_prop_prefix = new GUIContent("Text", "pure string for this text");
            m_brushColor_prefix = new GUIContent("Brush Color");
            m_TextRenderingHint_prefix = new GUIContent("Text Hint");
            m_fix_color_prefix = new GUIContent("Use \"Window LCD\"");
            m_useCustomCurve = new GUIContent("Use Custom Curve");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_text_prop, m_text_prop_prefix);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_color_prop, m_color_prop_prefix);
            EditorGUILayout.PropertyField(m_brushColor, m_brushColor_prefix);
            EditorGUILayout.PropertyField(m_TextRenderingHint, m_TextRenderingHint_prefix);
            EditorGUILayout.PropertyField(m_fix_color, m_fix_color_prefix);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_FontSize);
            EditorGUILayout.PropertyField(m_fontSizeUnit);
            EditorGUILayout.PropertyField(m_fontfile);
            EditorGUILayout.Space();
            if (GUILayout.Button("select font from file"))
            {
                string filename = EditorUtility.OpenFilePanelWithFilters("select font file", "C://Windows/Fonts", new[] { "字体文件", "ttf;*.ttc" });
                if (!string.IsNullOrEmpty(filename))
                {
                    if (serializedObject.targetObjects != null)
                    {
                        Action<UnityEngine.Object> handle = null;
                        handle = obj =>
                        {
                            if (!obj)
                                return;
                            var com = obj as PixelPrefectText;
                            if (com)
                            {
                                com.m_fontfile = filename;
                                com.SetAllDirty();
                            }
                        };
                        for (int i = 0; i < serializedObject.targetObjects.Length; i++)
                        {
                            var obj = serializedObject.targetObjects[i];
                            handle(obj);
                        }
                    }
                    //var com = serializedObject.targetObject as PixelPrefectText;
                    //if (com)
                    //    com.m_fontfile = filename;
                    else
                        Debug.LogWarning("no objct to set, file:" + filename);
                }
            }

            EditorGUILayout.PropertyField(m_userCustomCurve, m_useCustomCurve);

            var _obj = serializedObject.targetObject as PixelPrefectText;
            if (_obj && _obj.userCustomCurve)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_alphaMapCurve);
            }


            EditorGUILayout.Space();
            if (GUILayout.Button("SetNativeSize"))
            {
                Action<UnityEngine.Object> handle = null;
                handle = obj =>
                {
                    if (!obj)
                        return;
                    var com = obj as PixelPrefectText;
                    com?.SetNativeSize();
                };
                for (int i = 0; i < serializedObject.targetObjects.Length; i++)
                {
                    var obj = serializedObject.targetObjects[i];
                    handle(obj);
                }
            }

            if (serializedObject.ApplyModifiedProperties() /*|| m_HavePropertiesChanged*/)
            {
                //m_TextComponent.havePropertiesChanged = true;
                //m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }

        }

    }

#endif


}