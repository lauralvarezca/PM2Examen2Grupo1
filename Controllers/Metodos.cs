
/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-android)'
Antes:
using System;
Después:
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
using System;
*/

/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-ios)'
Antes:
using System;
Después:
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
using System;
*/

/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-windows10.0.19041.0)'
Antes:
using System;
Después:
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
using System;
*/
using
/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-android)'
Antes:
using System.Threading.Tasks;
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
Después:
using System.Threading.Tasks;
*/

/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-ios)'
Antes:
using System.Threading.Tasks;
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
Después:
using System.Threading.Tasks;
*/

/* Cambio no fusionado mediante combinación del proyecto 'PM2Examen2Grupo1 (net7.0-windows10.0.19041.0)'
Antes:
using System.Threading.Tasks;
using Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
Después:
using System.Threading.Tasks;
*/
Newtonsoft.Json;
using PM2Examen2Grupo1.Models;
using System.Text;

namespace PM2Examen2Grupo1.Controllers {
    public class Metodos {
        public async Task<string> insert_update_async(object data,string url) {
            string response_insert = null;

            using(HttpClient client = new HttpClient()) {
                var json = "";

                if(data is Sitios user) {
                    json=JsonConvert.SerializeObject(user);
                }


                if(json!="") {
                    try {
                        var content = new StringContent(json,Encoding.UTF8,"application/json");

                        var response = await client.PostAsync(url,content);

                        if(response.IsSuccessStatusCode) {
                            response_insert="exitoso";
                        } else {
                            response_insert="error";
                        }
                    } catch(Exception ex) {
                        response_insert=ex+"";
                    }

                } else {
                    response_insert="JSON null";
                }
            }

            return response_insert;
        }

        public async Task<string> select_async(object data,string url) {
            string json_response = "";
            string json = "";

            using(HttpClient client = new HttpClient()) {
                try {
                    if(data is Sitios user) {
                        json=JsonConvert.SerializeObject(user);
                    }


                    var json_content = new StringContent(json,Encoding.UTF8,"application/json");
                    var response = await client.PostAsync(url,json_content);

                    if(response.IsSuccessStatusCode) {
                        json_response=await response.Content.ReadAsStringAsync();
                    } else {
                        json_response="error";
                    }
                } catch(Exception ex) {
                    json_response=ex.ToString();
                }
            }

            return json_response;
        }
    }
}
