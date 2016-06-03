using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    [ProfileDetail]
    public enum Orientation
    {
        NULL,
        Straight,
        Gay,
        Bisexual,
        Asexual,
        Demisexual,
        Heteroflexibile,
        Homoflexibile,
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

        [RelationTo("Never Smokse")]
        No = 1,
        Sometimes
    }

    [ProfileDetail]
    public enum Drinking
    {
        NULL,
        Often,
        Socially,

        [RelationTo("Not At All")]
        NotAtAll
    }

    [ProfileDetail]
    public enum Drugs
    {
        NULL,
        Never,

        [RelationTo("Sometimes Does Drugs")]
        Sometimes = 2,
        Often
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
