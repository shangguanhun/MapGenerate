using System;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class ProvinceNode
{
    public string provinceName;
    public string provinceDescribe;
    public string meshSrc;
    public int offset;
    public ProvinceNode(string provinceName,string provinceDescribe,string meshSrc,int offset)
    {
        this.provinceName = provinceName;
        this.provinceDescribe = provinceDescribe;
        this.meshSrc = meshSrc;
        this.offset = offset;
    }
}

public class XmlLoader
{
    private static string localUrl = "Assets/Resources/ChinaProvince.txt";
    private static List<ProvinceNode> provinceNodeList = new List<ProvinceNode>();

    private static XmlDocument ReadAndLoadXml()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(localUrl);
        return doc;
    }

    public static List<ProvinceNode> GetAllProvinceNode()
    {
        if (provinceNodeList.Count == 0)
        {
            XmlDocument xmlDoc = ReadAndLoadXml();
            XmlNode provinces = xmlDoc.SelectSingleNode("provinces");

            foreach (XmlNode province in provinces.ChildNodes)
            {
                XmlElement _province = (XmlElement)province;
                ProvinceNode provinceNode = new ProvinceNode(_province.GetAttribute("name"), _province.GetAttribute("describe"), _province.GetAttribute("meshSrc"), int.Parse(_province.GetAttribute("offset")));
                provinceNodeList.Add(provinceNode);
            }
        }
        return provinceNodeList;
    }

    public static string GetDescribeByName(string name)
    {
        string describe = "";
        foreach (ProvinceNode node in provinceNodeList)
        {
            if (node.provinceName.Equals(name))
            {
                describe = node.provinceDescribe;
                break;
            }
        }
        return describe;
    }
}