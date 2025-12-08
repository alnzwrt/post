using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repeat
{
    class MailContainer<T> where T : IMailItem
    {
        private List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
            Console.WriteLine($"\n{item.Name} #{item.Id} додано до контейнеру.");
        }
        public void ProcessAll()
        {
            Console.WriteLine($"\nОбробка всіх відправлень {_items.Count} у контейнері.");
            foreach (var item in _items)
            {
                item.PrintDetails();
            }
        }
    }
}
