using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneButtonLauncher
{
    public class ProcessEventHandler : IMessageFilter
    {
        public uint EventId { get; private set; }

        public event EventHandler<uint> OnEventTriggered;

        public ProcessEventHandler(uint eventId)
        {
            this.EventId = eventId;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if(m.Msg == this.EventId)
            {
                OnEventTriggered?.Invoke(this, this.EventId);
                return true;
            }
            return false;
        }
    }
}
