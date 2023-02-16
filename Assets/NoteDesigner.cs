using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200002F RID: 47
public class NoteDesigner : MonoBehaviour
{
	// Token: 0x06000350 RID: 848 RVA: 0x0002790C File Offset: 0x00025B0C
	public void setColorScheme(float col_r, float col_g, float col_b, float col_r2, float col_g2, float col_b2)
	{
		this.g = new Gradient();
		this.gck = new GradientColorKey[2];
		this.gak = new GradientAlphaKey[2];
		this.gak[0].alpha = 1f;
		this.gak[0].time = 0f;
		this.gak[1].alpha = 1f;
		this.gak[1].time = 1f;
		this.gck[0].time = 0.4f;
		this.gck[1].time = 0.6f;
		Color32 c = new Color(col_r, col_g, col_b, 1f);
		Color32 c2 = new Color(col_r2, col_g2, col_b2, 1f);
		this.startdot.color = c;
		this.gck[0].color = c;
		this.enddot.color = c2;
		this.gck[1].color = c2;
		this.g.SetKeys(this.gck, this.gak);
		this.colorline.colorGradient = this.g;
	}

	// Token: 0x04000488 RID: 1160
	public Image startdot;

	// Token: 0x04000489 RID: 1161
	public Image enddot;

	// Token: 0x0400048A RID: 1162
	public LineRenderer colorline;

	// Token: 0x0400048B RID: 1163
	private Gradient g;

	// Token: 0x0400048C RID: 1164
	private GradientColorKey[] gck;

	// Token: 0x0400048D RID: 1165
	private GradientAlphaKey[] gak;
}
