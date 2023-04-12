using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebJobChollometro.Data;
using WebJobChollometro.Models;

namespace WebJobChollometro.Repositories
{
    public class RepositoryChollos
    {
        private ChollometroContext context;
        public RepositoryChollos(ChollometroContext context)
        {
            this.context = context;
        }

        //VAMOS A REALIZAR EL ID AUTOMATICO
        private int GetMaxChollo()
        {
            if (this.context.Chollos.Count() == 0)
            {
                return 1;
            }else
            {
                return this.context.Chollos.Max(x => x.IdChollo) + 1;
            }
        }

        //TENDREMOS UN METODO PARA LEER LOS CHOLLOS DEL RSS
        //ESTE METODO LEERA TODO EL RSS Y DEVOLVERA UNA COLECCION
        //DE CHOLLOS
        private List<Chollo> GetChollosWeb()
        {
            string url = "https://www.chollometro.com/rss";
            //TENEMOS QUE HACER PENSAR A LA WEB DE CHOLLOMETRO QUE ESTAMOS
            //ACCEDIENDO DESDE UN NAVEGADOR WEB
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = @"text/html application/xhtml+xml, *.*";
            request.Referer = "https://www.chollometro.com/";
            request.Headers.Add("Accept-Language", "es-ES");
            request.Host = "www.chollometro.com";
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";

        }
    }
}
