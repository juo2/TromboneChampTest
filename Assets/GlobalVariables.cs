using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class GlobalVariables : MonoBehaviour
{
	// Token: 0x0400033F RID: 831
	public static bool playtest_mode = false;

	// Token: 0x04000340 RID: 832
	public static string playtest_origin = "start";

	// Token: 0x04000341 RID: 833
	public static string version = "1.021";

	// Token: 0x04000342 RID: 834
	//public static SavedCardCollection localsave;

	// Token: 0x04000343 RID: 835
	//public static SavedSettings localsettings;

	// Token: 0x04000344 RID: 836
	public static bool latch_opened = false;

	// Token: 0x04000345 RID: 837
	public static string scene_destination = "";

	// Token: 0x04000346 RID: 838
	public static string tootscene = "treble";

	// Token: 0x04000347 RID: 839
	public static int chosen_track_index = 0;

	// Token: 0x04000348 RID: 840
	public static string chosen_track = "";

	// Token: 0x04000349 RID: 841
	public static int chosen_character = 0;

	// Token: 0x0400034A RID: 842
	public static int chosen_trombone = 0;

	// Token: 0x0400034B RID: 843
	public static int chosen_soundset = 0;

	// Token: 0x0400034C RID: 844
	public static int demon_sets_given = 0;

	// Token: 0x0400034D RID: 845
	public static string[] data_trackrefs = new string[]
	{
		"1",
		"2",
		"3",
		"4",
		"5",
		"6",
		"7",
		"8",
		"9",
		"10"
	};

	// Token: 0x0400034E RID: 846
	public static string[][] data_tracktitles = new string[][]
	{
		new string[]
		{
			"Track 1",
			"T1",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 2",
			"T2",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 3",
			"T3",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 4",
			"T4",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 5",
			"T5",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 6",
			"T6",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 7",
			"T7",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 8",
			"T8",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 9",
			"T9",
			"2021",
			"djv",
			"Genre",
			"short desc"
		},
		new string[]
		{
			"Track 10",
			"T10",
			"2021",
			"djv",
			"Genre",
			"short desc"
		}
	};

	// Token: 0x0400034F RID: 847
	public static string[][] data_trackscores = new string[][]
	{
		new string[]
		{
			"warmup",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"tundra",
			"S",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"ballgame",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"mare",
			"F",
			"6000",
			"5000",
			"4000",
			"3000",
			"2000"
		},
		new string[]
		{
			"eine",
			"F",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"blowfly",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"sugarplum",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"baboons",
			"A",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"alsosprach",
			"A",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"entertainer",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"skiptomylou",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"anthem",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"gladiators",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"havanagila",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"williamtell",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"bluedanube",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"starsandstripes",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"skabone",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"beethovensfifth",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"godsave",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"ocanada",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		},
		new string[]
		{
			"merengue",
			"-",
			"0",
			"0",
			"0",
			"0",
			"0"
		}
	};

	// Token: 0x04000350 RID: 848
	public static float gameplay_scoreperc = 1.25f;

	// Token: 0x04000351 RID: 849
	public static int gameplay_scoretotal = 0;

	// Token: 0x04000352 RID: 850
	public static int[] gameplay_notescores = new int[]
	{
		50,
		40,
		30,
		20,
		10
	};

	// Token: 0x04000353 RID: 851
	public static float gamescrollspeed = 1f;

	// Token: 0x04000354 RID: 852
	public static int[] all_res_w = new int[]
	{
		3840,
		3200,
		2560,
		1920,
		1600,
		1366,
		1280,
		1152,
		1024
	};

	// Token: 0x04000355 RID: 853
	public static int[] all_res_h = new int[]
	{
		2160,
		1800,
		1440,
		1080,
		900,
		768,
		720,
		648,
		576
	};

	// Token: 0x04000356 RID: 854
	public static List<int> available_res_w = new List<int>();

	// Token: 0x04000357 RID: 855
	public static List<int> available_res_h = new List<int>();

	// Token: 0x04000358 RID: 856
	public static int bab_qty_loaded = 0;

	// Token: 0x04000359 RID: 857
	public static int bab_pref_loaded = 0;

	// Token: 0x0400035A RID: 858
	public static bool jumpscares = true;

	// Token: 0x0400035B RID: 859
	public static int mousecontrolmode = 0;
}
