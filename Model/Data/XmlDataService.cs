using Model.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Data
{
    public class XmlDataService : DataService
    {
        private static readonly string DebugLogPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "debug-46d0bf.log"));

        public override void Save<T>(string path, T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Type type = data.GetType();
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }
                // #region agent log
                AgentLog("H-XmlDish", "XmlDataService.Save", "ok",
                    "{\"runtimeType\":\"" + type.Name + "\",\"path\":\"" + Escape(path) + "\"}");
                // #endregion
            }
            catch (Exception ex)
            {
                // #region agent log
                AgentLog("H-XmlDish", "XmlDataService.Save", "fail",
                    "{\"runtimeType\":\"" + type.Name + "\",\"error\":\"" + Escape(ex.Message) + "\",\"inner\":\"" + Escape(ex.InnerException != null ? ex.InnerException.Message : "") + "\"}");
                // #endregion
                throw;
            }
        }

        // #region agent log
        private static void AgentLog(string hypothesisId, string location, string message, string dataJson)
        {
            try
            {
                long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string line = "{\"sessionId\":\"46d0bf\",\"hypothesisId\":\"" + hypothesisId
                    + "\",\"location\":\"" + location + "\",\"message\":\"" + message
                    + "\",\"data\":" + dataJson + ",\"timestamp\":" + ts + "}\n";
                File.AppendAllText(DebugLogPath, line, Encoding.UTF8);
            }
            catch { }
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
        // #endregion

        public override T Load<T>(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}