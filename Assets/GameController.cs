
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public GameObject pointer;
	public Image pointerimg;
    // Token: 0x04000229 RID: 553
    public GameObject leftbounds;

    // Token: 0x0400022A RID: 554
    public GameObject leftboundsglow;

    // Token: 0x0400022B RID: 555
    private RectTransform pointerrect;

    // Token: 0x0400023B RID: 571
    public GameObject beatline;

    // Token: 0x0400023C RID: 572
    private int beatlineindex;

    // Token: 0x0400023D RID: 573
    private float maxbeatlinex;

    private RectTransform[] allbeatlines = new RectTransform[12];

    public GameObject noteholder;

    private RectTransform noteholderr;

    public GameObject lyricsholder;

    private RectTransform lyricsholderr;

    // Token: 0x04000246 RID: 582
    public GameObject singlenote;

    // Token: 0x04000247 RID: 583
    public GameObject singlelyric;

    // Token: 0x04000249 RID: 585
    private List<float[]> leveldata = new List<float[]>();

    // Token: 0x0400024A RID: 586
    private List<GameObject> alllyrics = new List<GameObject>();

    // Token: 0x0400024B RID: 587
    private List<float[]> lyricdata_pos = new List<float[]>();

    // Token: 0x0400024C RID: 588
    private List<string> lyricdata_txt = new List<string>();

    // Token: 0x0400026E RID: 622
    public AudioSource currentnotesound;

    // Token: 0x0400026F RID: 623
    public AudioClipsTromb trombclips;

	private float levelendpoint;

	private float tempo = 40f;

	private int defaultnotelength = 240;

	private int beatspermeasure = 2;

	private float levelendtime;

	private float trackmovemult = 1f;

	private int currentnoteindex;

	private float currentnotestart;

	// Token: 0x04000260 RID: 608
	private List<GameObject> allnotes = new List<GameObject>();

	// Token: 0x04000261 RID: 609
	private List<float[]> allnotevals = new List<float[]>();

	private bool flipscheme;

	private float currentnoteend;

	// Token: 0x04000265 RID: 613
	private float currentnotestarty;

	// Token: 0x04000266 RID: 614
	private float currentnoteendy;

	private float currentnotepshift;

	private int beatstoshow = 16;

	private int levelnotesize = 20;

	private int numbeatlines = 6;

	public bool freeplay;

	private bool playingineditor;

	private int scores_A;

	// Token: 0x040002A3 RID: 675
	private int scores_B;

	// Token: 0x040002A4 RID: 676
	private int scores_C;

	// Token: 0x040002A5 RID: 677
	private int scores_D;

	// Token: 0x040002A6 RID: 678
	private int scores_F;

	private float scorecounter;

	public GameObject musicref;

	public AudioSource musictrack;

	private float currenthealth;

	private float latency_offset;

	private float noteoffset = 0.05f;

	private bool level_finshed;

	public bool quitting;

	private bool paused;

	private int totalscore;

	private int maxlevelscore;

	private float tempotimer;

	// Token: 0x04000256 RID: 598
	private float tempotimerdot;

	// Token: 0x04000257 RID: 599
	private int beatnum = 1;

	private int timesigcount = 1;

	private float breathscale = 1f;

	private int beatnumdot = 1;

	private float zeroxpos = 60f;

	public int pointercolorindex;

	private bool noteactive;

	private int multiplier;

	private bool enteringlyrics;

	private bool released_button_between_notes;

	private int highestcombocounter;
	// Token: 0x0400026B RID: 619
	private float note_end_timer;

	// Token: 0x0400026C RID: 620
	private float max_note_end_timer = 0.25f;

	private bool noteplaying;

	// Token: 0x04000269 RID: 617
	private float notescoreaverage;

	// Token: 0x0400026A RID: 618
	private float notescoresamples = 1f;

	public bool controllermode = true;

	private bool autoplay;

	private float mousemult = 1.35f;

	private float startmouseX;

	private int dotsize = 60;

	private float vsensitivity = -350f;

	// Token: 0x04000273 RID: 627
	private float vbounds = 165f;

	// Token: 0x04000274 RID: 628
	private float outerbuffer = 15f;

	private float movesmoothing = 35f;

	private float[] notelinepos = new float[15];

	private bool outofbreath;

	private bool readytoplay;

	private float pitchamount = 0.00501f;

	private float breathcounter;

	// Token: 0x04000240 RID: 576
	private float[] note_c_start = new float[]
	{
		1f,
		1f,
		1f
	};

	// Token: 0x04000241 RID: 577
	private float[] note_c_end = new float[]
	{
		1f,
		1f,
		1f
	};

	private float notestartpos;


	private void Start()
    {
		latency_offset = 0;
		Application.targetFrameRate = 144;
		buildLevel();

		this.musictrack = this.musicref.GetComponent<AudioSource>();
		this.startSong();
	}

	private void startSong()
	{
		this.tempotimer = 60f / (this.tempo * (float)this.beatspermeasure);
		this.tempotimerdot = 60f / this.tempo;
		base.Invoke("playsong", 2.5f);
		base.Invoke("startDance", 2.5f);
		this.calcMaxScore();
		//if (this.puppet_human)
		//{
		//	LeanTween.scaleY(this.puppet_human, 1f, 0.5f).setEaseOutBounce().setDelay(2f);
		//}
	}

	private void calcMaxScore()
	{
		this.maxlevelscore = 0;
		for (int i = 0; i < this.leveldata.Count; i++)
		{
			float f = Mathf.Floor(Mathf.Floor(this.leveldata[i][1] * 10f) * 100f * 1.3f) * 10f;
			this.maxlevelscore += Mathf.FloorToInt(f);
		}
		Debug.Log(string.Concat(new object[]
		{
			"max level score: ",
			this.maxlevelscore,
			" (",
			this.leveldata.Count,
			" notes)"
		}));
	}

	private void buildLevel()
	{
		this.trackmovemult = this.tempo / 60f * (float)this.defaultnotelength;
		this.tryToLoadLevel("warmup");
		this.currentnoteindex = 0;
		Debug.Log("making " + this.numbeatlines + " beat lines");
		for (int i = 0; i < this.numbeatlines; i++)
		{
			Debug.Log("beat line " + i);
			RectTransform component = GameObject.Instantiate<GameObject>(this.beatline, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform).GetComponent<RectTransform>();
			this.allbeatlines[i] = component;
			component.anchoredPosition3D = new Vector3((float)((i + 1) * this.defaultnotelength * this.beatspermeasure), 0f, 0f);
			Debug.Log(component.anchoredPosition3D.x);
			if (component.anchoredPosition3D.x > this.maxbeatlinex)
			{
				this.maxbeatlinex = component.anchoredPosition3D.x;
			}
			Debug.Log(this.maxbeatlinex);
		}
		this.grabNoteRefs(0);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00018CC8 File Offset: 0x00016EC8
	private void tryToLoadLevel(string filename)
	{
		string text = string.Empty;
		if (filename == "EDITOR")
		{
			//text = Application.streamingAssetsPath + "/leveldata/" + this.levelnamefield.text + ".tmb";
		}
		else
		{
			text = Application.streamingAssetsPath + "/leveldata/" + filename + ".tmb";
		}
		Debug.Log(text);
		if (File.Exists(text))
		{
			Debug.Log("found level");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(text, FileMode.Open);
			SavedLevel savedLevel = (SavedLevel)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			//this.bgdata.Clear();
			//this.bgdata = savedLevel.bgdata;
			this.leveldata.Clear();
			this.leveldata = savedLevel.savedleveldata;
			this.lyricdata_pos = savedLevel.lyricspos;
			this.lyricdata_txt = savedLevel.lyricstxt;
			if (savedLevel.note_color_start == null)
			{
				Debug.Log("no color data :-(");
			}
			else
			{
				this.note_c_start = savedLevel.note_color_start;
				this.note_c_end = savedLevel.note_color_end;
				//this.col_r_1.text = this.note_c_start[0].ToString();
				//this.col_g_1.text = this.note_c_start[1].ToString();
				//this.col_b_1.text = this.note_c_start[2].ToString();
				//this.col_r_2.text = this.note_c_end[0].ToString();
				//this.col_g_2.text = this.note_c_end[1].ToString();
				//this.col_b_2.text = this.note_c_end[2].ToString();
			}
			this.levelendpoint = savedLevel.endpoint;
			//this.editorendpostext.text = "end: " + this.levelendpoint;
			this.tempo = savedLevel.tempo;
			this.defaultnotelength = savedLevel.savednotespacing;
			this.defaultnotelength = Mathf.FloorToInt((float)this.defaultnotelength * GlobalVariables.gamescrollspeed);
			this.beatspermeasure = savedLevel.timesig;
			//if (this.leveleditor)
			//{
			//	this.buildAllBGNodes();
			//}

			this.buildNotes();
			this.buildAllLyrics();
			//this.changeEditorTempo(0);
			//this.moveTimeline(0);
			//this.changeTimeSig(0);
			this.levelendtime = 60f / this.tempo * this.levelendpoint;
			Debug.Log("level end TIME: " + this.levelendtime);
			Debug.Log("Game Loaded");
			return;
		}
		Debug.Log("No file exists at that filename!");
	}

	private void buildNotes()
	{
		//int num = this.levelnum;
		for (int i = 0; i < this.leveldata.Count; i++)
		{
			float[] array = new float[]
			{
				9999f,
				9999f,
				9999f,
				9999f,
				9999f
			};
			GameObject gameObject = new GameObject();
			if (i > 0)
			{
				array = this.leveldata[i - 1];
				gameObject = this.allnotes[i - 1];
			}
			float[] array2 = this.leveldata[i];
			GameObject gameObject2 = GameObject.Instantiate<GameObject>(this.singlenote, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform);
			bool flag = false;
			if (array2[0] == array[0] + array[1])
			{
				flag = true;
			}
			this.allnotes.Add(gameObject2);
			NoteDesigner component = gameObject2.GetComponent<NoteDesigner>();
			if (flag)
			{
				if (!this.flipscheme)
				{
					this.flipscheme = true;
				}
				else
				{
					this.flipscheme = false;
				}
			}
			else
			{
				this.flipscheme = false;
			}
			component.setColorScheme(this.note_c_start[0], this.note_c_start[1], this.note_c_start[2], this.note_c_end[0], this.note_c_end[1], this.note_c_end[2]);
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			GameObject gameObject3 = gameObject2.transform.GetChild(0).gameObject;
			if (flag)
			{
				gameObject.transform.GetChild(1).gameObject.SetActive(false);
				gameObject3.SetActive(false);
			}
			else
			{
				gameObject3.SetActive(true);
			}
			GameObject gameObject4 = gameObject2.transform.GetChild(1).gameObject;
			RectTransform component3 = gameObject4.GetComponent<RectTransform>();
			GameObject gameObject5 = gameObject2.transform.GetChild(2).gameObject;
			LineRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<LineRenderer>();
			component2.anchoredPosition3D = new Vector3(array2[0] * (float)this.defaultnotelength, array2[2], 0f);
			component3.anchoredPosition3D = new Vector3((float)this.defaultnotelength * array2[1] - (float)this.levelnotesize, array2[3], 0f);
			gameObject4.transform.localScale = new Vector3(0.77f, 0.77f, 1f);
			float[] item = new float[]
			{
				array2[0] * (float)this.defaultnotelength,
				array2[0] * (float)this.defaultnotelength + (float)this.defaultnotelength * array2[1],
				array2[2],
				array2[3],
				array2[4]
			};
			this.allnotevals.Add(item);
			float num2 = (float)this.defaultnotelength * array2[1];
			float c = array2[3];
			foreach (LineRenderer lineRenderer in componentsInChildren)
			{
				if (!flag)
				{
					lineRenderer.SetPosition(0, new Vector3(-3f, 0f, 0f));
				}
				for (int k = 1; k < 10; k++)
				{
					lineRenderer.SetPosition(k, new Vector3(num2 / 9f * (float)k, this.easeInOutVal((float)k, 0f, c, 9f), 0f));
				}
			}
			if (i > this.beatstoshow)
			{
				gameObject2.SetActive(false);
			}
		}
	}

	private float easeInOutVal(float t, float b, float c, float d)
	{
		t /= d / 2f;
		if (t < 1f)
		{
			return c / 2f * t * t + b;
		}
		t -= 1f;
		return -c / 2f * (t * (t - 2f) - 1f) + b;
	}


	private void grabNoteRefs(int indexinc)
	{
		this.currentnoteindex += indexinc;
		if (this.currentnoteindex > this.allnotevals.Count - 1)
		{
			Debug.Log("ALL DONE!!!");
			this.currentnotestart = 9999999f;
			this.currentnoteend = 100000000f;
			return;
		}
		float[] array = this.allnotevals[this.currentnoteindex];
		this.currentnotestart = array[0];
		this.currentnoteend = array[1];
		this.currentnotestarty = array[2];
		this.currentnoteendy = array[4];
		this.currentnotepshift = this.currentnoteendy - this.currentnotestarty;
	}

	private void buildAllLyrics()
	{
		for (int i = 0; i < this.lyricdata_pos.Count; i++)
		{
			this.enterLyric(this.lyricdata_pos[i][0], this.lyricdata_pos[i][1], this.lyricdata_txt[i], false);
		}
	}

	private void enterLyric(float xpos, float ypos, string word, bool addtoarray)
	{
		Debug.Log(string.Concat(new object[]
		{
			"ENTERING ",
			word,
			" at ",
			xpos,
			" beat, ",
			ypos,
			" position"
		}));
		GameObject gameObject = GameObject.Instantiate<GameObject>(this.singlelyric, new Vector3(0f, 0f, 0f), Quaternion.identity, this.lyricsholder.transform);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		Text component2 = gameObject.transform.GetChild(1).GetComponent<Text>();
		Text component3 = gameObject.transform.GetChild(0).GetComponent<Text>();
		component2.text = word;
		component3.text = word;
		component.anchoredPosition3D = new Vector3(xpos * (float)this.defaultnotelength, 0f, 0f);
		if (addtoarray)
		{
			this.alllyrics.Add(gameObject);
			float[] item = new float[]
			{
				xpos,
				ypos
			};
			this.lyricdata_pos.Add(item);
			this.lyricdata_txt.Add(word);
		}
	}

	private void flashLeftBounds()
	{
		LeanTween.cancel(this.leftboundsglow);
		this.leftboundsglow.transform.localScale = new Vector3(1.25f, 1f, 1f);
		LeanTween.scaleX(this.leftboundsglow, 0.001f, 0.25f).setEaseOutQuad();
	//	LeanTween.cancel(this.healthmask.transform.parent.gameObject);
	//	this.healthmask.transform.parent.gameObject.transform.localScale = new Vector3(0.69f, 0.69f, 1f);
	//	LeanTween.scale(this.healthmask.transform.parent.gameObject, new Vector3(0.6f, 0.6f, 1f), 0.12f).setEaseOutQuart();
	}

	private void playNote()
	{
		float num = 9999f;
		int num2 = 0;
		for (int i = 0; i < 15; i++)
		{
			float num3 = Mathf.Abs(this.notelinepos[i] - this.pointer.transform.localPosition.y);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		Debug.Log("closest index: " + num2);
		this.notestartpos = this.notelinepos[num2];
		this.currentnotesound.clip = this.trombclips.tclips[Mathf.Abs(num2 - 14)];
		this.currentnotesound.Play();
	}

	private void animatePlayerDot()
	{
		this.pointercolorindex++;
		if (this.pointercolorindex > 3)
		{
			this.pointercolorindex = 0;
		}
		if (this.pointercolorindex == 0)
		{
			this.pointerimg.color = new Color32(byte.MaxValue, 54, 92, byte.MaxValue);
		}
		if (this.pointercolorindex == 1)
		{
			this.pointerimg.color = new Color32(35, 194, 201, byte.MaxValue);
		}
		if (this.pointercolorindex == 2)
		{
			this.pointerimg.color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
		}
		if (this.pointercolorindex == 3)
		{
			this.pointerimg.color = new Color32(byte.MaxValue, 171, 0, byte.MaxValue);
		}
	}

	private void activateNextNote(int noteindex)
	{
		if (noteindex > 0)
		{
			this.allnotes[noteindex - 1].SetActive(false);
		}
		if (this.beatstoshow < this.allnotes.Count - 1)
		{
			this.beatstoshow++;
			this.allnotes[this.beatstoshow].SetActive(true);
		}
	}

	private void stopNote()
	{
		this.currentnotesound.Stop();
	}


	private void Update()
	{
		//if (this.multtexthide > -1f)
		//{
		//	this.multtexthide += 1f * Time.deltaTime;
		//	if (this.multtexthide > 1.5f)
		//	{
		//		this.multtexthide = -1f;
		//		this.hideMultText();
		//	}
		//}
		float num = 3.68f;
		float num2 = -4.4f;
		//float num3 = this.currenthealth / 100f;
		//float x = this.healthfill.transform.localPosition.x;
		//float num4 = (num2 + num3 * num - x) * (6.85f * Time.deltaTime);
		//this.healthposy += 13.5f * Time.deltaTime;
		//if (this.healthposy > 3.08f)
		//{
		//	this.healthposy = -2f;
		//}
		//this.healthfill.transform.localPosition = new Vector3(x + num4, this.healthposy, 0f);
		//if (this.currenthealth <= 0.01f)
		//{
		//	this.healthzerotimer += Time.deltaTime;
		//	if (this.healthzerotimer > 5f)
		//	{
		//	}
		//}
		//else if (this.healthzerotimer > 0f && this.currenthealth > 0.01f)
		//{
		//	this.healthzerotimer = 0f;
		//}



		if (!this.freeplay || this.playingineditor)
		{
			float num8 = this.musictrack.time - this.latency_offset - this.noteoffset;
			if (this.musictrack.time > this.levelendtime && this.levelendtime > 0f && !this.level_finshed && !this.quitting)
			{
				this.level_finshed = true;
				Debug.Log("====== LEVEL DONE! ======");
				//this.curtainc.closeCurtain(true);
				GlobalVariables.gameplay_scoretotal = this.totalscore;
				GlobalVariables.gameplay_scoreperc = (float)this.totalscore / (float)this.maxlevelscore;
				Debug.Log(string.Concat(new object[]
				{
					"score percentage: ",
					GlobalVariables.gameplay_scoreperc,
					"(",
					this.totalscore,
					"/",
					this.maxlevelscore,
					")"
				}));
				GlobalVariables.gameplay_notescores = new int[]
				{
					this.scores_F,
					this.scores_D,
					this.scores_C,
					this.scores_B,
					this.scores_A
				};
			}
			if (this.musictrack.time > this.tempotimer)
			{
				this.tempotimer = 60f / this.tempo * (float)(this.beatnum + 1);
				this.beatnum++;
				this.timesigcount++;
				if (this.timesigcount > this.beatspermeasure)
				{
					this.timesigcount = 1;
					//this.bgcontroller.tickbg();
					if (this.beatspermeasure == 3)
					{
						this.flashLeftBounds();
					}
				}
				if (this.beatspermeasure != 3)
				{
					this.flashLeftBounds();
				}
				if (this.breathscale == 1f)
				{
					this.breathscale = 0.75f;
				}
				else
				{
					this.breathscale = 1f;
				}
			}
			if (this.musictrack.time > this.tempotimerdot)
			{
				this.beatnumdot++;
				this.tempotimerdot = 60f / (this.tempo * 4f) * (float)this.beatnumdot;
				this.animatePlayerDot();
			}
			Vector3 anchoredPosition3D = this.noteholderr.anchoredPosition3D;
			this.noteholderr.anchoredPosition3D = new Vector3(this.zeroxpos + num8 * -this.trackmovemult, 0f, 0f);
			this.lyricsholderr.anchoredPosition3D = new Vector3(this.zeroxpos + num8 * -this.trackmovemult, 0f, 0f);
			
			//if (!this.leveleditor)
			{
				if (this.noteholderr.anchoredPosition3D.x - this.zeroxpos + this.allbeatlines[this.beatlineindex].anchoredPosition3D.x < 0f)
				{
					this.maxbeatlinex += (float)(this.defaultnotelength * this.beatspermeasure);
					this.allbeatlines[this.beatlineindex].anchoredPosition3D = new Vector3(this.maxbeatlinex, 0f, 0f);
					this.beatlineindex++;
					if (this.beatlineindex > this.numbeatlines - 1)
					{
						this.beatlineindex = 0;
					}
				}
				//if (this.bgindex < this.bgdata.Count && num8 > this.bgdata[this.bgindex][0])
				//{
				//	Debug.Log("WAH!");
				//	this.bgcontroller.bgMove((int)this.bgdata[this.bgindex][1]);
				//	this.bgindex++;
				//}
			}
		}
		bool flag = false;
		if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.B) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.O) || Input.GetKey(KeyCode.P) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.U) || Input.GetKey(KeyCode.V) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)) && !this.quitting && !this.enteringlyrics)
		{
			flag = true;
		}
		if (!this.freeplay)
		{
			this.scorecounter += Time.deltaTime;
			if (this.scorecounter > 0.01f)
			{
				this.scorecounter = 0f;
				//this.tallyScore();
			}
		}
		//if (!this.leveleditor)
		{
			float num9 = this.noteholderr.anchoredPosition3D.x - this.zeroxpos;
			if (num9 > 0f)
			{
				num9 = -1f;
			}
			else
			{
				num9 = Mathf.Abs(num9);
			}
			if (num9 > this.currentnotestart && !this.noteactive)
			{
				this.noteactive = true;
				Debug.Log("note " + this.currentnoteindex + " START");
				if (this.currentnoteindex > 0 && this.allnotevals[this.currentnoteindex - 1][1] != this.currentnotestart && !this.released_button_between_notes)
				{
					Debug.Log("PLAYER DIDN'T RELEASE BUTTON BETWEEN NOTES!!");
					this.multiplier = 0;
					this.highestcombocounter = 0;
					//this.affectHealthBar(-15f);
				}
			}
			else if (num9 > this.currentnoteend && this.noteactive)
			{
				Debug.Log("note " + this.currentnoteindex + " END");
				this.activateNextNote(this.currentnoteindex);
				this.note_end_timer = this.max_note_end_timer;
				this.noteactive = false;
				this.released_button_between_notes = false;
				if (!this.freeplay)
				{
					//this.getScoreAverage();
					this.grabNoteRefs(1);
				}
			}
			if (this.noteactive && !this.freeplay)
			{
				float num10 = (this.currentnoteend - num9) / (this.currentnoteend - this.currentnotestart);
				num10 = Mathf.Abs(1f - num10);
				float num11 = this.easeInOutVal(num10, 0f, this.currentnotepshift, 1f);
				float f = this.pointerrect.anchoredPosition.y - (this.currentnotestarty + num11);
				float num12 = 100f - Mathf.Abs(f);
				if (!this.noteplaying)
				{
					num12 = 0f;
				}
				if (this.notescoreaverage == -1f)
				{
					this.notescoreaverage = num12;
					Debug.Log("FIRST SAMPLE SCOREAVG: " + this.notescoreaverage);
				}
				else
				{
					this.notescoresamples += 1f;
					float num13 = 1f - 1f / this.notescoresamples;
					float num14 = 1f / this.notescoresamples;
					this.notescoreaverage = this.notescoreaverage * num13 + num12 * num14;
				}
			}
			else if (!this.noteactive && !this.released_button_between_notes && !flag)
			{
				this.released_button_between_notes = true;
			}
		}
		if (this.playingineditor && !this.controllermode && !this.autoplay)
		{
			float num15 = Input.mousePosition.y / (float)Screen.height;
			if (num15 < 0f)
			{
				num15 = 0f;
			}
			else if (num15 > 1f)
			{
				num15 = 1f;
			}
			num15 -= 0.5f;
			num15 *= this.mousemult;//* GlobalVariables.localsettings.sensitivity;
			if (GlobalVariables.mousecontrolmode == 1)
			{
				num15 *= -1f;
			}
			if (!this.paused)
			{
				//this.puppet_humanc.doPuppetControl(num15 * 2f);
				//this.puppet_humanc.vibrato = this.vibratoamt;
			}
			Vector3 vector = new Vector3(this.zeroxpos - (float)this.dotsize * 0.5f, num15 * this.vsensitivity, 0f);
			if (vector.y > this.vbounds + this.outerbuffer)
			{
				vector.y = this.vbounds + this.outerbuffer;
			}
			else if (vector.y < -this.vbounds - this.outerbuffer)
			{
				vector.y = -this.vbounds - this.outerbuffer;
			}
			Vector3 localPosition = this.pointer.transform.localPosition;
			Vector3 b = (vector - localPosition) * this.movesmoothing * Time.deltaTime;
			this.pointer.transform.localPosition = localPosition + b;
		}
		if (Input.GetKey(KeyCode.Escape) && !this.quitting && this.musictrack.time > 0.5f && !this.freeplay)
		{
			this.musictrack.Pause();
			//this.sfxrefs.backfromfreeplay.Play();
			//this.curtainc.closeCurtain(false);
			this.paused = true;
			this.quitting = true;
			//this.pausecanvas.SetActive(true);
			//this.pausecontroller.showPausePanel();
		}
		else if (Input.GetKey(KeyCode.Escape) && !this.quitting && this.freeplay )//&& this.curtainc.doneanimating)
		{
			//this.sfxrefs.backfromfreeplay.Play();
			//this.curtainc.closeCurtain(true);
			this.paused = true;
			this.quitting = true;
		}
		if (!this.controllermode)
		{
			float num16 = Input.mousePosition.y / (float)Screen.height;
			if (flag && !this.noteplaying && num16 < 0.95f && !this.outofbreath && this.readytoplay)
			{
				this.startmouseX = Input.mousePosition.x;
				this.noteplaying = true;
				//this.setPuppetShake(true);
				this.currentnotesound.time = 0f;
				this.playNote();
			}
			else if (!flag && this.noteplaying && !this.autoplay)
			{
				this.noteplaying = false;
				//this.setPuppetShake(false);
				this.currentnotesound.time = 0f;
				this.stopNote();
			}
		}
		if (this.noteplaying)
		{
			if (this.currentnotesound.time > this.currentnotesound.clip.length - 1.25f)
			{
				this.currentnotesound.time = 1f;
			}
			float num17 = Mathf.Pow(this.notestartpos - this.pointer.transform.localPosition.y, 2f) * 6.8E-06f;
			float num18 = (this.notestartpos - this.pointer.transform.localPosition.y) * (1f + num17);
			if (num18 > 0f)
			{
				num18 = (this.notestartpos - this.pointer.transform.localPosition.y) * 1.392f;
				num18 *= 0.5f;
			}
			this.currentnotesound.pitch = 1f - num18 * this.pitchamount;
			if (this.currentnotesound.pitch > 2f)
			{
				this.currentnotesound.pitch = 2f;
			}
			else if (this.currentnotesound.pitch < 0.5f)
			{
				this.currentnotesound.pitch = 0.5f;
			}
		}
		if (this.noteplaying)
		{
			if (this.breathcounter < 1f)
			{
				this.breathcounter += Time.deltaTime * 0.22f;
				if (this.breathcounter > 1f)
				{
					this.breathcounter = 1f;
					Debug.Log("OUT OF BREATH");
					//this.sfxrefs.outofbreath.Play();
					//this.breathglow.anchoredPosition3D = new Vector3(-380f, 0f, 0f);
					this.outofbreath = true;
					this.noteplaying = false;
					//this.setPuppetShake(false);
					//this.setPuppetBreath(true);
					this.stopNote();
				}
			}
		}
		else if (!this.noteplaying && this.breathcounter > 0f)
		{
			if (!this.outofbreath)
			{
				this.breathcounter -= Time.deltaTime * 8.5f;
			}
			else if (this.outofbreath)
			{
				this.breathcounter -= Time.deltaTime * 0.29f;
				//this.breathglow.anchoredPosition3D = new Vector3(-380f + (this.breathcounter - 1f) * 100f, 0f, 0f);
			}
			if (this.breathcounter < 0f)
			{
				this.breathcounter = 0f;
				if (this.outofbreath)
				{
					this.outofbreath = false;
					//this.setPuppetBreath(false);
					Debug.Log("breath back");
				}
			}
		}

		//float x2 = 37f - 72f * this.breathcounter;
		//float num19 = this.topbreathr.anchoredPosition3D.y;
		//num19 += (this.breathcounter + 0.5f) * 3f;
		//if (num19 > -85f)
		//{
		//	num19 = -141f;
		//}
		//float num20 = this.bottombreathr.anchoredPosition3D.y;
		//num20 -= (this.breathcounter + 0.5f) * 3f;
		//if (num20 < -42f)
		//{
		//	num20 = 14f;
		//}
		//this.topbreathr.anchoredPosition3D = new Vector3(x2, num19, 0f);
		//this.bottombreathr.anchoredPosition3D = new Vector3(x2, num20, 0f);
		//if (this.breathcounter != 0f)
		//{
		//	float num21 = this.breathcounter;
		//}
	}

}