using System.Collections.Generic;

namespace Career.Web.Models.ScheduleOrder;

public record CustomerResponse
{
    public int status { get; set; }
    public Customer customer { get; set; }
}

public record BillingAddress
{
    public string firstName { get; set; }
    public string middleInitial { get; set; }
    public string lastName { get; set; }
    public string prefix { get; set; }
    public string suffix { get; set; }
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zipCode { get; set; }
}

public record LegalSetting
{
    public string description { get; set; }
    public bool enabled { get; set; }
}

public record MergeStatus
{
    public string mergeToCustomerId { get; set; }
    public string status { get; set; }
}

public record Phone
{
    public string number { get; set; }
    public string phoneType { get; set; }
}

public record Customer
{
    public int status { get; set; }
    public string customerId { get; set; }
    public string fullName { get; set; }
    public string emailAddress { get; set; }
    public string firstName { get; set; }
    public string middleInitial { get; set; }
    public string lastName { get; set; }
    public string accountComments { get; set; }
    public BillingAddress billingAddress { get; set; }
    public bool chapter13 { get; set; }
    public bool chapter7 { get; set; }
    public bool chargeOff { get; set; }
    public string coapName { get; set; }
    public bool deceased { get; set; }
    public bool deferredBankruptcy { get; set; }
    public bool doNotReport { get; set; }
    public int dobDay { get; set; }
    public int dobMonth { get; set; }
    public int dobYear { get; set; }
    public string driversLicense { get; set; }
    public int dueDate { get; set; }
    public bool financeInfoExists { get; set; }
    public bool franchiseChargeAccount { get; set; }
    public bool franchiseIsChargedOff { get; set; }
    public bool franchiseNotResponsible { get; set; }
    public bool fraud { get; set; }
    public bool hasAccessToFinance { get; set; }
    public bool inCollections { get; set; }
    public bool inLitigation { get; set; }
    public object inactiveDate { get; set; }
    public bool insuranceClaim { get; set; }
    public bool isAccountLocked { get; set; }
    public List<LegalSetting> legalSettings { get; set; }
    public MergeStatus mergeStatus { get; set; }
    public bool omitAutoEmail { get; set; }
    public bool optInForMarketing { get; set; }
    public bool outsideCollections { get; set; }
    public bool repo { get; set; }
    public object shippingAddress { get; set; }
    public bool skipped { get; set; }
    public string ssn { get; set; }
    public int statementDeliveryMethod { get; set; }
    public string storeName { get; set; }
    public string storePhone { get; set; }
    public List<Phone> phones { get; set; }
    public bool hasRewardPlan { get; set; }
    public string shippingInstructions { get; set; }
}
