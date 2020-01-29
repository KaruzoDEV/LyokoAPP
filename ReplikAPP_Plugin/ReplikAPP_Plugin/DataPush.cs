using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LyokoAPI.VirtualStructures;
using LyokoAPI.VirtualStructures.Interfaces;

//using System.Threading.Tasks;

namespace ReplikAPP_Plugin
{
    public static class DataPush
    {
        public static void SendData(ITower tower)
        {
            Task.Run(() =>
            {
                string dataKey = ServerKey.GetDataKey();

                var result = "-1";
                var time = DateTime.Now;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                int unixDateTime = (int) (time.ToUniversalTime() - epoch).TotalSeconds;
                var webAddr = ("https://lyokoapp.firebaseio.com/-USERS/" + Main.GetPin() + "/-HISTORY/" + unixDateTime +
                               ".json?auth=" + dataKey);

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(webAddr);
                httpWebRequest.Method = "PATCH";
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var sName = Main.GetUppercaseNames(tower.Sector.Name);
                    string json = "{\"world\": \"" + tower.Sector.World.Name + "\",\"sector\": \"" + sName + "\",\"number\": \"" + 
                                  tower.Number.ToString() + "\",\"activator\": \"" +
                                  Enum.GetName(typeof(APIActivator), tower.Activator) + "\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                using (var streamReader =
                    new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    result = streamReader.ReadToEnd();
                }

                return result;
            });
        }
    }
}