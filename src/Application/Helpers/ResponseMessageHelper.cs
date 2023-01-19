using System;
using System.Collections.Generic;
using Application.DTOs.InteractiveMesageDto;
using Application.DTOs.InteractiveMesageDto.InboundMessageDto;
using Application.DTOs.OutboundMessageRequests;
using Domain.Entities.DialogMessageEntitties;

namespace Application.Helpers
{
    public static class RequestHelper
    {
        
        /// <summary>
        /// This is a method to serialize business message to whatsapp model shape.
        /// The Generic T is required just for decorator reasons since this class would be used for multiple ToModel conversions
        /// </summary>
        /// <param name="model"></param>
        /// <param name="recepient"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ReplyButtonMessageRequest ToModel<T>(BusinessMessage model, string recepient) where T: ReplyButtonMessageRequest
        {
            // var buttons = new List<ButtonDto>();
            // if (model?.ReplyButtonMessage?.Buttons != null)
            //     foreach (var button in model?.ReplyButtonMessage?.Buttons)
            //     {
            //         buttons.Add(new ButtonDto()
            //         {
            //             Type = model.MessageType,
            //             Reply = new ReplyDto()
            //             {
            //                 Id = button.Id.ToString(),
            //                 Title = button.Title
            //             }
            //         });
            //     }
            //
           // var response = new ReplyButtonMessageRequest();
            // {
            //     To = recepient,
            //     Recipient_type = model?.RecipientType,
            //     Type = "interactive",
            //     Interactive = new ReplyButtonInteractiveDto()
            //     {
            //         Footer = new Footer()
            //         {
            //             Text = model?.ReplyButtonMessage?.Footer
            //         },
            //         Header = new Header()
            //         {
            //             Text = model?.ReplyButtonMessage?.Header
            //         },
            //         Body = new Body()
            //         {
            //             Text = model?.ReplyButtonMessage?.Body
            //         },
            //         Type = model?.MessageType,
            //         Action = new ActionDto()
            //         {
            //             Buttons = buttons
            //         }   
            //     }
            // };

            throw new NotImplementedException();
        }

        //public static ListInteractiveMessageRequest ToListMessageModel<T>(BusinessMessage model, string recepient) where T : ListInteractiveMessageRequest
        //{
            // var sections = new List<ListSection>();
            // var rows = new List<ListRowDto>();
            // if (model?.ListMessage?.Rows != null)
            //     foreach (var list in model?.ListMessage?.Rows)
            //     {
            //         sections.Add(new ListSection()
            //         {
            //             title= model.Name,
            //             rows = new List<ListRowDto>
            //             {
            //                 new ListRowDto
            //                 {
            //                     title = list.Title,
            //                     description = list.Description,
            //                     id = list.Id.ToString()
            //                 }
            //             }
            //         });
            //     }
            //
            // var response = new ListInteractiveMessageRequest()
            // {
            //     To = recepient,
            //     Recipient_type = model?.RecipientType,
            //     Type = "interactive",
            //     Interactive = new ListInteractiveMessageDto()
            //     {
            //         Footer = new Footer()
            //         {
            //             Text = model?.ReplyButtonMessage?.Footer
            //         },
            //         Header = new Header()
            //         {
            //             Text = model?.ReplyButtonMessage?.Header
            //         },
            //         Body = new Body()
            //         {
            //             Text = model?.ReplyButtonMessage?.Body
            //         },
            //         Type = model?.MessageType,
            //         Action = new ListActionDto()
            //         {
            //             button = ((BaseMessageTypeDetails) model?.ListMessage)?.Body,
            //             sections = sections
            //         }
            //     }
            // };

            // return response;
        //    throw new NotImplementedException();
        //}
    }
}