using DeviceWebAPI.DB;
using DeviceWebAPI.Models;
using Newtonsoft.Json;
using PusherServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebSocket4Net;

namespace DeviceWebAPI.Controllers
{

    [RoutePrefix("api/DeviceData")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]

   
    public class DeviceDataController : ApiController
    {
        WebSocket4Net.WebSocket _client = new WebSocket4Net.WebSocket("ws://localhost:9096");

        List<pushermodel> mdx;
        Hartama_IOTEntities db = new Hartama_IOTEntities();
        PusherOptions options = new PusherOptions
                   {
                       Cluster = "ap1",
                       Encrypted = true
                   };
        public class modletest
        {
            public string MacAddress { get; set; }
            public string[] ElectricaPhase1 { get; set; }
            public string[] ElectricalPhase3 { get; set; }
            public string[] GasSensor { get; set; }
            public string[] VibrationSensor { get; set; }
            public string[] DistancesSensor { get; set; }
            public string[] RoomTemperatureSensor { get; set; }
            public string[] HumiditySensor { get; set; }
            public string[] TemperatureSensor { get; set; }
            public string[] PressureSensor { get; set; }


        }

        public class pushermodel
        {
            public string DeviceID { get; set; }
            public string SensorID { get; set; }
            public decimal Value { get; set; }
            public DateTime date { get; set; }
            public string SensorName { get; set; }
            public decimal upper { get; set; }
            public decimal lower { get; set; }



        }



        public void WriteCSV(string Mac, decimal value, string units,string mainid)
        {
            // HttpServerUtility server=new HttpServerUtility();
            //before your loop
            // HttpServerUtility server { get; } ;
            string datetime = DateTime.Now.ToShortDateString();
            string replaced = datetime.Replace("/", "-");
            string filename = "Log" + replaced+".csv";
            var csv = new StringBuilder();
            var filePath= System.Web.Hosting.HostingEnvironment.MapPath("~/File/"+filename);

            var DeviceeID = db.TDetailDevices.Where(x => x.MacAdressSensor == Mac).FirstOrDefault();
            var Category = db.PDevices.Where(x => x.Device_ID == DeviceeID.ID_Device).FirstOrDefault();
            var DetailSensor = db.TDetailGroupSensors.Where(x => x.ID_Main_Group == mainid && x.unit.ToLower() == units.ToLower()).FirstOrDefault();
            var SensorParam = db.PSensorParameters.Where(x => x.ID_Category == Category.Device_category_ID && x.ID_Sensor_Detail == DetailSensor.ID_Detail_Group_Sensor).FirstOrDefault();
            var datetime2 = DateTime.Now;


                var newLine = string.Format("{0},{1},{2},{3}", DeviceeID.ID_Device, DetailSensor.ID_Detail_Group_Sensor,value, datetime2);
                csv.AppendLine(newLine);
            
            //in your loop
            

            //after your loop
            File.AppendAllText(filePath, csv.ToString());
            pushermodel mdl = new pushermodel();
            mdl.DeviceID = DeviceeID.ID_Device;
            mdl.SensorID = DetailSensor.ID_Detail_Group_Sensor;
            mdl.Value = Convert.ToDecimal(value);
            mdl.date = datetime2;
            mdl.SensorName = DetailSensor.ID_Main_Group == "SN002" ? DetailSensor.Detail_SensorName+"3": DetailSensor.Detail_SensorName;
            mdl.upper = SensorParam.Upper_Limit;
            mdl.lower = SensorParam.Lower_Limit;
            send(mdl);
            var problem = "";
            if (mdl.Value < mdl.lower)
            {
                problem = "lower";
            }
            else if(mdl.Value>mdl.upper)
            {
                problem = "upper";
            }
            else
            {
                problem = "";
            }
            InsertNotif(mdl.DeviceID,mdl.SensorID,mdl.SensorName,problem,mdl.date,mdl.Value,mdl.lower,mdl.upper,Category.Device_Name);
        }

        public void send(pushermodel mdl)
        {
            //var uri = string.Format("ws://{0}:{1}/", ip, port);
            //List<pushermodel> mm = new List<pushermodel>();
            List<pushermodel>  mds = new List<pushermodel>();
            mds.Add(mdl);
           
           
            _client.NoDelay = true;
            if ( _client.State != WebSocketState.Open)
            {
                if(_client.State != WebSocketState.Connecting)
                {
                    _client.Open();
                }
               
            }
            _client.Opened += (sender2, e2) => websocket_Opened(sender2, e2, mds);

            


            //_client.EnableAutoSendPing = true;
            //_client.AutoSendPingInterval = 3;
            //_client.Send("mantap");

        }
        int counter = 0;

        private void websocket_Opened(object sender, EventArgs e,List<pushermodel> mdl)
        {
            counter = counter + 1;
            string json = JsonConvert.SerializeObject(mdl);
          
            _client.Send(json.ToString());
            //_client.Close();
        }

        public async Task<string> InsertNotif(string deviceid,string sensorid,string sensorname,string problem,DateTime date,decimal valuecurrent, decimal valuelower, decimal valueupper,string devicename)
        {

            //// ModelTest mdl = new ModelTest();
            // List<pushermodel> result2 = new List<pushermodel>();
            // //mdl.Mcadress = "1234";
            // //mdl.DeviceID = "DEV001";
            // //mdl.value = 2;
            // result2.Add(mdl);

            // var pusher = new Pusher(
            //   "1099541",
            //   "4d57b796a305ad74611d",
            //   "1033ace6b885d1937090",
            //   options);

           // var result =  db.InsertNotification(deviceid,sensorid,problem,date,valuecurrent,valueupper,valuelower,sensorname,devicename);
           if (problem != "")
            {
                var result = await Task.Run(() => db.InsertNotification(deviceid, sensorid, problem, date, valuecurrent, valueupper, valuelower, sensorname, devicename).ToList());
            }
           
            return "ok";
        }

        [HttpPost]
        [ActionName("WriteJson")]
        public HttpResponseMessage WriteJson([FromBody]modletest str)
        {
            try
            {
                List<modletest> mdl1 = new List<modletest>();
                modletest mdl = new modletest();
                mdl.MacAddress = str.MacAddress;
                mdl.ElectricaPhase1 = str.ElectricaPhase1;
                mdl.ElectricalPhase3 = str.ElectricalPhase3;
                mdl.GasSensor = str.GasSensor;
                mdl.VibrationSensor = str.VibrationSensor;
                mdl.DistancesSensor = str.DistancesSensor;
                mdl.RoomTemperatureSensor = str.RoomTemperatureSensor;
                mdl.HumiditySensor = str.HumiditySensor;
                mdl.TemperatureSensor = str.TemperatureSensor;
                mdl.PressureSensor = str.PressureSensor;
                //int AffectedRows = 0;
                //var takeAction = db.Set<TSensorTest>();
                //takeAction.Add(new TSensorTest
                //{


                //    mcadrress = mdl.MacAddress,



                //});
                //AffectedRows = db.SaveChanges();
                mdl1.Add(mdl);

                string json = JsonConvert.SerializeObject(mdl);
                decimal valuedec;
                if (mdl.ElectricaPhase1 != null)
                {
                    for (int x = 0; x < mdl.ElectricaPhase1.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.ElectricaPhase1[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.ElectricaPhase1[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN001", mdl.ElectricaPhase1[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.ElectricaPhase1[x + 1], "SN001");
                        }

                    }
                }

                if (mdl.ElectricalPhase3 != null)
                {
                    for (int x = 0; x < mdl.ElectricalPhase3.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.ElectricalPhase3[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.ElectricalPhase3[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN002", mdl.ElectricalPhase3[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.ElectricalPhase3[x + 1], "SN002");
                        }

                    }
                }

                if (mdl.GasSensor != null)
                {
                    for (int x = 0; x < mdl.GasSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.GasSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.GasSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN003", mdl.GasSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.GasSensor[x + 1], "SN003");
                        }

                    }
                }

                if (mdl.VibrationSensor != null)
                {
                    for (int x = 0; x < mdl.VibrationSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.VibrationSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.VibrationSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN005", mdl.VibrationSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.VibrationSensor[x + 1], "SN005");
                        }

                    }
                }

                if (mdl.DistancesSensor != null)
                {
                    for (int x = 0; x < mdl.DistancesSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.DistancesSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.DistancesSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN004", mdl.DistancesSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.DistancesSensor[x + 1], "SN004");
                        }

                    }
                }


                if (mdl.RoomTemperatureSensor != null)
                {
                    for (int x = 0; x < mdl.RoomTemperatureSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.RoomTemperatureSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.RoomTemperatureSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN006", mdl.RoomTemperatureSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.RoomTemperatureSensor[x + 1], "SN006");
                        }

                    }
                }

                if (mdl.HumiditySensor != null)
                {
                    for (int x = 0; x < mdl.HumiditySensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.HumiditySensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.HumiditySensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN007", mdl.HumiditySensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.HumiditySensor[x + 1], "SN007");
                        }

                    }
                }

                if (mdl.TemperatureSensor != null)
                {
                    for (int x = 0; x < mdl.TemperatureSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.TemperatureSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.TemperatureSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN008", mdl.TemperatureSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.TemperatureSensor[x + 1], "SN008");
                        }

                    }
                }

                if (mdl.PressureSensor != null)
                {
                    for (int x = 0; x < mdl.PressureSensor.Length; x++)
                    {
                        if (Decimal.TryParse(mdl.PressureSensor[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.PressureSensor[x]);
                            //db.InsertDeviceLogNew(val, mdl.MacAddress, "SN009", mdl.PressureSensor[x + 1]);
                            WriteCSV(mdl.MacAddress, val, mdl.PressureSensor[x + 1], "SN009");
                        }

                    }
                }

                Hartama_IOTEntities ss = new Hartama_IOTEntities();
                ss.insertlogchange();

                return Request.CreateResponse(HttpStatusCode.OK, new APIActionResultModel
                {
                    Message = "Success",
                    StatusCode = 200

                });

            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new APIActionResultModel
                {
                    Message = e.Message,
                    StatusCode = 500

                });
            }
            

        }
    }
}
