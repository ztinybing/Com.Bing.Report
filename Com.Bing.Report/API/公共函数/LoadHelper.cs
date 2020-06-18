using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Com.Bing.API
{
    public class AssemblyPacket
    {
        public MethodInfo method;
        public Object obj;
        public AssemblyPacket(MethodInfo method, Object obj)
        {
            this.method = method;
            this.obj = obj;
        }
    }

    public static class LoadHelper
    {
        private static Dictionary<string, Assembly> dictAssembly = new Dictionary<string, Assembly>();
        private static Dictionary<string, AssemblyPacket> dictAssemblyPacket = new Dictionary<string, AssemblyPacket>();
        public static object Invoke(string FileName, string Namespace, string ClassName, string ProcName, object[] Parameters)
        {
            FileInfo fi = new FileInfo(FileName);
            string keyAssembly = FileName + fi.LastWriteTime.ToString();
            string keyAssemblyPacket = string.Concat(keyAssembly, ".", Namespace, ".", ClassName, ".", ProcName);
            Assembly assembly = null;
            if (dictAssembly.ContainsKey(keyAssembly))
            {
                assembly = dictAssembly[keyAssembly];
            }
            else
            {
                BufferedStream dataFileStream = new BufferedStream(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                BinaryReader reader = new BinaryReader(dataFileStream);
                byte[] dllFileData = reader.ReadBytes((int)dataFileStream.Length);
                reader.Close();
                assembly = Assembly.Load(dllFileData);
                if (assembly == null)
                    return null;
                dictAssembly[keyAssembly] = assembly;
            }
            if (dictAssemblyPacket.ContainsKey(keyAssemblyPacket))
            {
                return dictAssemblyPacket[keyAssemblyPacket].method.Invoke(dictAssemblyPacket[keyAssemblyPacket].obj, Parameters);
            }
            else
            {
                string fullClassName = Namespace + "." + ClassName;
                Type tp = assembly.GetType(fullClassName);
                if (tp == null)
                    return null;
                MethodInfo method = tp.GetMethod(ProcName);
                if (method == null)
                    return null;
                Object obj = Activator.CreateInstance(tp);
                dictAssemblyPacket[keyAssemblyPacket] = new AssemblyPacket(method, obj);
                return method.Invoke(obj, Parameters);
            }
        }
        public static object InvokeStandard(string FileName, string Namespace, string ClassName, string ProcName, object[] Parameter)
        {
            Assembly MyAssembly = Assembly.LoadFrom(FileName);
            Type[] type = MyAssembly.GetTypes();
            foreach (Type t in type)
            {
                if (t.Namespace == Namespace && t.Name == ClassName)
                {
                    MethodInfo m = t.GetMethod(ProcName);
                    if (m != null)
                    {
                        object o = Activator.CreateInstance(t);
                        return m.Invoke(o, Parameter);
                    }
                }
            }
            return (object)0;
        }
    }
}
