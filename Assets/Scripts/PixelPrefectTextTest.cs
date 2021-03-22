using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Drawing;
using UnityEngine;
using System.Drawing.Text;
using System.IO;
using Image = UnityEngine.UI.RawImage;
using Color = System.Drawing.Color;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class PixelPrefectTextTest : UIBehaviour
{
    [TextArea(3, 10)] [SerializeField] protected string m_Text = String.Empty;
    [SerializeField] RawImage m_Img = null;
    RawImage Img
    {
        get
        {
            print("Enter");
            if (!m_Img)
                m_Img = GetComponent<RawImage>();
            return m_Img;
        }
    }

    [SerializeField] BrushesColor brushColor = BrushesColor.White;
    [SerializeField] TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

    private Vector2 m_MesuredSize;
    static System.Drawing.Font defaultFont;


    static PixelPrefectTextTest()
    {
        PrivateFontCollection pcol = new PrivateFontCollection();
        pcol.AddFontFile(@"C:\Windows\Fonts\msyh.ttc");
        var fml = pcol.Families[0];
        defaultFont = new System.Drawing.Font(fml, 14);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        m_UpdateDrawingTexture();
    }
#endif

    private void m_UpdateDrawingTexture()
    {
        if (string.IsNullOrEmpty(m_Text))
            return;
        System.Drawing.Bitmap bmp = new Bitmap(1, 1);
        using (var graphic = System.Drawing.Graphics.FromImage(bmp))
        {
            var size = graphic.MeasureString(m_Text, defaultFont);
            this.m_MesuredSize = new Vector2(size.Width, size.Height);
        }
        bmp = new Bitmap((int)m_MesuredSize.x, (int)m_MesuredSize.y);

        string colorstr = brushColor.ToString();
        var syscolor = Color.FromName(colorstr);
        var brush = new SolidBrush(syscolor);

        using (var graphic = System.Drawing.Graphics.FromImage(bmp))
        {
            graphic.TextRenderingHint = TextRenderingHint;
            graphic.DrawString(m_Text, defaultFont, brush, PointF.Empty);
            graphic.Flush();
        }
        var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var pngdat = ms.ToArray();

        var t = new Texture2D(1, 1);
        t.LoadImage(pngdat);
        Img.texture = t;
        Img.SetNativeSize();
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
}
