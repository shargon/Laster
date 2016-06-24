using Laster.Core.Designer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace Laster.Core.Classes.Collections
{
    public class DataVariableCollection : Dictionary<string, Variable>
    {
        public class DesignClassLink
        {
            public DataVariableCollection Parent;
            public Dictionary<string, Variable> Variables;

            public void ApplyToParent()
            {
                if (Parent == null) return;

                Parent.Clear();
                foreach (string v in Variables.Keys) Parent.Add(v, Variables[v]);
            }
            public override string ToString() { return Variables.Count.ToString(); }
        }
        public class DesignClass
        {
            [Editor(typeof(DataVariableCollectionEditor), typeof(UITypeEditor))]
            public DesignClassLink Variables { get; set; }

            //public VariableEdit() {  }
            public DesignClass(DataVariableCollection dataVariableCollection)
            {
                Variables = new DesignClassLink()
                {
                    Parent = dataVariableCollection,
                    Variables = new Dictionary<string, Variable>()
                };
            }
        }

        public DesignClass Designer
        {
            get { return new DesignClass(this); }
            set
            {
                Clear();

                if (value != null && value.Variables != null)
                    value.Variables.ApplyToParent();
            }
        }

        public void Add(ValueCollection values) { foreach (Variable v in values) Add(v); }
        public void Add(Variable v) { Add(v.Name, v); }
    }
}