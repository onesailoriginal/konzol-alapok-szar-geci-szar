using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows;

namespace ConsoleApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            backendConnect bConnect = new backendConnect();
            Console.WriteLine("Mit szeretnél csinálni?[vásárolni, nézelődni, törölni]");
            string res = Console.ReadLine();

            if(res.Contains("vásárolni")){
                Console.WriteLine("Kolbászod neve");
                string kolbaszNeve = Console.ReadLine();

                Console.WriteLine("Kolbászod Értékelése");
                string asdasd = Console.ReadLine();
                float kolbaszErtekelese = float.Parse(asdasd);

                Console.WriteLine("Kolbász ára");
                int kolbaszAra = Convert.ToInt32(Console.ReadLine());

                bool kolbaszConnect = await bConnect.CreateKolbasz(kolbaszNeve, kolbaszErtekelese, kolbaszAra);
                if (kolbaszConnect)
                {
                    Console.WriteLine("Sikeresen megvetted a(z) kolbászt");
                }
                else
                {
                    Console.WriteLine("Sikertelenül vásároltad meg.");
                }
            }

            else if (res.Contains("nézelődni"))
            {
                List<dataTypes> kolbaszList = await bConnect.Kolbaszok();


                foreach (dataTypes item in kolbaszList)
                {
                    Console.WriteLine("Kolbászod Neve:"+item.kolbaszNeve+" ("+item.id+")"+ "Értékelése"+ item.kolbaszErtekelese+" Ára:"+item.kolbaszAra);
                }
                Console.WriteLine("Összes kolbászod kész.");

            }
            else if (res.Contains("törölni"))
            {
                Console.WriteLine("írdd be melyik kolbászt szeretnéd törölni");
                string asd = Console.ReadLine();
                int kolbaszId = Convert.ToInt32(asd);

                bool kolbaszConnect = await bConnect.DeleteKolbasz(kolbaszId);
                if (kolbaszConnect)
                {
                    Console.WriteLine("sikeres törlés");
                }
                else
                {
                    Console.WriteLine("szar");
                }
            }
            Console.ReadKey();
        }
    }


    public class backendConnect
    {
        HttpClient client = new HttpClient();
        string url = "";
        public backendConnect()
        {
            this.url = "http://127.0.0.1:3000";
        }

        public async Task<List<dataTypes>> Kolbaszok()
        {
            List<dataTypes> all = new List<dataTypes>();
            string surl = url + "/kolbaszok";
            try
            {
                HttpResponseMessage response = await client.GetAsync(surl);
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response to match the structure
                var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                if (responseData.ContainsKey("kolbasz"))
                {
                    var kolbaszList = JsonConvert.DeserializeObject<List<dataTypes>>(responseData["kolbasz"].ToString());
                    all = kolbaszList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return all;
        }

        public async Task<bool> CreateKolbasz(string kolbaszNeve, float kolbaszErtekelese, int kolbaszAra)
        {
            try
            {
                string sUrl = "http://127.0.0.1:3000/createKolbasz";
                var jsonInfo = new
                {
                    kolbaszNeve = kolbaszNeve,
                    kolbaszErtkelese = kolbaszErtekelese,
                    kolbaszAra = kolbaszAra
                };
                string jsonS = JsonConvert.SerializeObject(jsonInfo);
                HttpContent s = new StringContent(jsonS, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync(sUrl, s);
                res.EnsureSuccessStatusCode();
                string result = await res.Content.ReadAsStringAsync();
                dataTypes data = JsonConvert.DeserializeObject<dataTypes>(result);
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public async Task<bool> DeleteKolbasz(int id)
        {
            try
            {
                string sUrl = url + "/deleteKolbasz/" + id;
                HttpResponseMessage res = await client.DeleteAsync(sUrl);
                res.EnsureSuccessStatusCode();

                string result = await res.Content.ReadAsStringAsync();
                dataTypes data = JsonConvert.DeserializeObject<dataTypes>(result);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }


        public class dataTypes
    {
        public int id { get; set; }
        public string kolbaszNeve { get; set; }
        public float kolbaszErtekelese { get; set; }
        public int kolbaszAra { get; set; }
    }


}
