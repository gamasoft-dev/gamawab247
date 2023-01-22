using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.FormProcessing
{
    public class BusinessFormConlusionConfig: AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessFormId { get; set; }

        [Column(TypeName = "jsonb")]
        public List<FormConclusionProcessesConfig> ConfigDetails { get; set; }

    }

    /// <summary>
    /// Value object.
    /// </summary>
    public class FormConclusionProcessesConfig
    {
        public string FormConclusionProcessName { get; set; }

        /// <summary>
        /// 1 means on while 0 means off. So a formconclusion process can either be toggled on or off.
        /// </summary>
        public int ToggleValue { get; set; }
        public int Priority { get; set; }
    }
}

