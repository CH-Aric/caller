using System;

using System;
using System.Net;
using Independentsoft.Sip;
using Independentsoft.Sip.Sdp;

namespace Sample
{
    class Program
    {
        private static Logger logger;
        private static SipClient client;

        public static void Main(string[] args)
        {
            client = new SipClient("coolheatcrm.duckdns.org", "800", "0uts1de");

            //create logger
            logger = new Logger();
            logger.WriteLog += new WriteLogEventHandler(OnWriteLog);
            client.Logger = logger;

            client.ReceiveRequest += new ReceiveRequestEventHandler(OnReceiveRequest);
            client.ReceiveResponse += new ReceiveResponseEventHandler(OnReceiveResponse);

            //IPHostEntry localhost = Dns.Resolve("mycomputer"); 
            //client.LocalIPEndPoint = new IPEndPoint(localhost.AddressList[0],5060);

            System.Net.IPAddress localAddress = System.Net.IPAddress.Parse("192.168.2.29");
            client.LocalIPEndPoint = new System.Net.IPEndPoint(localAddress, 5060);

            client.Connect();

            client.Register("sip:coolheatcrm.duckdns.org", "sip:800@coolheatcrm.duckdns.org", "sip:800@" + client.LocalIPEndPoint.ToString());

            SessionDescription session = new SessionDescription();
            session.Version = 0;

            Owner owner = new Owner();
            owner.Username = "Bob";
            owner.SessionID = 16264;
            owner.Version = 18299;
            owner.Address = "192.168.2.29";

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = "192.168.2.29";

            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 25282;
            media1.TransportProtocol = "RTP/AVP";
            media1.MediaFormats.Add("0");
            media1.MediaFormats.Add("101");

            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            media1.Attributes.Add("fmtp", "101 0-11");

            session.Media.Add(media1);

            RequestResponse inviteRequestResponse = client.Invite("sip:800@coolheatcrm.duckdns.org", "sip:503@coolheatcrm.duckdns.org", "sip:800@" + client.LocalIPEndPoint.ToString(), session);
            client.Ack(inviteRequestResponse);

            Console.WriteLine("Press ENTER to exit.");
            Console.Read();
            client.Disconnect();
        }

        private static void OnReceiveRequest(object sender, RequestEventArgs e)
        {
            //accept any request from server or another sip user agent
            client.AcceptRequest(e.Request);
        }

        private static void OnReceiveResponse(object sender, ResponseEventArgs e)
        {
        }

        private static void OnWriteLog(object sender, WriteLogEventArgs e)
        {
            Console.Write(e.Log);
        }
    }
}