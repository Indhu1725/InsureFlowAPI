namespace InsureFlowAPI.DTOs.Customer
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PinCode { get; set; }

        public string NomineeName { get; set; }

        public string NomineeRelation { get; set; }
    }
}
