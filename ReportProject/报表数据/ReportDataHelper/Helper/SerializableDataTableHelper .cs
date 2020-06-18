using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace Com.Bing
{
    public class SerializableDataTableHelper : IXmlSerializable
    {
        DataTable dataTable = null;
        public SerializableDataTableHelper(DataTable dataTable)
        {
            this.dataTable = dataTable;
        }
        public DataTable ResultTable
        {
            get
            {
                return dataTable;
            }
        }
        public void WriteXml(XmlWriter write)
        {
            //表
            write.WriteStartElement("t");
            //行
            foreach (DataRow dr in dataTable.Rows)
            {
                write.WriteStartElement("r");
                //列
                foreach (DataColumn dc in dataTable.Columns)
                {
                    write.WriteStartElement("c");
                    write.WriteString(dr[dc].ToString());
                    write.WriteFullEndElement();
                }
                write.WriteFullEndElement();
            }
            write.WriteFullEndElement();
        }

        public void ReadXml(XmlReader reader)
        {
            dataTable.Rows.Clear();
            try//旧版数据解析
            {
                //表
                reader.ReadStartElement("t");
                //行
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("r");
                    DataRow dr = dataTable.NewRow();
                    dataTable.Rows.Add(dr);
                    int i = 0;
                    string value = "";
                    //列
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.ReadStartElement("c");
                        //dr[i++] = reader.ReadString();
                        value = reader.ReadString();
                        if (i < dataTable.Columns.Count)
                        {
                            dr[i++] = value;
                        }
                        reader.ReadEndElement();
                    }
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
            catch
            {
                //新版数据解析
                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable xmlTable = ds.Tables[0];
                        List<string> oldColumns = new List<string>();
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            oldColumns.Add(col.Caption);
                        }
                        dataTable.Columns.Clear();
                        foreach (DataColumn col in xmlTable.Columns)
                        {
                            dataTable.Columns.Add(col.Caption, col.DataType);
                        }
                        foreach (DataRow row in xmlTable.Rows)
                        {
                            DataRow drNew = dataTable.NewRow();
                            drNew.ItemArray = (object[])row.ItemArray.Clone();
                            dataTable.Rows.Add(drNew);
                        }
                        if (oldColumns.Contains("feeName") && dataTable.Columns.Contains("standardName") && !dataTable.Columns.Contains("feeName"))
                        {
                            dataTable.Columns.Add("feeName", dataTable.Columns["standardName"].DataType);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["feeName"] = row["standardName"];
                            }
                        }
                    }
                }
                catch
                {
                    throw new ArgumentException("解析数据失败!");
                }
            }
        }

        public string XML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                XmlTextWriter w = new XmlTextWriter(new StringWriter(sb));
                this.WriteXml(w);
                return sb.ToString();
            }
            set
            {
                XmlTextReader r = new XmlTextReader(new StringReader(value));
                this.ReadXml(r);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
    }

}
