using System.Collections.Generic;

namespace UIShell.Extension
{
    public abstract class BuilderBase<T> : AbstractBuilder<T>
    {
        public BuilderBase()
        {
            Items = new List<T>();
        }

        public List<T> Items { get; private set; }

        public void AddItem(T item)
        {
            Items.Add(item);
            OnItemAdded(item);
        }

        public override void Reset()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                this.OnItemRemoved(Items[i]);
            }
            Items.Clear();
        }
    }
}