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

            public override string ToString()
            {
                return "From: " + From + " To: " + To;
            }
        }
        public class TopologyItem
        {
            public Point Position { get; set; }
            public ITopologyItem Item { get; set; }

            public override string ToString()
            {
                return Item.ToString();
            }
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
        /// Devuelve el archivo
        /// </summary>
        public string Save()
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

            return SerializationHelper.SerializeToJson(this, true);
        }
        /// <summary>
        /// Guarda a un archivo
        /// </summary>
        /// <param name="fileName">Archivo</param>
        public void Save(string fileName)
        {
            File.WriteAllText(fileName, Save(), Encoding.UTF8);
        }
        public static void RemplaceVariables(DataInputCollection inputs, Dictionary<string, Variable> vars)
        {
            if (vars == null || inputs == null) return;
            if (vars.Count == 0 || inputs.Count == 0) return;

            foreach (IDataInput item in inputs)
            {

            }
        }
        public static void RemplaceVariables(IEnumerable<ITopologyItem> inputs, Dictionary<string, Variable> vars)
        {
            if (vars == null || inputs == null) return;
            if (vars.Count == 0) return;

            foreach (Variable v in vars.Values)
            {
                foreach (ITopologyItem item in inputs)
                {
                    RemplaceVariables(item.Process, vars);

                    // Replace variables

                }
            }
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
                }

                if (Relations != null)
                {
                    foreach (Relation rel in Relations)
                    {
                        if (rel.From == rel.To) continue;

                        TopologyItem from, to;
                        if (Items.TryGetValue(rel.From, out from) && Items.TryGetValue(rel.To, out to) && from != null && to != null)
                        {
                            if (to.Item is IDataProcess)
                                from.Item.Process.Add((IDataProcess)to.Item);
                        }
                    }
                }

                // Si los remplazo en diseño, ahora mismo se cambiarian en real, tendria que runearse una copia, y no el de edición
                // RemplaceVariables(inputs, Variables);
            }
        }
        /// <summary>
        /// Crea un formato desde un archivo
        /// </summary>
        /// <param name="fileName">Archivo</param>
        public static TLYFile LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            return Load(File.ReadAllText(fileName, Encoding.UTF8));
        }
        public static TLYFile Load(string data)
        {
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