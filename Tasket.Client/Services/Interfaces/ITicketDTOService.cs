﻿using System.Net.Sockets;
using Tasket.Client.Models;

namespace Tasket.Client.Services.Interfaces
{
    public interface ITicketDTOService
    {
        #region Get List Items
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId);

        #endregion


        #region Get One Item
        Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId);
        #endregion



        #region Update DB Item/Items
        Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId);
        Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId);
        Task ArchiveTicketAsync(int ticketId, int companyId);
        Task RestoreTicketAsync(int ticketId, int companyId);
        #endregion





        #region Comments
        #region Get List Items
        Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId);
        #endregion


        #region Get One Item
        Task<TicketCommentDTO?> GetCommentByIdAsync(int ticketId, int companyId);
        #endregion


        #region Update DB Item (Returns Nothing)
        Task AddCommentAsync(TicketCommentDTO comment, int companyId);
        Task DeleteCommentAsync(int commentId, int companyId);
        Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId);
        #endregion
        #endregion
    }
}