using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Utility
{
    public class PriorityQ<T>
    {
        public List<(double priority, T value)> elements;

        public PriorityQ()
        {
            elements = new List<(double priority, T value)>();
        }

        public void Add(double priority, T edge)
        {
            int index = elements.FindIndex(e => priority > e.priority);
            if (index == -1)
            {
                // Ajouter à la fin si aucune priorité plus petite trouvée
                elements.Add((priority, edge));
            }
            else
            {
                elements.Insert(index, (priority, edge));
            }
        }

        public bool IsEmpty()
        {
            return elements.Count == 0;
        }
        public int Size()
        {
            return elements.Count;
        }
        public T Dequeue()
        {
            var t = elements[0];
            elements.RemoveAt(0);
            return t.value;
        }
    }
}