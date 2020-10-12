using DeviceWebAPI.DB;
using DeviceWebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;


namespace DeviceWebAPI.Controllers
{
    [RoutePrefix("api/DeviceData")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
   
    public class DeviceDataController : ApiController
    {
        Hartama_IOTEntities db = new Hartama_IOTEntities();
       
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

                string json = JsonConvert.SerializeObject(mdl);
                decimal valuedec;
                if (mdl.ElectricaPhase1 != null)
                {
                    for(int x = 0; x < mdl.ElectricaPhase1.Length; x++)
                    {
                        if(Decimal.TryParse(mdl.ElectricaPhase1[x], out valuedec))
                        {
                            var val = Convert.ToDecimal(mdl.ElectricaPhase1[x]);
                            db.InsertDeviceLogNew( val, mdl.MacAddress, "SN001", mdl.ElectricaPhase1[x+1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN002", mdl.ElectricalPhase3[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN003", mdl.GasSensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN005", mdl.VibrationSensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN004", mdl.DistancesSensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN006", mdl.RoomTemperatureSensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN007", mdl.HumiditySensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN008", mdl.TemperatureSensor[x + 1]);
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
                            db.InsertDeviceLogNew(val, mdl.MacAddress, "SN009", mdl.PressureSensor[x + 1]);
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
