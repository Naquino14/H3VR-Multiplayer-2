namespace FistVR
{
	public enum FireArmMagazineType
	{
		mNone = 0,
		mG11 = 1,
		mCyber_Handgun = 2,
		mRuger_22Mk2 = 10,
		mRuger_1022 = 11,
		mP22_22LR = 12,
		mExplorer22 = 13,
		mMk4_22LR = 14,
		mMatchTarget_22LR = 0xF,
		mPMR30_22WM = 0x10,
		mBeretta_9x19mm = 20,
		mType100_8x22mm = 21,
		mTec9_9x19mm = 23,
		mPX4_9x19mm = 24,
		mBrowningHP_9x19mm = 25,
		mMP9_9x19mm = 26,
		mCZ75Shadow_9x19mm = 27,
		mGepard_9x19mm = 28,
		mP7M8_9x19mm = 29,
		mGlock_40SW = 30,
		mpp2000_9x19mm = 0x1F,
		mStenMk2_9x19mm = 0x20,
		mColt_9x19mm = 33,
		mP08Luger_9x19mm = 34,
		mMP40_9x19mm = 35,
		mMP34_9x19mm = 36,
		mP38_9x19mm = 37,
		mModel38_9x19mm = 38,
		mGlock_9mm = 39,
		mColtSingle_45acp = 40,
		mColtOversized_45acp = 41,
		mMP5_10mm = 42,
		mMP5_40SW = 43,
		mWebley1903_455Auto = 44,
		mUSP_9mm = 45,
		mDeaglov_10mm = 46,
		mSig250_45acp = 50,
		mLilGreasy_45acp = 51,
		mUSP_45acp = 52,
		mMac10_45acp = 53,
		mDesertEagle_44mag = 55,
		mDegle_50IM = 56,
		m1894_44mag = 57,
		mDesertEagle_357mag = 59,
		mTokarev_762_25 = 60,
		mPPSH41_762_25 = 61,
		mMauser_763_25 = 62,
		mK50m_762_25 = 0x3F,
		mBergmann_763_25 = 0x40,
		mBorchardt_763_25 = 65,
		m12gauge = 70,
		KWG12gauge = 71,
		mSaiga12k = 72,
		mCQB870 = 73,
		mAA12 = 74,
		mMP155k = 75,
		mToz106 = 76,
		mScalpel = 77,
		mU12 = 78,
		mDMK12 = 79,
		mIngram_380 = 80,
		mPPK_380 = 81,
		mSkorpion_380 = 82,
		mM1Carbine_30Carbine = 83,
		mM1928Proto_30Carbine = 84,
		mHydeLightRifle_30Carbine = 85,
		mNatoStanag_556 = 90,
		mG36_556 = 91,
		mAug_556 = 92,
		mFamasF1_556 = 93,
		mAK102_556 = 94,
		mSig552_556 = 95,
		mM249Box_556 = 96,
		mHK_Mp5_9mm = 100,
		mCobrayM11_9mm = 101,
		mBlyskawica_9mm = 102,
		mUzi_9mm = 103,
		mEvo3_9x19mm = 104,
		mAgram2000 = 105,
		mKP31_9x19mm = 106,
		mSpectreM4_9x19mm = 107,
		mVP9_9x19mm = 108,
		mP226_9x19mm = 109,
		mHK_UMP_45mm = 110,
		mVector_45mm = 111,
		mThompson_45mm = 112,
		mM3GreaseGun_45mm = 113,
		mAPC45_45acp = 114,
		mMk23_45acp = 115,
		mAKM_762_39mm = 120,
		mSKS_762_39mm = 121,
		mGalil_762_51mm = 122,
		mG3A3_762_51mm = 123,
		mP762_762_51mm = 124,
		mVZ58_762_39mm = 125,
		mAR10_762_51mm = 126,
		mBren807_762x39mm = 0x7F,
		mRPD_762x39mm = 0x80,
		mSako85_308Winchester = 130,
		m8400SOC_308Winchester = 131,
		mWA2000_300WinMag = 132,
		mAK74U_545_39mm = 140,
		mQBZ95_58x42mm = 141,
		mDSVD_762x54mmR = 150,
		mSVT40_762x54mmR = 151,
		mDP28_762x54mmR = 152,
		mSV98_762x54mmR = 153,
		mM107A1_50BMG = 160,
		mHecate2_50BMG = 161,
		m950Jetfire_25ACP = 170,
		mFN1906_25ACP = 171,
		mKuul_25ACP = 172,
		mAEK919k_9x18mm = 180,
		mBizon_9x18mm = 181,
		mPP91KEDR_9x18mm = 182,
		mMakarov_9x18mm = 183,
		mPL14_9x19mm = 184,
		mGSh18_9x19mm = 185,
		mMAS4956_75mmFrench = 190,
		mStG44_792x33mmKurz = 191,
		mModel81_35Remington = 192,
		mMinigun_762x51mm = 200,
		mM14_762x51mm = 201,
		mMk17_762x51mm = 202,
		mFRF2_762x51mm = 203,
		mSt_762x51mm = 204,
		mMp7a1_46x30mm = 210,
		mSCX_46x30mm = 211,
		mP90_57x28mm = 220,
		mFiveSeven_57x28mm = 221,
		mASVal_9x39mm = 222,
		mUnion_32ACP = 230,
		mRuby_32ACP = 231,
		mVolcanic_41 = 240,
		mWinchesterLongColt_45 = 241,
		mBergmannNo2_5mm = 242,
		m8mmBergmannSimplex = 250,
		m36FrontierModelB = 251,
		mModel835Remington = 252,
		mC96MauserInternal = 253,
		m792x57mmMauserInternal = 254,
		m1903SpringfieldInteral = 0xFF,
		m38MosinInternal = 0x100,
		mLeeEnfieldInteral = 257,
		mModel70Internal = 258,
		aJohnson1941Internal = 259,
		mM1GarandEnBlock = 260,
		mSKSInteral = 261,
		m1918_3006 = 270,
		mBrenLMG_303 = 271,
		mMG42_792 = 272,
		mM2Tombstone_50BMG = 273,
		mHandCrankFrank_4570 = 274,
		mM60_762_51 = 275,
		mDesertEagle_50AE = 280,
		mThompson_10mmDSM = 281,
		mHKCAWS_12gBelted = 282,
		mM40Internal = 290,
		mKS23Interal = 291,
		mAWM_338Lapua = 300,
		mCarcano1891 = 301,
		mM200_408 = 302,
		mHoneyBadger_300BAC = 303,
		mPocketHammer1901 = 304,
		mMRAD_338Lapua = 305,
		mFlameThrowerTank = 310,
		mStingerBattery = 311,
		mKtest = 312,
		mHCBTest = 313,
		mPatoot = 314,
		mtest_RL = 400,
		mf_ge_pistol = 402,
		mf_sn_smg = 403,
		mf_dm_stickylauncher = 404,
		mf_py_flametank = 405,
		mf_me_syringegun = 406,
		mf_he_minigun = 407,
		mf_ge_ar = 408,
		mf_ge_sc = 409,
		sl_ar3 = 450,
		mTest1 = 1001,
		mTest2 = 1002,
		mTest3 = 1003,
		mTest4 = 1004,
		mTest5 = 1005,
		mTest6 = 1006,
		mTest7 = 1007,
		mTest8 = 1008,
		mTest9 = 1009,
		mTest10 = 1010,
		mTest11 = 1011,
		mTest12 = 1012,
		mTest13 = 1013,
		mTest14 = 1014,
		mTest15 = 1015,
		mTest16 = 1016,
		mTest17 = 1017,
		mTest18 = 1018,
		mTest19 = 1019,
		mTest20 = 1020,
		mTest21 = 1021,
		mTest22 = 1022,
		mTest23 = 0x3FF,
		mTest24 = 0x400
	}
}