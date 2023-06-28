using System;
namespace BroadcastMessageServiceWorker.Services
{
	public interface IBroadcastDispatchService
	{
		void SendMessage();
	}

    public class BroadcastDispatchService : IBroadcastDispatchService
    {
        public void SendMessage()
        {
            // get paginated list of broacast messages on pending by order of FIFO using the createdTime

             // iterate through the list and process message sending as below
             {


                // format the message value of the broadcast message entity

                // update the broadcast message status as processing once send

                // call the httpSendMessage service to send text based message;
                // using the business settings to get the apikey.

                // if response ok message is received from httpMessageSend service

                // update the broadcast message to sent

            }


        }
    }
}

