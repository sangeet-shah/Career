using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Middleware.Web.Domains.PaycorAPI;

public class PaycorAPIJobsResponse
{
    [JsonPropertyName("hasMoreResults")]
    public bool HasMoreResults { get; set; }

    [JsonPropertyName("continuationToken")]
    public string ContinuationToken { get; set; }

    [JsonPropertyName("additionalResultsUrl")]
    public string AdditionalResultsUrl { get; set; }

    [JsonPropertyName("records")]
    public IList<JobRecord> Records { get; set; }

    public class JobRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("atsAccountId")]
        public string AtsAccountId { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("openings")]
        public int Openings { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("confidential")]
        public bool Confidential { get; set; }

        [JsonPropertyName("internal")]
        public bool Internal { get; set; }

        [JsonPropertyName("timeToFill")]
        public int TimeToFill { get; set; }

        [JsonPropertyName("remoteStatus")]
        public string RemoteStatus { get; set; }

        [JsonPropertyName("eeoCategory")]
        public string EeoCategory { get; set; }

        [JsonPropertyName("payRange")]
        public PayRangeResponse PayRange { get; set; }

        [JsonPropertyName("atsLocation")]
        public AtsLocationResponse AtsLocation { get; set; }

        [JsonPropertyName("atsDepartment")]
        public AtsDepartmentResponse AtsDepartment { get; set; }

        [JsonPropertyName("hiringManagers")]
        public IList<object> HiringManagers { get; set; }

        [JsonPropertyName("recruiters")]
        public IList<object> Recruiters { get; set; }

        [JsonPropertyName("teamMembers")]
        public IList<object> TeamMembers { get; set; }

        [JsonPropertyName("executives")]
        public IList<object> Executives { get; set; }

        [JsonPropertyName("activatedDate")]
        public DateTime? ActivatedDate { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonPropertyName("postedToCareers")]
        public bool PostedToCareers { get; set; }

        [JsonPropertyName("postedToIndeed")]
        public bool PostedToIndeed { get; set; }

        [JsonPropertyName("postedToSmartSourcing")]
        public bool PostedToSmartSourcing { get; set; }

        [JsonPropertyName("postedToLinkedIn")]
        public bool PostedToLinkedIn { get; set; }

        [JsonPropertyName("postedToZipRecruiter")]
        public bool PostedToZipRecruiter { get; set; }

        [JsonPropertyName("postedToGravity")]
        public bool PostedToGravity { get; set; }

        public class PayRangeResponse
        {
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            [JsonPropertyName("minimum")]
            public double Minimum { get; set; }

            [JsonPropertyName("maximum")]
            public double Maximum { get; set; }

            [JsonPropertyName("payPeriod")]
            public string PayPeriod { get; set; }
        }

        public class AtsLocationResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("address1")]
            public string Address1 { get; set; }

            [JsonPropertyName("address2")]
            public string Address2 { get; set; }

            [JsonPropertyName("city")]
            public string City { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("postalCode")]
            public string PostalCode { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }
        }
        public class AtsDepartmentResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }
        }
    }
}