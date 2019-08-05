using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace UtilKits.Serializer
{
    public class XmlHelper : ISerializer
    {
        private static Lazy<XmlHelper> _xmlSerializer = new Lazy<XmlHelper>(() => new XmlHelper());

        /// <summary>
        /// 取得處理Xml序列化的執行個體
        /// </summary>
        public static XmlHelper Instance => _xmlSerializer.Value;

        /// <summary>
        /// 將物件轉換為Xml字串
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>Xml 字串</returns>
        public string Serialize<T>(T obj)
        {
            string result = String.Empty;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result;
        }

        /// <summary>
        /// 將物件轉換為Xml字串，不產生Xml Version Namespace
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="obj">要轉換的物件</param>
        /// <param name="namespaceUrl">Namespace Url</param>
        /// <returns>Xml 字串</returns>
        public string Serialize<T>(T obj, string namespaceUrl)
        {
            string result = String.Empty;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            if (string.IsNullOrEmpty(namespaceUrl))
            {
                ns.Add("", "");
            }
            else
            {
                ns.Add("ns", namespaceUrl);
            }

            using (var ms = new MemoryStream())
            using (var writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, obj, ns);
                result = Encoding.UTF8.GetString(ms.ToArray());
            }

            return result.Replace("xmlns:ns", "xmlns");
        }

        /// <summary>
        /// 將物件轉換為XDcouemnt元件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>XDcouemnt元件</returns>
        public XDocument SerializeToXDocument<T>(T obj)
        {
            XDocument result = new XDocument();
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            using (var writer = result.CreateWriter())
            {
                serializer.WriteObject(writer, obj);
            }

            return result;
        }

        /// <summary>
        /// 將Xml字串轉換為泛型物件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="xml">Xml 字串</param>
        /// <param name="isIgnoreNamespace">true: 省略xml的namespace屬性</param>
        /// <returns>
        /// 泛型物件
        /// </returns>
        public T Deserialize<T>(string xml)
        {
            T obj = Activator.CreateInstance<T>();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                obj = (T)serializer.Deserialize(ms);
            }

            return obj;
        }

        /// <summary>
        /// 將Xml字串轉換為泛型物件並省略xml的namespace屬性
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="xml">Xml 字串</param>
        /// <returns>
        /// 泛型物件
        /// </returns>
        public T DeserializeIgnoreNamespace<T>(string xml)
        {
            T obj = Activator.CreateInstance<T>();
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            obj = (T)serializer.Deserialize(new NamespaceIgnorant(new StringReader(xml)));

            return obj;
        }

        /// <summary>
        /// 將XDocument轉換為泛型物件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="doc">XDocument物件</param>
        /// <returns>泛型物件</returns>
        public T Deserialize<T>(XDocument doc)
        {
            T obj = Activator.CreateInstance<T>();
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var reader = doc.Root.CreateReader())
            {
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }
        /// <summary>
        /// 將IEnumerable<XNode>轉換為泛型物件清單
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="nodes">IEnumerable<XNode>物件</param>
        /// <returns>泛型物件清單</returns>
        public List<T> DeserializeToList<T>(IEnumerable<XNode> nodes)
        {
            List<T> result = Activator.CreateInstance<List<T>>();
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            foreach (var node in nodes)
            {
                using (var reader = node.CreateReader())
                {
                    result.Add((T)serializer.Deserialize(reader));
                }
            }

            return result;
        }

        /// <summary>
        /// 省略xml格式中的namespace
        /// </summary>
        /// <seealso cref="System.Xml.XmlTextReader" />
        internal class NamespaceIgnorant : XmlTextReader
        {
            public NamespaceIgnorant(TextReader reader) : base(reader) { }

            public override string NamespaceURI
            {
                get { return string.Empty; }
            }
        }
    }
}
