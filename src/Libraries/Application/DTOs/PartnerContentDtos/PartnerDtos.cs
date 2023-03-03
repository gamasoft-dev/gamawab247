using System;
namespace Application.DTOs.PartnerContentDtos
{
	public class PartnerDto
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // This would have a value if the  business is also a partner
        public Guid? BusinessId { get; set; }
    }

    public class CreatePartnerDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // This would have a value if the  business is also a partner
        public Guid? BusinessId { get; set; }
    }

    public class UpdatePartnerDto: CreatePartnerDto
    {
    }
}

