using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamla.Types.Records
{
    public sealed class MultiChoice
        : List<int>
    {
        Schema schema;
        ChoiceSet choiceSet;

        public MultiChoice(Schema schema)
        {
            if (schema == null)
                throw new ArgumentNullException();

            if (schema.DataType != DataType.MultiChoice)
                throw new ArgumentException();

            this.schema = schema;
        }

        public ChoiceSet ChoiceSet
        {
            get
            {
                if (choiceSet == null)
                    choiceSet = ChoiceSet.FromCursor(schema.DeclarationItem);

                return choiceSet;
            }
        }

        public void Add(string name)
        {
            if (!Contains(name))
                this.Add(this.ChoiceSet.Options.First(x => x.Name == name).Value);
        }

        public void Remove(string name)
        {
            this.Remove(this.ChoiceSet.Options.First(x => x.Name == name).Value);
        }

        public bool Contains(string name)
        {
            return this.ActiveOptions.Any(x => x.Name == name);
        }

        public void Assign(IEnumerable<int> values)
        {
            this.Clear();
            this.AddRange(values);
        }

        public IEnumerable<ChoiceOption> ActiveOptions
        {
            get { return this.SelectMany(x => this.ChoiceSet.Options.Where(y => y.Value == x).Take(1)); }
        }

        public Schema Schema
        {
            get { return schema; }
        }

        public IEditableList ToEditable(IEditableFactory factory)
        {
            var list = (IEditableList)factory.Create(schema);
            foreach (var x in this)
            {
                list.Add().Set(x);
            }
            return list;
        }

        public override string ToString()
        {
            return string.Join(", ", this.ActiveOptions.Select(x => x.Caption));
        }

        public MultiChoice Clone()
        {
            var clone = new MultiChoice(schema);
            clone.AddRange(this);
            return clone;
        }
    }
}
