using System.ComponentModel.DataAnnotations;

namespace Career.Data.Domains.LandingPages;
public enum SponsorshipLevelEnum
{
    [Display(Name = "Executive sponsor")]
    ExecutiveSponsor = 1,

    [Display(Name = "Diamond sponsor")]
    DiamondSponsor = 2,

    [Display(Name = "Titanium sponsor")]
    TitaniumSponsor = 3,

    [Display(Name = "Dinner sponsor")]
    DinnerSponsor = 4,

    [Display(Name = "Lunch sponsor")]
    LunchSponsor = 5,

    [Display(Name = "Gold sponsor")]
    GoldSponsor = 6,

    [Display(Name = "Practice range sponsor")]
    PracticeRangeSponsor = 7,

    [Display(Name = "Practice green sponsor")]
    PracticeGreenSponsor = 8,

    [Display(Name = "Flag sponsor")]
    FlagSponsor = 9,

    [Display(Name = "Individual player")]
    IndividualPlayer = 10,

    [Display(Name = "Meal Sponsor")]
    MealSponsor = 11,

    [Display(Name = "Hole Sponsor")]
    HoleSponsor = 12
}