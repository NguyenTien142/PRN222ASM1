﻿namespace BusinessObject.BusinessObject.CustomerModels.Request;

public class CreateCustomerRequestDto
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}