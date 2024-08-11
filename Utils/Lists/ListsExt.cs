using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadDev.Utils.Lists
{
    public static class ListsExt
    {
        public static ObservableCollection<T> ListToObs<T>(this List<T> lists)
        {
            var results = new ObservableCollection<T>();
            foreach (var item in lists) {
                results.Add(item);
            }
            return results;
        }
    }
}
