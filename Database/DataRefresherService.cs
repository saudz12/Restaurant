using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public interface IDataRefreshService
    {
        event EventHandler DataChanged;
        void NotifyDataChanged();
    }

    public class DataRefreshService : IDataRefreshService
    {
        public event EventHandler DataChanged;

        public void NotifyDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
