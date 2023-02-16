using System;
using System.Collections.Generic;

// Token: 0x0200003A RID: 58
[Serializable]
public class SavedLevel
{
	// Token: 0x04000545 RID: 1349
	public List<float[]> savedleveldata = new List<float[]>();

	// Token: 0x04000546 RID: 1350
	public List<float[]> bgdata = new List<float[]>();

	// Token: 0x04000547 RID: 1351
	public float endpoint;

	// Token: 0x04000548 RID: 1352
	public List<float[]> lyricspos = new List<float[]>();

	// Token: 0x04000549 RID: 1353
	public List<string> lyricstxt = new List<string>();

	// Token: 0x0400054A RID: 1354
	public int savednotespacing = 120;

	// Token: 0x0400054B RID: 1355
	public float tempo = 60f;

	// Token: 0x0400054C RID: 1356
	public int timesig = 2;

	// Token: 0x0400054D RID: 1357
	public float[] note_color_start = new float[3];

	// Token: 0x0400054E RID: 1358
	public float[] note_color_end = new float[3];
}
