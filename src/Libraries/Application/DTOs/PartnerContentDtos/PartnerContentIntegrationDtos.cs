using System;
using Domain.Entities;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs.PartnerContentDtos
{
	public class PartnerContentIntegrationDto
	{
        public Guid Id { get; set; }
        public string PartnerContentProcessorKey { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Headers { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Parameters { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Configs { get; set; }

        public string FullUrl { get; set; }
        public string MetaData { get; set; }

        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }
    }

    public class CreatePartnerContentIntegrationDto
    {
        public string PartnerContentProcessorKey { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Headers { get; set; }

        [Column(TypeName = "jsonb")]
        public List<KeyValueObj> Parameters { get; set; }

        public string FullUrl { get; set; }
        public string MetaData { get; set; }

        public Guid PartnerId { get; set; }
    }

    public class UpdatePartnerContentIntegrationDto: CreatePartnerContentIntegrationDto
    {
        public Guid Id { get; set; }
    }
}

