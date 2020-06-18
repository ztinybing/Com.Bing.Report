using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Com.Bing.API;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace Com.Bing.Report
{
    public static class ReportHelper
    {
        #region  用户自定义报表XML
        public static string GenerateXml(string folderPath, string xmlPath)
        {
            if (!Directory.Exists(folderPath)) return xmlPath;
            DirectoryInfo di = new DirectoryInfo(folderPath);
            XmlDocument xDoc = new XmlDocument();

            AppendDeclare(xDoc);

            XmlElement xFolder = AddFolder(xDoc, di.Name);

            AddSubDirectoriesAndFiles(di, xFolder);

            xDoc.Save(xmlPath);
            return xmlPath;
        }

        private static void AppendDeclare(XmlDocument xDoc)
        {
            XmlDeclaration xDeclare = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xDoc.AppendChild(xDeclare);
        }

        private static void AddSubDirectoriesAndFiles(DirectoryInfo di, XmlElement xParent)
        {
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                XmlElement xFolder = AddFolder(xParent, subDi.Name);
                AddSubDirectoriesAndFiles(subDi, xFolder);
            }
            foreach (FileInfo subFi in di.GetFiles("*.rpt"))
            {
                if (Path.GetExtension(subFi.Name) == ".rpt")
                {
                    string reportName = TrimSuffix(subFi.Name, ".rpt").TrimEnd('_');
                    int dashIndex = reportName.LastIndexOf('_');
                    //有下划线且不在最后一位
                    if (dashIndex >= 0)
                    {
                        reportName = reportName.Substring(dashIndex + 1);
                    }
                    AddFile(xParent, reportName, subFi.FullName);
                }
            }
        }
        private static XmlElement AddFolder(XmlDocument xDoc, string name)
        {
            XmlElement xElement = xDoc.CreateElement("ReportRoot");
            xElement.SetAttribute("id", "dir");
            xElement.SetAttribute("name", name);
            xElement.SetAttribute("src", "");
            xDoc.AppendChild(xElement);

            return xElement;
        }

        private static XmlElement AddFolder(XmlNode xParentElement, string name)
        {
            XmlElement xElement = xParentElement.OwnerDocument.CreateElement("ReportRoot");
            xElement.SetAttribute("id", "dir");
            xElement.SetAttribute("name", name);
            xElement.SetAttribute("src", "");
            xParentElement.AppendChild(xElement);

            return xElement;
        }
        private static XmlElement AddFile(XmlNode xParentElement, string name, string path)
        {
            XmlElement xElement = xParentElement.OwnerDocument.CreateElement("Report");
            xElement.SetAttribute("id", "file");
            xElement.SetAttribute("name", name);
            xElement.SetAttribute("src", path.Substring(path.IndexOf("用户自定义")));
            xParentElement.AppendChild(xElement);

            return xElement;
        }

        private static string TrimSuffix(string source, string suffix)
        {
            if (source.Length >= suffix.Length &&
                source.Substring(source.Length - suffix.Length, suffix.Length) == suffix)
            {
                return source.Substring(0, source.Length - suffix.Length);
            }
            else
            {
                return source;
            }
        }
        #endregion

        #region WORD
        /// <summary>
        /// 打开word并替换其中的宏变量
        /// </summary>
        /// <param name="filePath">word路径</param>
        public static void OpenWord(string filePath)
        {
            Word.ApplicationClass app = new Word.ApplicationClass();
            Word.Document doc = null;
            object missing = System.Reflection.Missing.Value;
            object path = (object)filePath;
            try
            {
                doc = app.Documents.Open(ref path, ref missing, ref missing,
             ref missing, ref missing, ref missing, ref missing, ref missing,
             ref missing, ref missing, ref missing, ref missing, ref missing,
             ref missing, ref missing, ref missing);
                app.Visible = true;
            }
            catch
            {
                Function.Alert("打开WORD失败", "提示");
            }
        }

        private static bool Replace(Word.Document oDoc, MacrosVerbCollection macrosCollection)
        {
            Word.Comment comment;
            Word.Range range;
            for (int i = oDoc.Comments.Count; i > 0; i--)
            {
                comment = oDoc.Comments[i];
                KeyValuePair<string, int> key = new KeyValuePair<string, int>(comment.Range.Text, 1);
                if (macrosCollection.ContainsKey(key))
                {
                    range = comment.Scope;
                    comment.Delete();
                    range.Text = macrosCollection[key];
                    object name = (object)key.Key;
                    oDoc.Comments.Add(range, ref name);
                }
            }
            Word.View view = oDoc.Application.ActiveWindow.View;
            view.ShowComments = false;

            return true;
        }


        /// <summary>
        /// 打印word
        /// </summary>
        public static void PrintWord(string filePath, MacrosVerbCollection macrosCollection)
        {
            Word.Application app = new Word.Application();

            Word.Document doc = null;
            object collate = true;
            object item = Word.WdPrintOutItem.wdPrintDocumentContent;
            object missing = System.Reflection.Missing.Value;
            try
            {
                object path = filePath;
                doc = app.Documents.Add(ref path, ref missing, ref missing, ref missing);
                Replace(doc, macrosCollection);

                app = new Microsoft.Office.Interop.Word.ApplicationClass();
                doc.PrintOut(ref missing, ref missing, ref missing, ref missing,
                     ref missing, ref missing, ref item, ref missing, ref missing,
                     ref missing, ref missing, ref missing, ref missing, ref missing,
                     ref missing, ref missing, ref missing, ref missing);
            }
            catch (Exception exp)
            {
                Function.Alert(exp.Message, "提示");
            }
            finally
            {
                object saveChange = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                Word._Document _doc = doc as Word._Document;
                if (_doc != null)
                    _doc.Close(ref saveChange, ref missing, ref missing);
                Word._Application _app = app as Word._Application;
                if (_app != null)
                    _app.Quit(ref missing, ref missing, ref missing);
            }
        }
        #endregion
    }
}
