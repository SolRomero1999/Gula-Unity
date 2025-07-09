using UnityEngine;

public class UserGenerator : MonoBehaviour
{
    private static readonly string[] namePool = new string[]
    {
        "xXGamer99", "TTV_Eater", "MrFoodie", "DrChomp", "Sawkhe", "RonLimonMon", "Milk_ai",
        "KingNom", "QueenHungry", "ProSnack", "LilMunch", "HungryAF", "FoodDestroyer", "SnackKing", 
        "MukbangLover", "NoodleSlurp", "RamenDevil", "BurgerBeast", "PizzaGoblin", "TacoOverlord",
        "SushiSensei", "DumplingDiva", "SpicyRicky", "WasabiWizard", "UmamiUlysses", "BentoBandit", 
        "TempuraTim", "GyozaGuru", "TeriyakiTony", "MochiManiac", "PockyPete", "MatchaMike", "EbiEthan",
        "TofuTitan", "KimchiKarl", "BibimBop", "IkuraIvan", "TobikoTanya", "MasagoMilo", "HamachiHank", 
        "CurryKing", "SashimiSam", "UnagiYuki", "OkonomiOlly", "TakoyakiTina", "AwabiAlice", "UniUriel",
        "YakitoriYan", "EdamameEd", "HotPotHank", "DimSumDee", "PhoPhanatic", "KaniKara", "HotateHugo",
        "BubbleTeaBen", "TapiocaTroy", "MangoMolly", "DurianDave", "LycheeLiam", "IkaIvy", "TakoTerry",
        "PandaExpress", "BaoBaoBella", "XiaoXiao", "CongeeCarl", "JjajangJin", "SabaSonia", "AjiAiden", 
        "TteokTyler", "BingsuBetty", "HotteokHarry", "JapchaeJill", "BanchanBill", "FuguFiona", "AnagoAndy",
        "SojuSally", "MakgeolliMark", "SomaekSteve", "YuzuYvonne", "MirinMarty", "OdenOlga", "ChankoCharlie", 
        "DashiDanny", "KombuKyle", "BonitoBlake", "SakeSierra", "ChawanChad", "MotsuMaya", "NegimaNate",
        "TamagoTara", "OnigiriOscar", "FurikakeFinn", "NoriNancy", "PankoPaul", "TsukuneTess", "TebasakiTom",
        "TonkatsuTroy", "KaraageKai", "AgedashiAva", "DonburiDan", "KatsudonKate", "ShogayakiShawn", 
        "OyakodonOwen", "GyudonGrace", "UnadonUmar", "TendonTina", "SukiyakiSam", "YakinikuYara"
    };

    private static readonly string[] hexColors = new string[]
    {
        "#FF0000", "#0000FF", "#008000", "#B22222", "#FF7F50", "#9ACD32", "#FF4500", "#2E8B57",
        "#DAA520", "#D2691E", "#5F9EA0", "#1E90FF", "#FF69B4", "#BA55D3", "#9370DB", "#3CB371",
        "#7B68EE", "#00FA9A", "#48D1CC", "#C71585", "#191970", "#FF1493", "#FF6347", "#FFD700"
    };

    public static string GetRandomName()
    {
        return namePool[Random.Range(0, namePool.Length)];
    }

    public static Color GetRandomColor()
    {
        string hex = hexColors[Random.Range(0, hexColors.Length)];
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }
}