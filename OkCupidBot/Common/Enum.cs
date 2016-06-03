using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    public enum Operator
    {
        Equals,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Contains,
    }



    [ProfileDetail]
    public enum Orientation
    {
        NULL,
        Straight,
        Gay,
        Bisexual,
        Asexual,
        Demisexual,
        Heteroflexible,
        Homoflexible,
        Lesbian,
        Pansexual,
        Queer,
        Questioning,
        Sapiosexual
    }

    [ProfileDetail]
    public enum Ethnicity
    {
        NULL,
        Asian,

        [RelationTo("Native American")]
        NativeAmerican,
        Indian,

        [RelationTo("Middle Eastern")]
        MiddleEastern,

        [RelationTo("Hispanic / Latin")]
        Hispanc,
        White,
        Black,

        [RelationTo("Pacific Islander")]
        PacificIslander,
        Other
    }

    [ProfileDetail]
    public enum RelationshipStatus
    {
        NULL,
        Single,

        [RelationTo("Seeing Someone")]
        SeeingSomeone,
        Married,

        [RelationTo("Open Relationship")]
        OpenRelationship
    }

    [ProfileDetail]
    public enum BodyType
    {
        NULL,

        [RelationTo("Rather Not Say")]
        RatherNotSay,
        Thin,
        Overweight,

        [RelationTo("Average Build")]
        Average,
        Fit,
        Jacked,

        [RelationTo("A Little Extra")]
        LittleExtra,
        Curvy,

        [RelationTo("Full Figured")]
        FullFigured,

        [RelationTo("Used Up")]
        UsedUp
    }

    [ProfileDetail]
    public enum Smoking
    {
        NULL,
        Yes,

        [RelationTo("Never Smokes")]
        No = 1,

        [RelationTo("Smokes regularly")]
        Regularly,

        [RelationTo("Smokes sometimes")]
        Sometimes
    }

    [ProfileDetail]
    public enum Drinking
    {
        NULL,

        [RelationTo("Drinks Socially")]
        Socially,

        [RelationTo("Not At All")]
        Never
    }

    [ProfileDetail]
    public enum Drugs
    {
        NULL,

        [RelationTo("Doesn’t do drugs")]
        Never,

        [RelationTo("Sometimes Does Drugs")]
        Sometimes = 2,

        [RelationTo("Does Drugs")]
        Yes
    }

    [ProfileDetail]
    public enum Religion
    {
        NULL,
        Agnosticism,
        Atheism,
        Christianity,
        Judasim,
        Catholicism,
        Islam,
        Hinduism,
        Buddihism,
        Sikh,
        Other
    }

    [ProfileDetail]
    public enum Sex
    {
        NULL,

        [RelationTo("Woman")]
        Female,

        [RelationTo("Man")]
        Male,
        TransWoman,
    }
}
