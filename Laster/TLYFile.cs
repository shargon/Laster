using Laster.Core.Classes;
using Laster.Core.Classes.Collections;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Laster
{
    public class TLYFile
    {
        public class Relation
        {
            public int From { get; set; }
            public int To { get; set; }
        }
        public class TopologyItem
        {
            public Point Position { get; set; }
            public ITopologyItem Item { get; set; }
        }

        /// <summary>
        /// Items
        /// </summary>
        public Dictionary<int, TopologyItem> Items { get; set; }
        /// <summary>
        /// Relaciones
        /// </summary>
        public List<Relation> Relations { get; set; }
        /// <summary>
        /// Variables
        /// </summary>
        public Dictionary<string, Variable> Variables { get; set; }
        /// <summary>
        /// Ensamblados
        /// </summary>
        public List<string> Assemblies { get; set; }

        public TLYFile()
        {
            Items = new Dictionary<int, TopologyItem>();
            Relations = new List<TLYFile.Relation>();
        }

        /// <summary>
        /// Guarda a un archivo
        /// </summary>
        /// <param name="fileName">Archivo</param>
        public void Save(string fileName)
        {
            Assemblies = new List<string>();
            if (Items != null)
            {
                // Sacar los ensamblados utilizados, por si no están cargados en el momento de la ejecución
                foreach (TopologyItem i in Items.Values)
                {
                    if (i == null || i.Item == null) continue;

                    Assembly asm = i.Item.GetType().Assembly;
                    string path = asm.Location;

                    if (string.IsNullOrEmpty(path)) continue;

                    // Relativar la ruta
                    string relPath = PathHelper.MakeRelative(Path.GetDirectoryName(path), Application.StartupPath);
                    if (!string.IsNullOrEmpty(relPath)) relPath += Path.DirectorySeparatorChar;

                    relPath += Path.GetFileName(path);
                    if (Assemblies.Contains(relPath)) continue;
                    Assemblies.Add(relPath);
                }
            }

            string data = SerializationHelper.Serialize2Json(this, true);
            File.WriteAllText(fileName, data, Encoding.UTF8);
        }
        /// <summary>
        /// Compila el archivo
        /// </summary>
        /// <param name="inputs">Colección de entradas</param>
        public void Compile(DataInputCollection inputs)
        {
            // Cargar topología
            if (Items != null)
            {
                foreach (TopologyItem item in Items.Values)
                {
                    if (item.Item is IDataInput)
                        inputs.Add((IDataInput)item.Item);

                    if (Variables != null)
                    {
                        /*
                        foreach (Variable v in Variables.Values)
                        {

                        }
                        */
                    }
                }

                if (Relations != null)
                {
                    foreach (Relation rel in Relations)
                    {
                        if (rel.From == rel.To) continue;

                        TopologyItem from, to;
                        if (Items.TryGetValue(rel.From, out from) && Items.TryGetValue(rel.To, out to) && from != null && to != null)
                        {
                            if (from.Item is ITopologyReltem)
                            {
                                ITopologyReltem rfrom = (ITopologyReltem)from.Item;

                                if (to.Item is IDataProcess) rfrom.Process.Add((IDataProcess)to.Item);
                                else if (to.Item is IDataOutput) rfrom.Out.Add((IDataOutput)to.Item);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Crea un formato desde un archivo
        /// </summary>
        /// <param name="fileName">Archivo</param>
        public static TLYFile Load(string fileName)
        {
            string data = File.ReadAllText(fileName, Encoding.UTF8);
            if (!string.IsNullOrEmpty(data))
            {
                // Cargar los ensamblados que no están cargados ya
                dynamic ob = SerializationHelper.DeserializeFromJson<object>(data, false);

                foreach (string asm in ob.Assemblies)
                {
                    string path = Application.StartupPath + Path.DirectorySeparatorChar + asm;

                    bool esta = false;
                    foreach (Assembly lasm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (lasm.IsDynamic) continue;
                        if (lasm.Location == path) { esta = true; break; }
                    }

                    if (esta) continue;

                    Assembly.LoadFile(path);
                }

                // Deserializar con todo cargado previamente
                return SerializationHelper.DeserializeFromJson<TLYFile>(data, true);
            }
            return null;
        }
    }
}