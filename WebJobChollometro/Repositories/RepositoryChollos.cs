﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        private async Task<List<Chollo>> GetChollosWebAsync()
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
            HttpWebResponse response =
                (HttpWebResponse)request.GetResponse();
            //ESTA PETICION NOS OFRECE UN STREAM, QUE DEBEMOS CONVERTIRLO
            //A STRING
            string xmlData = "";
            using (StreamReader reader =
                new StreamReader(response.GetResponseStream()))
            {
                xmlData = await reader.ReadToEndAsync();
            }
            XDocument document = XDocument.Parse(xmlData);
            var consulta = from datos in document.Descendants("item")
                           select datos;
            List<Chollo> chollosList = new List<Chollo>();
            int idchollo = this.GetMaxChollo();
            foreach (XElement tag in consulta)
            {
                Chollo chollo = new Chollo();
                chollo.IdChollo = idchollo;
                chollo.Titulo = tag.Element("title").Value;
                chollo.Descripcion = tag.Element("description").Value;
                chollo.Link = tag.Element("link").Value;
                chollo.Fecha = DateTime.Now;
                idchollo += 1;
                chollosList.Add(chollo);
            }
            return chollosList;
        }

        //METODO PARA INSERTAR DATOS EN SQL AZURE
        public async Task PopulateChollosAsync()
        {
            //RECUPERAMOS LOS CHOLLOS DE RSS
            List<Chollo> chollos = await this.GetChollosWebAsync();
            //RECORREMOS TODOS LOS CHOLLOS Y LOS ALMACENAMOS EN CONTEXT
            foreach (Chollo ch in chollos)
            {
                this.context.Chollos.Add(ch);
            }
            await this.context.SaveChangesAsync();
        }
    }
}
