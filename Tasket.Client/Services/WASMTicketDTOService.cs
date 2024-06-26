﻿using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using Tasket.Client.Models;
using Tasket.Client.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace Tasket.Client.Services
{
    public class WASMTicketDTOService(HttpClient _httpClient) : ITicketDTOService
    {

        #region Get Many
        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId)
        {
            IEnumerable<TicketDTO> tickets = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>("api/tickets") ?? [];
            return tickets;
        }

        public async Task<IEnumerable<TicketDTO>> GetUserTicketsAsync(int companyId, string userId)
        {
            IEnumerable<TicketDTO> tickets = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>("api/tickets/assignments") ?? [];
            return tickets;
        }
        public async Task<IEnumerable<TicketDTO>> GetUsersRecentlyEditedTicketsAsync(int companyId, string userId)
        {
            IEnumerable<TicketDTO> tickets = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>("api/tickets/recent") ?? [];
            return tickets;
        }
        #endregion



        #region Get One
        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            TicketDTO? ticket = await _httpClient.GetFromJsonAsync<TicketDTO>($"api/tickets/{ticketId}");
            return ticket;
        }
        #endregion



        #region Update W/ Return
        public async Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/tickets", ticket);
            response.EnsureSuccessStatusCode();

            TicketDTO? ticketDTO = await response.Content.ReadFromJsonAsync<TicketDTO>();
            return ticketDTO!;
        }
        #endregion




        #region Update DB item
        public async Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }
        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/archive/{ticketId}", ticketId);
            response.EnsureSuccessStatusCode();
        }
        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/restore/{ticketId}", ticketId);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            IEnumerable<TicketCommentDTO> comments = await _httpClient.GetFromJsonAsync<IEnumerable<TicketCommentDTO>>($"api/tickets/{ticketId}/comments") ?? [];
            return comments;
        }

        public async Task<TicketCommentDTO?> GetCommentByIdAsync(int ticketCommentId, int companyId)
        {
            TicketCommentDTO? ticket = await _httpClient.GetFromJsonAsync<TicketCommentDTO>($"api/tickets/comments/{ticketCommentId}");
            return ticket;
        }

        public async Task AddCommentAsync(TicketCommentDTO comment, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/tickets/comments/{comment.Id}", comment);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCommentAsync(int commentId, int companyId)
        {
            HttpResponseMessage? response = await _httpClient.DeleteAsync($"api/tickets/comments/{commentId}");
        }

        public async Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/comments/{comment.Id}", comment);
            response.EnsureSuccessStatusCode();
        }
        #endregion





        #region Attachments
        public async Task<TicketAttachmentDTO> AddTicketAttachment(TicketAttachmentDTO attachment, byte[] uploadData, string contentType, int companyId)
        {
            using var formData = new MultipartFormDataContent();
            formData.Headers.ContentDisposition = new("form-data");

            var fileContent = new ByteArrayContent(uploadData);
            fileContent.Headers.ContentType = new(contentType);

            if (string.IsNullOrWhiteSpace(attachment.FileName))
            {
                formData.Add(fileContent, "file");
            }
            else
            {
                formData.Add(fileContent, "file", attachment.FileName);
            }

            formData.Add(new StringContent(attachment.Id.ToString()), nameof(attachment.Id));
            formData.Add(new StringContent(attachment.FileName ?? string.Empty), nameof(attachment.FileName));
            formData.Add(new StringContent(attachment.Description ?? string.Empty), nameof(attachment.Description));
            formData.Add(new StringContent(DateTimeOffset.Now.ToString()), nameof(attachment.Created));
            formData.Add(new StringContent(attachment.UserId ?? string.Empty), nameof(attachment.UserId));
            formData.Add(new StringContent(attachment.TicketId.ToString()), nameof(attachment.TicketId));

            var res = await _httpClient.PostAsync($"api/tickets/{attachment.TicketId}/attachments", formData);
            res.EnsureSuccessStatusCode();

            var addedAttachment = await res.Content.ReadFromJsonAsync<TicketAttachmentDTO>();
            return addedAttachment!;
        }

        public async Task DeleteTicketAttachment(int attachmentId, int companyId)
        {
            var res = await _httpClient.DeleteAsync($"api/tickets/attachments/{attachmentId}");
            res.EnsureSuccessStatusCode();
        }

        public Task<TicketAttachmentDTO?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
