using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.FormProcessing.ValueObjects;

namespace Domain.Entities.FormProcessing
{
    /// <summary>
    /// This entity hold a details of the session and business form and list of user inputs in the course of the
    /// form data collation.
    /// </summary>
    public class UserFormData: AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid BusinessFormId { get; set; }
        public Guid DialogSessionId { get; set; }

        /// <summary>
        /// This would hold list of all user inputs for a particular form processing session.
        /// Note userinput detail is ordered by id which should be an incremental integer.
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<UserInputDetail> UserInputDetails { get; set; }
        public bool IsFormCompleted { get; set; }


        public BusinessForm BusinessForm { get; set; }
    }
}

