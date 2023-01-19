using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class _360MessageDto
    {
        public List<MessageMsg> messages { get; set; }
        public List<MessageContact> contacts { get; set; }
        public MessageInteractive interactive { get; set; }
    }
    public class MessageInteractive
    {
        public string type { get; set; }
    }

    public class MsgText
    {
        public string body { get; set; }
    }

    public class MessageMsg
    {
        public string from { get; set; }
        public string id { get; set; }
        public MsgText text { get; set; }
        public ResponseInteractive interactive { get; set; }
        public Context context { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
    }

    public class ProfileDto
    {
        public string name { get; set; }
    }

    public class MessageContact
    {
        public ProfileDto profile { get; set; }
        public string wa_id { get; set; }
    }

    public class Context
    {
        public string from { get; set; }
        public string group_id { get; set; }
        public string id { get; set; }
        public List<string> mentions { get; set; }
    }

    public class ResponseListReply
    {
        public string description { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }

    public class ResponseInteractive
    {
        public ResponseListReply list_reply { get; set; }
        public string type { get; set; }
    }
}
