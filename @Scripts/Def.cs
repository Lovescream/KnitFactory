using System;
using UnityEngine;

public static class Def {

    public const string URLPrivacyPolicy = "";
    public const string SupportMailAddress = "";
    
    public static long TimeMin => DateTime.MinValue.ToBinary();
    public static long TimeCurrent => DateTime.UtcNow.ToBinary();
    
    #region Levels

    public const ushort LEVEL_META_REVEAL = 15;
    public const ushort LEVEL_LOBBY_UNLOCKED = 15;
    public const ushort LEVEL_BANNER_REVEAL = 20;
    public const ushort LEVEL_INTERSTITIAL_REVEAL = 21;
    public const ushort LEVEL_LUCKYPASS_REVEAL = 25;

    public const ushort LEVEL_GIMMICK_LIFT = 7;
    public const ushort LEVEL_GIMMICK_AXISLOCK = 15;
    public const ushort LEVEL_GIMMICK_LAYER = 30;
    public const ushort LEVEL_GIMMICK_COMBINED = 50;
    public const ushort LEVEL_GIMMICK_KEYLOCK = 70;
    public const ushort LEVEL_GIMMICK_CATTOWER = 90;
    public const ushort LEVEL_GIMMICK_BUTTERFLY = 110;
    public const ushort LEVEL_GIMMICK_ICE = 130;
    public const ushort LEVEL_GIMMICK_SAND = 150;
    public const ushort LEVEL_GIMMICK_ANGRY = 170;
    public const ushort LEVEL_GIMMICK_SCRATCHER = 200;
    public const ushort LEVEL_GIMMICK_CATGATE = 250;
    public const ushort LEVEL_GIMMICK_RopeAndScissors = 300;
    public const ushort LEVEL_GIMMICK_ColorPath = 350;
    public const ushort LEVEL_GIMMICK_Hidden = 400;
    public const ushort LEVEL_GIMMICK_FROZENCAT = 450;

    public const ushort LEVEL_ITEM_CLOCK = 8;
    public const ushort LEVEL_ITEM_BALLOON = 12;
    public const ushort LEVEL_ITEM_CHURU = 16;

    public const ushort LEVEL_REVIEW_REVEAL = 15;

    #endregion

    #region Currency

    public const int CURRENCY_RECEIVE_AMOUNT = 40;
    public const int CURRENCY_RECEIVE_AMOUNT_REWARD = 80;

    public const int CURRENCY_CONTINUE_FIRST = 900;
    public const int CURRENCY_CONTINUE_SECONDS = 1400;
    public const int CURRENCY_CONTINUE_OTHERS = 1900;
    
    public const int CURRENCY_ITEM_CLOCK = 900;
    public const int CURRENCY_ITEM_WAND = 1400;
    public const int CURRENCY_ITEM_UFO = 1900;

    public const int CURRENCY_LIVES_REFILL = 900;

    public const int CURRENCY_META_RECEIVE_AMOUNT = 20;

    #endregion

    #region ITEM
    
    public const int ITEM_INITCOUNT = 2;
    public const int ITEM_PURCHASE_COUNT = 3;
    public const int ITEM_PURCHASE_COUNT_WAND = 1;
    public const int ITEM_PURCHASE_COUNT_UFO = 1;

    public const string PREFSKEY_ITEMGUIDE_CLOCK = "GuideClock";
    public const string PREFSKEY_ITEMGUIDE_WAND = "GuideWand";
    public const string PREFSKEY_ITEMGUIDE_UFO = "GuideUFO";

    #endregion

    #region PREFSKEY

    public const string PREFSKEY_SETTINGS_BGM = "SettingBGM";
    public const string PREFSKEY_SETTINGS_SFX = "SettingSFX";
    public const string PREFSKEY_SETTINGS_HAPTIC = "SettingHaptic";
    public const string PREFSKEY_UPDATEALERTVERSION = "AppUpdateAlertVersion";
    public const string PREFSKEY_UPDATEBONUSCOIN = "AppUpdateBonusCoin";
    
    #endregion

    #region LuckyPass

    public const ushort LUCKY_REWARD_NORMAL = 1;
    public const ushort LUCKY_REWARD_HARD = 1;
    public const ushort LUCKY_REWARD_SUPERHARD = 2;

    #endregion

    #region Sprites

    public static string GetCollectSpriteKey(CollectorItemType type) {
        string key = $"Icon_Item_{type}";
        if (Main.Resource.IsExist<Sprite>(key)) return key;
        return "Icon_Item_Box";
    }

    #endregion

}

public static class EventDef {
    // TODO::
    public const string LevelClear = "";

    public const string CoinPurchase1000 = "";
    public const string CoinPurchase5000 = "";

    public const string IsWatchAdFromDay = "";

    public const string PurchaseCount1 = "";
    public const string PurchaseCount5 = "";

    public const string LevelAttempt = "";

}