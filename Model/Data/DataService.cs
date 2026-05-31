using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Core;

namespace Model.Data
{
    public abstract class DataService
    {
        public abstract void Save<T>(string path, T data);
        public abstract T Load<T>(string path);
    }
}