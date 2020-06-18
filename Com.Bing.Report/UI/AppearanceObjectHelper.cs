using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Com.Bing.API;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using DevExpress.Utils;

namespace Com.Bing.UI
{
    public class AppearanceObjectHelper
    {
        //�Ƽ۱����屣��·��
        static string fontPath = Function.UserFolder + "�Ƽ۱�����.Font";

        public static void SetFont()
        {
            FontDialog f = new FontDialog();
            Font font = getFont();
            if (font != null)
            {
                f.Font = font;
            }
            if (f.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fontPath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, f.Font);
                stream.Close();
                Function.Alert("���������������Ч", "���óɹ�");
            }
        }

        private static Font getFont()
        {
            Font font = null;
            if (File.Exists(fontPath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fontPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                font = (Font)formatter.Deserialize(stream);
                stream.Close();
            }
            return font;
        }

        public static void LoadFont(AppearanceObject obj)
        {
            Font font = getFont();
            if (font != null)
            {
                obj.Font = font;
                obj.Options.UseFont = true;
            }
        }
    }
}
