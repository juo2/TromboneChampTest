using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
//using UnityEngine.PostProcessing;
using UnityEngine.UI;

// Token: 0x02000020 RID: 32
public class GameController : MonoBehaviour
{
	// Token: 0x060001AC RID: 428 RVA: 0x00015C50 File Offset: 0x00013E50
	private void Start()
	{
		this.latency_offset = 0;//(float)GlobalVariables.localsettings.latencyadjust * 0.001f;
		Debug.Log("latency_offset: " + this.latency_offset);
		Application.targetFrameRate = 144;
		//Cursor.lockState = CursorLockMode.Confined;
		this.retrying = false;
		this.notescoreaverage = -1f;
		this.notescoresamples = 1f;
		this.curtains.SetActive(true);
		//this.curtainc = this.curtains.GetComponent<CurtainController>();
		this.level_finshed = false;
		this.scores_A = 0;
		this.scores_B = 0;
		this.scores_C = 0;
		this.scores_D = 0;
		this.scores_F = 0;
		if (GlobalVariables.scene_destination == "freeplay")
		{
			this.freeplay = true;
			this.backbtn.SetActive(true);
			//this.champcontroller.hideChampText();
		}
		else
		{
			this.backbtn.SetActive(false);
		}
		this.puppetnum = GlobalVariables.chosen_character;
		this.textureindex = GlobalVariables.chosen_trombone;
		this.levelnum = GlobalVariables.chosen_track_index;
		this.soundset = GlobalVariables.chosen_soundset;
		this.popuptextobj.transform.localScale = new Vector3(0f, 1f, 1f);
		this.multtextobj.transform.localScale = new Vector3(0f, 1f, 1f);
		this.ui_savebtn.onClick.AddListener(new UnityAction(this.tryToSaveLevel));
		this.ui_loadbtn.onClick.AddListener(new UnityAction(this.loadFromEditor));
		//BloomModel.Settings settings = this.gameplayppp.bloom.settings;
		//settings.bloom.intensity = 0f;
		//this.gameplayppp.bloom.settings = settings;
		if (!this.freeplay)
		{
			this.songtitle.text = GlobalVariables.data_tracktitles[this.levelnum][0];
			this.songtitleshadow.text = GlobalVariables.data_tracktitles[this.levelnum][0];
		}
		else if (this.freeplay)
		{
			this.songtitle.text = "";
			this.songtitleshadow.text = "";
			this.ui_score.text = "";
			this.ui_score_shadow.text = "";
			this.highestcombo.text = "";
			this.highestcomboshad.text = "";
		}
		string text = "/StreamingAssets/trackassets/";
		if (!this.freeplay)
		{
			text += GlobalVariables.data_trackrefs[this.levelnum];
		}
		else if (this.freeplay)
		{
			text += "freeplay";
		}
		this.myLoadedAssetBundle = AssetBundle.LoadFromFile(Application.dataPath + text);
		if (this.myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			return;
		}
		Debug.Log("LOADED ASSETBUNDLE: " + Application.dataPath + text);
		if (!this.freeplay)
		{
			AudioSource component = this.myLoadedAssetBundle.LoadAsset<GameObject>("music_" + GlobalVariables.data_trackrefs[this.levelnum]).GetComponent<AudioSource>();
			this.musictrack.clip = component.clip;
			this.musictrack.volume = component.volume;
		}
		//base.StartCoroutine(this.loadAssetBundleResources());
		//this.bgcontroller.songname = GlobalVariables.data_trackrefs[this.levelnum];
		GameObject gameObject = new GameObject();
		if (!this.freeplay)
		{
			gameObject = this.myLoadedAssetBundle.LoadAsset<GameObject>("BGCam_" + GlobalVariables.data_trackrefs[this.levelnum]);
		}
		else if (this.freeplay)
		{
			gameObject = this.myLoadedAssetBundle.LoadAsset<GameObject>("BGCam_freeplay");
		}
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, this.bgholder.transform);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			//this.bgcontroller.fullbgobject = gameObject2;
		}
		if (this.soundset > 0)
		{
			this.currentnotesound.volume = 0.25f;
		}
		if (this.soundset == 4)
		{
			this.currentnotesound.volume = 0.75f;
		}
		string[] array = new string[]
		{
			"default",
			"slidewhistle",
			"eightbit",
			"club",
			"fart"
		};
		this.mySoundAssetBundle = AssetBundle.LoadFromFile(Application.dataPath + "/StreamingAssets/soundpacks/soundpack" + array[this.soundset]);
		if (this.mySoundAssetBundle == null)
		{
			Debug.Log("Failed to load sound pack AssetBundle!");
			return;
		}
		Debug.Log("LOADED <<<sound pack>>> ASSETBUNDLE");
		UnityEngine.Object.Instantiate<GameObject>(this.mySoundAssetBundle.LoadAsset<GameObject>("soundpack" + array[this.soundset]), new Vector3(0f, 0f, 0f), Quaternion.identity, this.soundSets.transform);
		base.StartCoroutine(this.loadSoundBundleResources());
		this.puppet_human = UnityEngine.Object.Instantiate<GameObject>(this.playermodels[this.puppetnum], new Vector3(0f, 0f, 0f), Quaternion.identity, this.modelparent.transform);
		this.puppet_human.transform.localPosition = new Vector3(0.7f, -0.38f, 1.3f);
		if (!this.leveleditor && !this.freeplay)
		{
			LeanTween.scaleY(this.puppet_human, 0.01f, 0.01f);
		}
		if (this.freeplay)
		{
			LeanTween.moveLocalX(this.puppet_human, 0.4f, 0.01f);
		}
		
		//this.puppet_humanc = this.puppet_human.GetComponent<HumanPuppetController>();
		
		//if (GlobalVariables.localsave.cardcollectionstatus[36] > 9)
		{
			//this.puppet_humanc.show_rainbow = true;
		}
		//this.puppet_humanc.setTromboneTex(this.textureindex);
		this.topbreathr = this.topbreath.GetComponent<RectTransform>();
		this.bottombreathr = this.bottombreath.GetComponent<RectTransform>();
		this.noteholderr = this.noteholder.GetComponent<RectTransform>();
		this.lyricsholderr = this.lyricsholder.GetComponent<RectTransform>();
		this.noteparticlesrect = this.noteparticles.transform.GetComponent<RectTransform>();
		this.leftboundsglow.transform.localScale = new Vector3(0.01f, 1f, 1f);
		for (int i = 0; i < 15; i++)
		{
			this.notelines[i] = this.notelinesholder.transform.GetChild(i).gameObject;
		}
		for (int j = 0; j < 8; j++)
		{
			GameObject gameObject3 = this.notelines[j].gameObject;
			float num = this.vbounds / 12f;
			if (j == 0)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds, 0f);
			}
			else if (j == 1)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num, 0f);
			}
			else if (j == 2)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 3f, 0f);
			}
			else if (j == 3)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 5f, 0f);
			}
			else if (j == 4)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 7f, 0f);
			}
			else if (j == 5)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 8f, 0f);
			}
			else if (j == 6)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 10f, 0f);
			}
			else if (j == 7)
			{
				gameObject3.transform.localPosition = new Vector3(0f, this.vbounds - num * 12f, 0f);
			}
		}
		for (int k = 0; k < 7; k++)
		{
			GameObject gameObject4 = this.notelines[k + 8].gameObject;
			float num2 = this.vbounds / 12f;
			if (k == 0)
			{
				gameObject4.transform.localPosition = new Vector3(0f, -num2, 0f);
			}
			else if (k == 1)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -3f, 0f);
			}
			else if (k == 2)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -5f, 0f);
			}
			else if (k == 3)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -7f, 0f);
			}
			else if (k == 4)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -8f, 0f);
			}
			else if (k == 5)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -10f, 0f);
			}
			else if (k == 6)
			{
				gameObject4.transform.localPosition = new Vector3(0f, num2 * -12f, 0f);
			}
		}
		for (int l = 0; l < 15; l++)
		{
			this.notelinepos[l] = this.notelines[l].gameObject.transform.localPosition.y;
			this.notelines[l] = null;
			UnityEngine.Object.Destroy(this.notelines[l]);
		}
		this.pointerrect = this.pointer.GetComponent<RectTransform>();
		this.pointerrect.anchoredPosition3D = new Vector3(this.zeroxpos - (float)this.dotsize * 0.5f, 0f, 0f);
		this.noteparticlesrect.anchoredPosition3D = new Vector3(this.zeroxpos, 0f, 0f);
		this.leftbounds.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(this.zeroxpos, 60f, 0f);
		if (!this.leveleditor && !this.freeplay)
		{
			this.buildLevel(this.levelnum);
			this.trackmovemult = this.tempo / 60f * (float)this.defaultnotelength;
			float num3 = this.zeroxpos - this.noteoffset * -this.trackmovemult;
			LeanTween.value(num3 + 1000f, num3, 1.5f).setEaseInOutQuad().setOnUpdate(delegate (float val)
			{
				this.noteholderr.anchoredPosition3D = new Vector3(val, 0f, 0f);
				this.lyricsholderr.anchoredPosition3D = new Vector3(val, 0f, 0f);
			});
		}
		if (!this.leveleditor)
		{
			this.editorcanvas.SetActive(false);
		}
		else if (this.leveleditor)
		{
			this.readytoplay = true;
			this.editorcanvas.SetActive(true);
			this.buildEditorGUI();
		}
		if (this.freeplay)
		{
			this.editorcanvas.SetActive(false);
			this.healthobj.SetActive(false);
		}
		this.pointer.transform.SetAsLastSibling();
		if (!this.freeplay && !this.leveleditor)
		{
			this.musictrack = this.musicref.GetComponent<AudioSource>();
			this.startSong();
			return;
		}
		if (this.freeplay)
		{
			this.tempo = 40f;
			base.Invoke("startDance", 1f);
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0001680C File Offset: 0x00014A0C
	private IEnumerator loadAssetBundleResources()
	{
		yield return 0;
		if (this.freeplay)
		{
			//this.bgcontroller.songname = "freeplay";
		}
		//this.bgcontroller.setUpBGControllerRefs();
		yield break;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0001681B File Offset: 0x00014A1B
	private IEnumerator loadSoundBundleResources()
	{
		yield return 0;
		this.trombclips = this.currentnotesound.gameObject.transform.GetChild(0).gameObject.GetComponent<AudioClipsTromb>();
		this.fixAudioMixerStuff();
		yield break;
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0001682A File Offset: 0x00014A2A
	private void fixAudioMixerStuff()
	{
		this.musictrack.outputAudioMixerGroup = this.audmix_bgmus;
		this.audmix.SetFloat("mastervol", 60f);//(GlobalVariables.localsettings.maxvolume - 1f) * 60f);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00016864 File Offset: 0x00014A64
	public void unloadBundles()
	{
		this.myLoadedAssetBundle.Unload(true);
		this.mySoundAssetBundle.Unload(true);
		this.myLoadedAssetBundle = null;
		this.mySoundAssetBundle = null;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0001688C File Offset: 0x00014A8C
	public void backFromFreeplay()
	{
		//this.sfxrefs.backfromfreeplay.Play();
		this.closeCurtains();
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x000168A4 File Offset: 0x00014AA4
	private void closeCurtains()
	{
		//LeanTween.cancelAll();
		//this.curtainc.closeCurtain(true);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x000168B7 File Offset: 0x00014AB7
	private void loadFromEditor()
	{
		this.tryToLoadLevel("EDITOR");
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x000168C4 File Offset: 0x00014AC4
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

	// Token: 0x060001B5 RID: 437 RVA: 0x00016918 File Offset: 0x00014B18
	private void startSong()
	{
		this.tempotimer = 60f / (this.tempo * (float)this.beatspermeasure);
		this.tempotimerdot = 60f / this.tempo;
		base.Invoke("playsong", 2.5f);
		base.Invoke("startDance", 2.5f);
		this.calcMaxScore();
		if (this.puppet_human)
		{
			LeanTween.scaleY(this.puppet_human, 1f, 0.5f).setEaseOutBounce().setDelay(2f);
		}
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x000169AC File Offset: 0x00014BAC
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

	// Token: 0x060001B7 RID: 439 RVA: 0x00016A64 File Offset: 0x00014C64
	private void startDance()
	{
		this.readytoplay = true;
		float num = this.tempo;
		if (this.tempo > 90f)
		{
			num *= 0.5f;
		}
		if (this.beatspermeasure == 3)
		{
			num *= 0.6666667f;
		}
		//this.puppet_humanc.startPuppetBob(num);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00016AB1 File Offset: 0x00014CB1
	private void playsong()
	{
		this.readytoplay = true;
		this.musictrack.Play();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x00016AC8 File Offset: 0x00014CC8
	private void buildEditorGUI()
	{
		this.editorpostext.text = this.editorposition.ToString();
		this.changeEditorTime(0);
		this.changeEditorTempo(0);
		this.changeLineSpacing(false, 0);
		this.changeTimeSig(0);
		this.noteholderr.anchoredPosition3D = new Vector3(this.zeroxpos, 0f, 0f);
		for (int i = 0; i < 500; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.beatline, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform);
			gameObject.name = ("bl" + i).ToString();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			this.alleditorbeatlines[i] = component;
			component.anchoredPosition3D = new Vector3((float)((i + 1) * this.defaultnotelength * this.beatspermeasure), 0f, 0f);
			if (component.anchoredPosition3D.x > this.maxbeatlinex)
			{
				this.maxbeatlinex = component.anchoredPosition3D.x;
			}
		}
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00016BE4 File Offset: 0x00014DE4
	private void buildAllLyrics()
	{
		for (int i = 0; i < this.lyricdata_pos.Count; i++)
		{
			this.enterLyric(this.lyricdata_pos[i][0], this.lyricdata_pos[i][1], this.lyricdata_txt[i], false);
		}
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00016C38 File Offset: 0x00014E38
	private void buildNotes()
	{
		int num = this.levelnum;
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
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.singlenote, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform);
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
			if (i > this.beatstoshow && !this.leveleditor)
			{
				gameObject2.SetActive(false);
			}
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00016F4C File Offset: 0x0001514C
	private void resetNoteColor()
	{
		for (int i = 0; i < this.leveldata.Count; i++)
		{
			float[] array = this.leveldata[i];
			this.allnotes[i].GetComponent<NoteDesigner>().setColorScheme(float.Parse(this.col_r_1.text), float.Parse(this.col_g_1.text), float.Parse(this.col_b_1.text), float.Parse(this.col_r_2.text), float.Parse(this.col_g_2.text), float.Parse(this.col_b_2.text));
		}
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00016FF8 File Offset: 0x000151F8
	private void repositionNotes()
	{
		for (int i = 0; i < this.leveldata.Count; i++)
		{
			float[] array = this.leveldata[i];
			GameObject gameObject = this.allnotes[i];
			RectTransform component = gameObject.GetComponent<RectTransform>();
			RectTransform component2 = gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
			component.anchoredPosition3D = new Vector3(array[0] * (float)this.defaultnotelength, array[2], 0f);
			component2.anchoredPosition3D = new Vector3((float)this.defaultnotelength * array[1] - (float)this.levelnotesize, array[3], 0f);
			float[] item = new float[]
			{
				array[0] * (float)this.defaultnotelength,
				array[0] * (float)this.defaultnotelength + (float)this.defaultnotelength * array[1],
				array[2],
				array[3],
				array[4]
			};
			this.allnotevals.Add(item);
			LineRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<LineRenderer>();
			float num = (float)this.defaultnotelength * array[1];
			float c = array[3];
			foreach (LineRenderer lineRenderer in componentsInChildren)
			{
				lineRenderer.SetPosition(0, new Vector3(0f, 0f, 0f));
				for (int k = 1; k < 10; k++)
				{
					lineRenderer.SetPosition(k, new Vector3(num / 9f * (float)k, this.easeInOutVal((float)k, 0f, c, 9f), 0f));
				}
			}
		}
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00017184 File Offset: 0x00015384
	private void buildLevel(int levelnum)
	{
		this.trackmovemult = this.tempo / 60f * (float)this.defaultnotelength;
		this.tryToLoadLevel(GlobalVariables.data_trackrefs[levelnum]);
		this.currentnoteindex = 0;
		Debug.Log("making " + this.numbeatlines + " beat lines");
		for (int i = 0; i < this.numbeatlines; i++)
		{
			Debug.Log("beat line " + i);
			RectTransform component = UnityEngine.Object.Instantiate<GameObject>(this.beatline, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform).GetComponent<RectTransform>();
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

	// Token: 0x060001BF RID: 447 RVA: 0x000172C4 File Offset: 0x000154C4
	private void getScoreAverage()
	{
		float num = this.notescoreaverage;
		this.notescoresamples = 1f;
		this.notescoreaverage = -1f;
		float num2 = Mathf.Floor(this.leveldata[this.currentnoteindex][1] * 10f);
		float num3 = 0f;
		//if (this.rainbowcontroller.champmode)
		//{
		//	num3 = 1.5f;
		//}
		float num4 = Mathf.Floor(num2 * num * (((float)this.multiplier + num3) * 0.1f + 1f)) * 10f;
		float num5 = (num - 79f) * 0.2f;
		float num6 = Mathf.Floor(num2 * 100f * ((float)this.multiplier * 0.1f + 1f)) * 10f;
		Debug.Log(string.Concat(new object[]
		{
			"AVG: ",
			num,
			" / TOTAL: ",
			num4,
			" / MAX: ",
			num6,
			" / HLTHCHNG: ",
			num5,
			" / CHAMPMODE?: ",
			//this.rainbowcontroller.champmode.ToString()
		}));
		this.affectHealthBar(num5);
		if (num > 95f)
		{
			this.doScoreText(4);
		}
		else if (num > 88f)
		{
			this.doScoreText(3);
		}
		else if (num > 79f)
		{
			this.doScoreText(2);
		}
		else if (num > 70f)
		{
			this.doScoreText(1);
		}
		else
		{
			this.doScoreText(0);
		}
		this.totalscore += Mathf.FloorToInt(num4);
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0001745C File Offset: 0x0001565C
	private void affectHealthBar(float healthchange)
	{
		if (this.currenthealth < 100f && this.currenthealth + healthchange >= 100f)
		{
			this.maxScoreEffect(true);
			this.playGoodSound();
		}
		else if (this.currenthealth >= 100f && this.currenthealth + healthchange < 100f)
		{
			this.maxScoreEffect(false);
			this.currenthealth = 0f;
			//this.sfxrefs.slidedown.Play();
		}
		this.currenthealth += healthchange;
		if (this.currenthealth > 100f)
		{
			this.currenthealth = 100f;
		}
		else if (this.currenthealth < 0f)
		{
			this.currenthealth = 0f;
		}
		//int num = Mathf.FloorToInt(this.currenthealth * 0.1f) - this.champcontroller.healthcounter;
		//for (int i = 0; i < Mathf.Abs(num); i++)
		//{
		//	if (num > 0)
		//	{
		//		this.champcontroller.advanceCounter(1);
		//	}
		//	else if (num < 0)
		//	{
		//		this.champcontroller.advanceCounter(-1);
		//	}
		//}
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00017564 File Offset: 0x00015764
	private void maxScoreEffect(bool entering)
	{
		if (entering)
		{
			//this.rainbowcontroller.startRainbowLess();
			LeanTween.value(30f, 0f, 2f).setEaseInOutQuad().setOnUpdate(delegate (float val)
			{
				//BloomModel.Settings settings = this.gameplayppp.bloom.settings;
				//settings.bloom.intensity = val;
				//this.gameplayppp.bloom.settings = settings;
			});
			return;
		}
		if (!entering)
		{
			//this.rainbowcontroller.stopRainbow();
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x000175BC File Offset: 0x000157BC
	private void doScoreText(int whichtext)
	{
		string text = "";
		if (whichtext == 4)
		{
			this.scores_A++;
			text = "PERFECTO";
		}
		else if (whichtext == 3)
		{
			this.scores_B++;
			text = "NICE";
		}
		else if (whichtext == 2)
		{
			this.scores_C++;
			text = "OK";
		}
		else if (whichtext == 1)
		{
			this.scores_D++;
			text = "MEH";
		}
		else if (whichtext == 0)
		{
			this.scores_F++;
			text = "NASTY";
		}
		if (whichtext > 2)
		{
			this.multiplier++;
			this.highestcombocounter++;
			if (this.highestcombocounter > this.highestcombo_level)
			{
				this.updateHighestCombo(this.highestcombocounter);
			}
			if (this.multiplier > this.max_multiplier)
			{
				this.multiplier = this.max_multiplier;
			}
		}
		else
		{
			this.multiplier = 0;
			this.highestcombocounter = 0;
		}
		this.animateOutNote(this.currentnoteindex, whichtext);
		this.popuptext.text = text;
		this.popuptextshadow.text = text;
		this.multtexthide = 0f;
		if (this.multiplier > 0 && this.multiplier < this.max_multiplier)
		{
			this.multtext.text = this.multiplier.ToString() + "x";
			this.multtextshadow.text = this.multiplier.ToString() + "x";
		}
		else if (this.multiplier == this.max_multiplier)
		{
			this.multtext.text = this.multiplier.ToString() + "x (MAX)";
			this.multtextshadow.text = this.multiplier.ToString() + "x (MAX)";
		}
		else
		{
			this.multtext.text = "";
			this.multtextshadow.text = "";
		}
		this.popuptextobj.transform.localScale = new Vector3(0f, 1f, 1f);
		this.multtextobj.transform.localScale = new Vector3(0f, 1f, 1f);
		LeanTween.scale(this.popuptextobj, new Vector3(1f, 1f, 1f), 0.1f).setEaseOutQuad();
		LeanTween.scale(this.multtextobj, new Vector3(1f, 1f, 1f), 0.1f).setEaseOutQuad();
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x00017848 File Offset: 0x00015A48
	private void hideMultText()
	{
		LeanTween.scale(this.popuptextobj, new Vector3(0f, 1f, 1f), 0.05f).setEaseOutQuad();
		LeanTween.scale(this.multtextobj, new Vector3(0f, 1f, 1f), 0.05f).setEaseOutQuad();
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x000178AC File Offset: 0x00015AAC
	private void updateHighestCombo(int thecombo)
	{
		this.highestcombo_level = thecombo;
		LeanTween.cancel(this.highestcomboobj);
		this.highestcomboobj.transform.localScale = new Vector3(1.5f, 1.2f, 1f);
		LeanTween.scale(this.highestcomboobj, new Vector3(1f, 1f, 1f), 0.1f).setEaseOutQuart();
		this.highestcombo.text = "longest combo: <color=#ffff00>" + thecombo.ToString() + "</color>";
		this.highestcomboshad.text = "longest combo: " + thecombo.ToString();
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x00017956 File Offset: 0x00015B56
	private void playGoodSound()
	{
		//this.sfxrefs.ahorn5.Play();
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00017968 File Offset: 0x00015B68
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

	// Token: 0x060001C7 RID: 455 RVA: 0x00017A04 File Offset: 0x00015C04
	private void tallyScore()
	{
		if (this.currentscore < this.totalscore)
		{
			this.currentscore += 503;
		}
		if (this.currentscore > this.totalscore)
		{
			this.currentscore = this.totalscore;
		}
		this.ui_score.text = this.currentscore.ToString("n0");
		this.ui_score_shadow.text = this.currentscore.ToString("n0");
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00017A84 File Offset: 0x00015C84
	private void moveCursor(int dir)
	{
		this.editornoteindex -= dir;
		if (this.editornoteindex > 12)
		{
			this.editornoteindex = -12;
		}
		else if (this.editornoteindex < -12)
		{
			this.editornoteindex = 12;
		}
		float num = this.vbounds / 12f;
		this.pointer.transform.localPosition = new Vector3(this.zeroxpos - (float)this.dotsize * 0.5f, num * (float)this.editornoteindex, 0f);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00017B09 File Offset: 0x00015D09
	public void toggleEditorAutoplay()
	{
		if (!this.autoplay)
		{
			this.autoplay = true;
			this.autoplaytxt.text = "Autoplay ON";
			return;
		}
		this.autoplay = false;
		this.autoplaytxt.text = "Autoplay OFF";
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00017B44 File Offset: 0x00015D44
	private void changeEditorTempo(int dir)
	{
		this.tempo += (float)dir;
		if (this.tempo < 10f)
		{
			this.tempo = 10f;
		}
		this.editortempotext.text = "tempo: " + this.tempo.ToString();
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00017B98 File Offset: 0x00015D98
	private void changeTimeSig(int dir)
	{
		this.beatspermeasure += dir;
		if (this.beatspermeasure < 1)
		{
			this.beatspermeasure = 4;
		}
		this.editortsigtext.text = "t_sig: " + this.beatspermeasure.ToString();
		Debug.Log("yeaaa");
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00017BF0 File Offset: 0x00015DF0
	private void changeLineSpacing(bool reposition, int dir)
	{
		this.defaultnotelength += dir * 10;
		if (this.defaultnotelength < 20)
		{
			this.defaultnotelength = 20;
		}
		else if (this.defaultnotelength > 900)
		{
			this.defaultnotelength = 900;
		}
		this.editorspacingtext.text = "spacing: " + this.defaultnotelength.ToString();
		if (reposition)
		{
			for (int i = 0; i < 500; i++)
			{
				this.alleditorbeatlines[i].anchoredPosition3D = new Vector3((float)((i + 1) * this.defaultnotelength * this.beatspermeasure), 0f, 0f);
			}
			this.repositionNotes();
			float num = Mathf.Floor((this.noteholderr.anchoredPosition3D.x - this.zeroxpos) / (float)(-(float)this.defaultnotelength));
			this.editorposition = num;
			LeanTween.moveLocal(this.noteholder, new Vector3(this.zeroxpos - num * (float)this.defaultnotelength, 0f, 0f), 0.15f).setEaseInOutQuad();
		}
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00017D08 File Offset: 0x00015F08
	private void changeEditorTime(int dir)
	{
		float[] array = new float[]
		{
			8f,
			4f,
			2f,
			1f,
			0.5f,
			0.33333334f,
			0.25f,
			0.125f,
			0.16666667f,
			0.083333336f,
			0.0625f
		};
		this.lengthindex += dir;
		if (this.lengthindex < 0)
		{
			this.lengthindex = array.Length - 1;
		}
		else if (this.lengthindex >= array.Length)
		{
			this.lengthindex = 0;
		}
		this.editorlength = array[this.lengthindex];
		this.editorlengthtext.text = "dur: " + this.editorlength.ToString();
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00017D8C File Offset: 0x00015F8C
	private void moveTimeline(int dir)
	{
		this.editorposition += (float)dir * this.editorlength;
		if (this.editorposition < 0f)
		{
			this.editorposition = 0f;
		}
		LeanTween.moveLocalX(this.noteholder, this.zeroxpos - this.editorposition * (float)this.defaultnotelength, 0.1f).setEaseInOutQuad();
		LeanTween.moveLocalX(this.lyricsholder, this.zeroxpos - this.editorposition * (float)this.defaultnotelength, 0.1f).setEaseInOutQuad();
		this.editorpostext.text = "time: " + this.editorposition.ToString();
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00017E40 File Offset: 0x00016040
	private void tryToDeleteNote()
	{
		for (int i = 0; i < this.leveldata.Count; i++)
		{
			float[] array = this.leveldata[i];
			if (this.editorposition >= array[0] && this.editorposition <= array[0] + array[1])
			{
				this.leveldata.RemoveAt(i);
				UnityEngine.Object.Destroy(this.allnotes[i]);
				this.allnotes.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00017EB4 File Offset: 0x000160B4
	private void tryToDeleteLyric(bool allyrics)
	{
		Debug.Log("Looking for lyrics to delete...");
		if (this.alllyrics.Count == 0)
		{
			Debug.Log("- You dumb asshole! there aren't any lyrics to delete!");
		}
		for (int i = 0; i < this.alllyrics.Count; i++)
		{
			Debug.Log("- Checking lyric #" + i + "...");
			float[] array = this.lyricdata_pos[i];
			this.lyricdata_pos.RemoveAt(i);
			this.lyricdata_txt.RemoveAt(i);
			UnityEngine.Object.Destroy(this.alllyrics[i]);
			this.alllyrics.RemoveAt(i);
		}
		this.alllyrics.Clear();
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x00017F60 File Offset: 0x00016160
	private void tryToDeleteBGThing()
	{
		for (int i = 0; i < this.bgdata.Count; i++)
		{
			float[] array = this.bgdata[i];
			Debug.Log(this.editorposition + ", " + array[2]);
			if (this.editorposition == array[2])
			{
				this.bgdata.RemoveAt(i);
				UnityEngine.Object.Destroy(this.bgobjs[i]);
				this.bgobjs.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00017FEC File Offset: 0x000161EC
	private void buildAllBGNodes()
	{
		this.bgobjs.Clear();
		for (int i = 0; i < this.bgdata.Count; i++)
		{
			float[] array = this.bgdata[i];
			int num = (int)array[1];
			float num2 = array[2];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.singlebg, new Vector3(0f, 0f, 0f), Quaternion.identity, this.lyricsholder.transform);
			this.bgobjs.Add(gameObject);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			gameObject.transform.GetChild(0).GetComponent<Text>().text = num.ToString();
			component.anchoredPosition3D = new Vector3(num2 * (float)this.defaultnotelength, 18f, 0f);
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x000180B4 File Offset: 0x000162B4
	private void clickBG(int bgbtn)
	{
		float num = this.editorposition;
		Debug.Log(string.Concat(new object[]
		{
			"ENTERING BG_ACTION",
			bgbtn,
			" at ",
			num,
			" beat"
		}));
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.singlebg, new Vector3(0f, 0f, 0f), Quaternion.identity, this.lyricsholder.transform);
		this.bgobjs.Add(gameObject);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		gameObject.transform.GetChild(0).GetComponent<Text>().text = bgbtn.ToString();
		component.anchoredPosition3D = new Vector3(num * (float)this.defaultnotelength, 18f, 0f);
		float[] item = new float[]
		{
			60f / this.tempo * num,
			(float)bgbtn,
			num
		};
		this.bgdata.Add(item);
		Debug.Log("bgdata length = " + this.bgdata.Count);
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x000181CE File Offset: 0x000163CE
	private void clearAllBGData()
	{
		Debug.Log("clearing all bg data");
		this.bgdata.Clear();
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x000181E8 File Offset: 0x000163E8
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
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.singlelyric, new Vector3(0f, 0f, 0f), Quaternion.identity, this.lyricsholder.transform);
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

	// Token: 0x060001D6 RID: 470 RVA: 0x000182FC File Offset: 0x000164FC
	private void enterEndpoint()
	{
		Debug.Log("ENTERING ENDPOINT at " + this.editorposition.ToString());
		this.levelendpoint = this.editorposition;
		this.editorendpostext.text = "end: " + this.levelendpoint;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00018350 File Offset: 0x00016550
	private void enterNote()
	{
		if (this.enterindex == 0)
		{
			if (true)
			{
				this.startvals[0] = this.editorposition;
				this.startvals[1] = this.pointerrect.transform.localPosition.y;
				this.enterindex = 1;
				return;
			}
		}
		else
		{
			this.endvals[0] = this.editorposition;
			this.endvals[1] = this.pointerrect.transform.localPosition.y;
			if (this.endvals[0] <= this.startvals[0])
			{
				Debug.Log("note doesn't move forwards");
				return;
			}
			float[] array = new float[]
			{
				this.startvals[0],
				this.endvals[0] - this.startvals[0],
				this.startvals[1],
				this.endvals[1] - this.startvals[1],
				this.endvals[1]
			};
			bool flag = false;
			for (int i = 0; i < this.leveldata.Count; i++)
			{
				if (this.leveldata[i][0] > this.startvals[0])
				{
					Debug.Log("You inserted a note BEFORE note at index " + i);
					flag = true;
					this.leveldata.Insert(i, array);
					break;
				}
			}
			if (!flag)
			{
				this.leveldata.Add(array);
			}
			Debug.Log("FULL LIST OF STARTING VALUES");
			for (int j = 0; j < this.leveldata.Count; j++)
			{
				Debug.Log("-> " + this.leveldata[j][0]);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.singlenote, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform);
			this.allnotes.Add(gameObject);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			RectTransform component2 = gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
			GameObject gameObject2 = gameObject.transform.GetChild(2).gameObject;
			LineRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<LineRenderer>();
			component.anchoredPosition3D = new Vector3(array[0] * (float)this.defaultnotelength, array[2], 0f);
			component2.anchoredPosition3D = new Vector3((float)this.defaultnotelength * array[1] - (float)this.levelnotesize, array[3], 0f);
			float[] item = new float[]
			{
				array[0] * (float)this.defaultnotelength,
				array[0] * (float)this.defaultnotelength + (float)this.defaultnotelength * array[1],
				array[2],
				array[3],
				array[4]
			};
			this.allnotevals.Add(item);
			float num = (float)this.defaultnotelength * array[1];
			float c = array[3];
			foreach (LineRenderer lineRenderer in componentsInChildren)
			{
				lineRenderer.SetPosition(0, new Vector3(0f, 0f, 0f));
				for (int l = 1; l < 10; l++)
				{
					lineRenderer.SetPosition(l, new Vector3(num / 9f * (float)l, this.easeInOutVal((float)l, 0f, c, 9f), 0f));
				}
			}
			this.enterindex = 0;
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00018690 File Offset: 0x00016890
	private void pasteNote(float[] notevals)
	{
		this.leveldata.Add(notevals);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.singlenote, new Vector3(0f, 0f, 0f), Quaternion.identity, this.noteholder.transform);
		this.allnotes.Add(gameObject);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		RectTransform component2 = gameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
		GameObject gameObject2 = gameObject.transform.GetChild(2).gameObject;
		LineRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<LineRenderer>();
		component.anchoredPosition3D = new Vector3(notevals[0] * (float)this.defaultnotelength, notevals[2], 0f);
		component2.anchoredPosition3D = new Vector3((float)this.defaultnotelength * notevals[1] - (float)this.levelnotesize, notevals[3], 0f);
		float[] item = new float[]
		{
			notevals[0] * (float)this.defaultnotelength,
			notevals[0] * (float)this.defaultnotelength + (float)this.defaultnotelength * notevals[1],
			notevals[2],
			notevals[3],
			notevals[4]
		};
		this.allnotevals.Add(item);
		float num = (float)this.defaultnotelength * notevals[1];
		float c = notevals[3];
		foreach (LineRenderer lineRenderer in componentsInChildren)
		{
			lineRenderer.SetPosition(0, new Vector3(0f, 0f, 0f));
			for (int j = 1; j < 10; j++)
			{
				lineRenderer.SetPosition(j, new Vector3(num / 9f * (float)j, this.easeInOutVal((float)j, 0f, c, 9f), 0f));
			}
		}
		this.enterindex = 0;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0001884C File Offset: 0x00016A4C
	private void playFromPlayhead()
	{
		if (!this.playingineditor)
		{
			float num = Mathf.Abs(this.noteholderr.anchoredPosition3D.x - this.zeroxpos);
			bool flag = false;
			for (int i = this.allnotevals.Count - 1; i > -1; i--)
			{
				Debug.Log(num + "," + this.allnotevals[i][0]);
				if (num >= this.allnotevals[i][0])
				{
					Debug.Log("editor starts on note: " + i);
					this.currentnoteindex = i;
					this.grabNoteRefs(0);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.Log("editor starts on note: 0");
				this.currentnoteindex = 0;
				this.grabNoteRefs(0);
			}
			this.trackmovemult = this.tempo / 60f * (float)this.defaultnotelength;
			float time = (this.noteholderr.anchoredPosition3D.x - this.zeroxpos) / -this.trackmovemult;
			this.musictrack.time = time;
			this.musictrack.Play();
			this.playingineditor = true;
			return;
		}
		if (this.playingineditor)
		{
			float num2 = Mathf.Floor((this.noteholderr.anchoredPosition3D.x - this.zeroxpos) / (float)(-(float)this.defaultnotelength));
			this.editorposition = num2;
			this.editorpostext.text = "time: " + this.editorposition.ToString();
			this.playingineditor = false;
			this.musictrack.Stop();
			this.musictrack.time = 0f;
			LeanTween.moveLocal(this.noteholder, new Vector3(this.zeroxpos - num2 * (float)this.defaultnotelength, 0f, 0f), 0.15f).setEaseInOutQuad();
		}
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00018A24 File Offset: 0x00016C24
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

	// Token: 0x060001DB RID: 475 RVA: 0x00018B08 File Offset: 0x00016D08
	private void tryToSaveLevel()
	{
		if (this.levelnamefield.text == "")
		{
			Debug.Log("No level name entered");
			return;
		}
		SavedLevel graph = this.CreateSaveGameObject();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Create(Application.streamingAssetsPath + "/leveldata/" + this.levelnamefield.text + ".tmb");
		Debug.Log(Application.persistentDataPath);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
		Debug.Log("Level Saved at:");
		Debug.Log(Application.streamingAssetsPath + "/leveldata/" + this.levelnamefield.text + ".tmb");
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00018BB0 File Offset: 0x00016DB0
	private SavedLevel CreateSaveGameObject()
	{
		SavedLevel savedLevel = new SavedLevel();
		savedLevel.lyricspos = this.lyricdata_pos;
		savedLevel.lyricstxt = this.lyricdata_txt;
		this.note_c_start = new float[]
		{
			float.Parse(this.col_r_1.text),
			float.Parse(this.col_g_1.text),
			float.Parse(this.col_b_1.text)
		};
		this.note_c_end = new float[]
		{
			float.Parse(this.col_r_2.text),
			float.Parse(this.col_g_2.text),
			float.Parse(this.col_b_2.text)
		};
		savedLevel.note_color_start = this.note_c_start;
		savedLevel.note_color_end = this.note_c_end;
		savedLevel.bgdata = this.bgdata;
		savedLevel.savedleveldata = this.leveldata;
		savedLevel.tempo = this.tempo;
		savedLevel.endpoint = this.levelendpoint;
		savedLevel.timesig = this.beatspermeasure;
		savedLevel.savednotespacing = this.defaultnotelength;
		return savedLevel;
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00018CC8 File Offset: 0x00016EC8
	private void tryToLoadLevel(string filename)
	{
		string text;
		if (filename == "EDITOR")
		{
			text = Application.streamingAssetsPath + "/leveldata/" + this.levelnamefield.text + ".tmb";
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
			this.bgdata.Clear();
			this.bgdata = savedLevel.bgdata;
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
				this.col_r_1.text = this.note_c_start[0].ToString();
				this.col_g_1.text = this.note_c_start[1].ToString();
				this.col_b_1.text = this.note_c_start[2].ToString();
				this.col_r_2.text = this.note_c_end[0].ToString();
				this.col_g_2.text = this.note_c_end[1].ToString();
				this.col_b_2.text = this.note_c_end[2].ToString();
			}
			this.levelendpoint = savedLevel.endpoint;
			this.editorendpostext.text = "end: " + this.levelendpoint;
			this.tempo = savedLevel.tempo;
			this.defaultnotelength = savedLevel.savednotespacing;
			this.defaultnotelength = Mathf.FloorToInt((float)this.defaultnotelength * GlobalVariables.gamescrollspeed);
			this.beatspermeasure = savedLevel.timesig;
			if (this.leveleditor)
			{
				this.buildAllBGNodes();
			}
			this.buildNotes();
			this.buildAllLyrics();
			this.changeEditorTempo(0);
			this.moveTimeline(0);
			this.changeTimeSig(0);
			this.levelendtime = 60f / this.tempo * this.levelendpoint;
			Debug.Log("level end TIME: " + this.levelendtime);
			Debug.Log("Game Loaded");
			return;
		}
		Debug.Log("No file exists at that filename!");
	}

	// Token: 0x060001DE RID: 478 RVA: 0x00018F54 File Offset: 0x00017154
	private void Update()
	{
		if (this.multtexthide > -1f)
		{
			this.multtexthide += 1f * Time.deltaTime;
			if (this.multtexthide > 1.5f)
			{
				this.multtexthide = -1f;
				this.hideMultText();
			}
		}
		float num = 3.68f;
		float num2 = -4.4f;
		float num3 = this.currenthealth / 100f;
		float x = this.healthfill.transform.localPosition.x;
		float num4 = (num2 + num3 * num - x) * (6.85f * Time.deltaTime);
		this.healthposy += 13.5f * Time.deltaTime;
		if (this.healthposy > 3.08f)
		{
			this.healthposy = -2f;
		}
		this.healthfill.transform.localPosition = new Vector3(x + num4, this.healthposy, 0f);
		if (this.currenthealth <= 0.01f)
		{
			this.healthzerotimer += Time.deltaTime;
			if (this.healthzerotimer > 5f)
			{
			}
		}
		else if (this.healthzerotimer > 0f && this.currenthealth > 0.01f)
		{
			this.healthzerotimer = 0f;
		}
		if (this.leveleditor)
		{
			if (Input.GetKeyDown(KeyCode.Return) && !this.enteringlyrics)
			{
				this.playFromPlayhead();
			}
			if (!this.playingineditor && !this.levelnamefield.isFocused && !this.enteringlyrics)
			{
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					if (Input.GetKey(KeyCode.LeftControl))
					{
						this.changeEditorTime(1);
					}
					else if (Input.GetKey(KeyCode.LeftShift))
					{
						this.changeEditorTempo(-1);
					}
					else if (Input.GetKey(KeyCode.LeftAlt))
					{
						this.changeTimeSig(-1);
					}
					else
					{
						this.moveCursor(1);
					}
				}
				else if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					if (Input.GetKey(KeyCode.LeftControl))
					{
						this.changeEditorTime(-1);
					}
					else if (Input.GetKey(KeyCode.LeftShift))
					{
						this.changeEditorTempo(1);
					}
					else if (Input.GetKey(KeyCode.LeftAlt))
					{
						this.changeTimeSig(1);
					}
					else
					{
						this.moveCursor(-1);
					}
				}
				else if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.changeLineSpacing(true, -1);
					}
					else
					{
						this.moveTimeline(-1);
					}
				}
				else if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					{
						this.changeLineSpacing(true, 1);
					}
					else
					{
						this.moveTimeline(1);
					}
				}
				else if (Input.GetKeyDown(KeyCode.N))
				{
					this.enterNote();
				}
				else if (Input.GetKeyDown(KeyCode.M))
				{
					float num5 = Mathf.Floor((this.noteholderr.anchoredPosition3D.x - this.zeroxpos) / (float)(-(float)this.defaultnotelength));
					this.editorposition = num5;
					LeanTween.moveLocal(this.noteholder, new Vector3(this.zeroxpos - num5 * (float)this.defaultnotelength, 0f, 0f), 0.15f).setEaseInOutQuad();
				}
				else if (Input.GetKeyDown(KeyCode.X))
				{
					this.tryToDeleteNote();
				}
				else if (Input.GetKeyDown(KeyCode.Z))
				{
					this.tryToDeleteLyric(true);
				}
				else if (Input.GetKeyDown(KeyCode.O))
				{
					this.resetNoteColor();
				}
				else if (Input.GetKeyDown(KeyCode.B))
				{
					if (!this.enteringlyrics)
					{
						this.clearAllBGData();
					}
				}
				else if (Input.GetKeyDown(KeyCode.H))
				{
					this.editorcopySTART = this.editorposition;
				}
				else if (Input.GetKeyDown(KeyCode.J))
				{
					this.editorcopyEND = this.editorposition;
				}
				else if (Input.GetKeyDown(KeyCode.K))
				{
					this.editorcopyPASTE = this.editorposition;
					float num6 = this.editorcopyPASTE - this.editorcopySTART;
					List<float[]> list = new List<float[]>();
					for (int i = 0; i < this.leveldata.Count; i++)
					{
						float num7 = this.leveldata[i][0];
						if (num7 >= this.editorcopySTART && num7 <= this.editorcopyEND)
						{
							float[] item = new float[]
							{
								this.leveldata[i][0],
								this.leveldata[i][1],
								this.leveldata[i][2],
								this.leveldata[i][3],
								this.leveldata[i][4]
							};
							list.Add(item);
						}
					}
					Debug.Log(list.Count + " notes.");
					for (int j = 0; j < list.Count; j++)
					{
						float[] array = list[j];
						array[0] += num6;
						this.pasteNote(array);
					}
				}
				else if (Input.GetKeyDown(KeyCode.L))
				{
					Debug.Log("ENTERING LYRIC!!");
					this.enteringlyrics = true;
				}
				else if (Input.GetKeyDown(KeyCode.E))
				{
					this.enterEndpoint();
				}
				else if (Input.GetKeyDown(KeyCode.I))
				{
				}
			}
			else if (!this.playingineditor && !this.levelnamefield.isFocused && this.enteringlyrics)
			{
				foreach (object obj in Enum.GetValues(typeof(KeyCode)))
				{
					KeyCode key = (KeyCode)obj;
					if (Input.GetKeyDown(key))
					{
						string text = key.ToString();
						if (text == "Backspace")
						{
							if (this.editorlyrictxt.Length != 0)
							{
								this.editorlyrictxt = this.editorlyrictxt.Substring(0, this.editorlyrictxt.Length - 1);
							}
						}
						else if (text == "Return")
						{
							if (this.editorlyrictxt == "")
							{
								Debug.Log("Press L and type a lyric first");
							}
							else
							{
								float xpos = this.editorposition;
								float y = this.pointerrect.transform.localPosition.y;
								this.enterLyric(xpos, y, this.editorlyrictxt, true);
								this.editorlyrictxt = "";
								this.enteringlyrics = false;
							}
						}
						else if (text == "Space" || text == "space")
						{
							this.editorlyrictxt += " ";
							Debug.Log(this.editorlyrictxt);
						}
						else if (text == "Quote" || text == "quote")
						{
							this.editorlyrictxt += "'";
							Debug.Log(this.editorlyrictxt);
						}
						else if (text.Length == 1)
						{
							if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
							{
								this.editorlyrictxt += text.ToLower();
							}
							else
							{
								this.editorlyrictxt += text;
							}
							Debug.Log(this.editorlyrictxt);
						}
						else if (text == "minus" || text == "Minus")
						{
							this.editorlyrictxt += "-";
						}
					}
				}
			}
		}
		if ((!this.leveleditor && !this.freeplay) || this.playingineditor)
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
			if (!this.leveleditor)
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
				if (this.bgindex < this.bgdata.Count && num8 > this.bgdata[this.bgindex][0])
				{
					Debug.Log("WAH!");
					//this.bgcontroller.bgMove((int)this.bgdata[this.bgindex][1]);
					this.bgindex++;
				}
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
				this.tallyScore();
			}
		}
		if (!this.leveleditor)
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
					this.affectHealthBar(-15f);
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
					this.getScoreAverage();
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
		if ((!this.leveleditor || this.playingineditor) && !this.controllermode && !this.autoplay)
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
			num15 *= this.mousemult * 1f;//GlobalVariables.localsettings.sensitivity;
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
			this.pausecanvas.SetActive(true);
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
				this.setPuppetShake(true);
				this.currentnotesound.time = 0f;
				this.playNote();
			}
			else if (!flag && this.noteplaying && !this.autoplay)
			{
				this.noteplaying = false;
				this.setPuppetShake(false);
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
					this.breathglow.anchoredPosition3D = new Vector3(-380f, 0f, 0f);
					this.outofbreath = true;
					this.noteplaying = false;
					this.setPuppetShake(false);
					this.setPuppetBreath(true);
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
				this.breathglow.anchoredPosition3D = new Vector3(-380f + (this.breathcounter - 1f) * 100f, 0f, 0f);
			}
			if (this.breathcounter < 0f)
			{
				this.breathcounter = 0f;
				if (this.outofbreath)
				{
					this.outofbreath = false;
					this.setPuppetBreath(false);
					Debug.Log("breath back");
				}
			}
		}
		float x2 = 37f - 72f * this.breathcounter;
		float num19 = this.topbreathr.anchoredPosition3D.y;
		num19 += (this.breathcounter + 0.5f) * 3f;
		if (num19 > -85f)
		{
			num19 = -141f;
		}
		float num20 = this.bottombreathr.anchoredPosition3D.y;
		num20 -= (this.breathcounter + 0.5f) * 3f;
		if (num20 < -42f)
		{
			num20 = 14f;
		}
		this.topbreathr.anchoredPosition3D = new Vector3(x2, num19, 0f);
		this.bottombreathr.anchoredPosition3D = new Vector3(x2, num20, 0f);
		if (this.breathcounter != 0f)
		{
			float num21 = this.breathcounter;
		}
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0001A514 File Offset: 0x00018714
	public void pauseQuitLevel()
	{
		//this.sfxrefs.backfromfreeplay.Play();
		//this.curtainc.transitionOut();
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0001A531 File Offset: 0x00018731
	public void pauseRetryLevel()
	{
		this.retrying = true;
		//this.sfxrefs.backfromfreeplay.Play();
		//this.curtainc.transitionOut();
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0001A558 File Offset: 0x00018758
	private void animateOutNote(int noteindex, int performance)
	{
		GameObject gameObject = this.noteparticles.transform.GetChild(this.noteparticles_index).gameObject;
		gameObject.SetActive(true);
		gameObject.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, this.allnotes[noteindex].transform.GetComponent<RectTransform>().anchoredPosition3D.y + this.allnotes[noteindex].transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition3D.y, 0f);
		gameObject.transform.GetChild(0).localScale = new Vector3(0.5f, 0.5f, 1f);
		LeanTween.cancel(gameObject.transform.GetChild(0).gameObject);
		LeanTween.scale(gameObject.transform.GetChild(0).gameObject, new Vector3(1f, 1f, 1f), 0.3f).setEaseOutQuart();
		LeanTween.rotateZ(gameObject.transform.GetChild(0).gameObject, -90f, 0.3f).setEaseLinear();
		Image component = gameObject.transform.GetChild(0).transform.GetComponent<Image>();
		if (performance > 3)
		{
			component.sprite = this.noteparticle_images[1];
		}
		else
		{
			component.sprite = this.noteparticle_images[0];
		}
		CanvasGroup component2 = gameObject.transform.GetChild(0).GetComponent<CanvasGroup>();
		if (performance > 3)
		{
			component2.alpha = 0.6f;
		}
		else
		{
			component2.alpha = 0.4f;
		}
		LeanTween.alphaCanvas(component2, 0f, 0.3f).setEaseInOutQuart();
		GameObject gameObject2 = gameObject.transform.GetChild(1).gameObject;
		LeanTween.cancel(gameObject2);
		gameObject2.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
		gameObject2.transform.GetComponent<CanvasGroup>().alpha = 0.85f;
		LeanTween.scale(gameObject2, new Vector3(0.9f, 0.9f, 1f), 0.25f).setEaseOutQuart();
		LeanTween.alphaCanvas(gameObject2.transform.GetComponent<CanvasGroup>(), 0f, 0.25f).setEaseLinear();
		GameObject gameObject3 = gameObject.transform.GetChild(2).gameObject;
		RectTransform component3 = gameObject3.GetComponent<RectTransform>();
		Text component4 = gameObject3.transform.GetComponent<Text>();
		Text component5 = gameObject3.transform.GetChild(0).GetComponent<Text>();
		gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
		LeanTween.cancel(gameObject3);
		LeanTween.scale(gameObject3, new Vector3(0.7f, 0.7f, 1f), 0.5f).setEaseOutQuart();
		if (this.multiplier > 0)
		{
			component.color = new Color(1f, 1f, 1f, 1f);
			component4.text = this.multiplier.ToString() + "x";
			component5.color = new Color(1f, 1f, 1f, 1f);
			component5.text = this.multiplier.ToString() + "x";
			component3.anchoredPosition3D = new Vector3(15f, 15f, 0f);
			component3.localEulerAngles = new Vector3(0f, 0f, -40f);
			LeanTween.moveLocalY(gameObject3, 45f, 0.5f).setEaseOutQuad();
			LeanTween.rotateZ(gameObject3, -10f, 0.5f).setEaseOutQuart();
		}
		else
		{
			if (performance == 0)
			{
				component.color = new Color(1f, 0f, 0f, 1f);
				component4.text = "x";
				component5.color = new Color(1f, 0f, 0f, 1f);
				component5.text = "x";
			}
			else if (performance == 1)
			{
				component.color = new Color(1f, 1f, 1f, 1f);
				component4.text = "MEH";
				component5.color = new Color(1f, 1f, 1f, 1f);
				component5.text = "MEH";
			}
			else if (performance == 2)
			{
				component.color = new Color(1f, 1f, 1f, 1f);
				component4.text = "OK";
				component5.color = new Color(1f, 1f, 1f, 1f);
				component5.text = "OK";
			}
			component3.anchoredPosition3D = new Vector3(15f, -20f, 0f);
			component3.localEulerAngles = new Vector3(0f, 0f, 40f);
			LeanTween.moveLocalY(gameObject3, -50f, 0.5f).setEaseOutQuad();
			LeanTween.rotateZ(gameObject3, 10f, 0.5f).setEaseOutQuart();
		}
		LeanTween.moveLocalX(gameObject3, 30f, 0.5f).setEaseOutQuart();
		LeanTween.scale(gameObject3, new Vector3(1E-05f, 1E-05f, 0f), 0.2f).setEaseInOutQuart().setDelay(0.51f);
		this.noteparticles_index++;
		if (this.noteparticles_index > 9)
		{
			this.noteparticles_index = 0;
		}
		LeanTween.scaleY(this.allnotes[noteindex], 0.001f, 0.1f).setEaseOutQuart();
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0001AB08 File Offset: 0x00018D08
	private void flashLeftBounds()
	{
		LeanTween.cancel(this.leftboundsglow);
		this.leftboundsglow.transform.localScale = new Vector3(1.25f, 1f, 1f);
		LeanTween.scaleX(this.leftboundsglow, 0.001f, 0.25f).setEaseOutQuad();
		LeanTween.cancel(this.healthmask.transform.parent.gameObject);
		this.healthmask.transform.parent.gameObject.transform.localScale = new Vector3(0.69f, 0.69f, 1f);
		LeanTween.scale(this.healthmask.transform.parent.gameObject, new Vector3(0.6f, 0.6f, 1f), 0.12f).setEaseOutQuart();
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0001ABE5 File Offset: 0x00018DE5
	private void setPuppetBreath(bool hasbreath)
	{
		//this.puppet_humanc.outofbreath = hasbreath;
		//this.puppet_humanc.applyFaceTex();
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0001AC00 File Offset: 0x00018E00
	private void setPuppetShake(bool shake)
	{
		if (shake)
		{
			LeanTween.alphaCanvas(this.pointerglow, 0.95f, 0.05f);
		}
		else
		{
			LeanTween.alphaCanvas(this.pointerglow, 0f, 0.05f);
		}
		//this.puppet_humanc.shaking = shake;
		//this.puppet_humanc.applyFaceTex();
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0001AC58 File Offset: 0x00018E58
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

	// Token: 0x060001E6 RID: 486 RVA: 0x0001ACB8 File Offset: 0x00018EB8
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

	// Token: 0x060001E7 RID: 487 RVA: 0x0001AD54 File Offset: 0x00018F54
	private void stopNote()
	{
		this.currentnotesound.Stop();
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0001AD61 File Offset: 0x00018F61
	public void clickBG1()
	{
		this.clickBG(1);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0001AD6A File Offset: 0x00018F6A
	public void clickBG2()
	{
		this.clickBG(2);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0001AD73 File Offset: 0x00018F73
	public void clickBG3()
	{
		this.clickBG(3);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0001AD7C File Offset: 0x00018F7C
	public void clickBG4()
	{
		this.clickBG(4);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0001AD85 File Offset: 0x00018F85
	public void clickBG5()
	{
		this.clickBG(5);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0001AD8E File Offset: 0x00018F8E
	public void clickBG6()
	{
		this.clickBG(6);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0001AD97 File Offset: 0x00018F97
	public void clickBG7()
	{
		this.clickBG(7);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0001ADA0 File Offset: 0x00018FA0
	public void clickBG8()
	{
		this.clickBG(8);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0001ADA9 File Offset: 0x00018FA9
	public void clickBG9()
	{
		this.clickBG(9);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0001ADB3 File Offset: 0x00018FB3
	public void clickBG10()
	{
		this.clickBG(10);
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0001ADBD File Offset: 0x00018FBD
	public void clickBG11()
	{
		this.clickBG(11);
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0001ADC7 File Offset: 0x00018FC7
	public void clickBG12()
	{
		this.clickBG(12);
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0001ADD1 File Offset: 0x00018FD1
	public void clickBG13()
	{
		this.clickBG(13);
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0001ADDB File Offset: 0x00018FDB
	public void clickBGX()
	{
		this.tryToDeleteBGThing();
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0001ADE4 File Offset: 0x00018FE4
	private void aNote(float pos, float duration, string startnote, string endnote)
	{
		float num = this.returnYPos(startnote);
		float num2 = this.returnYPos(endnote);
		float num3 = num2 - num;
		float[] item = new float[]
		{
			pos,
			duration,
			num,
			num3,
			num2
		};
		this.leveldata.Add(item);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0001AE30 File Offset: 0x00019030
	private float returnYPos(string notename)
	{
		notename = notename.ToUpper();
		int num = 0;
		if (notename == "C3")
		{
			num = 0;
		}
		else if (notename == "B2")
		{
			num = 1;
		}
		else if (notename == "A2")
		{
			num = 2;
		}
		else if (notename == "G2")
		{
			num = 3;
		}
		else if (notename == "F2")
		{
			num = 4;
		}
		else if (notename == "E2")
		{
			num = 5;
		}
		else if (notename == "D2")
		{
			num = 6;
		}
		else if (notename == "C2")
		{
			num = 7;
		}
		else if (notename == "B1")
		{
			num = 8;
		}
		else if (notename == "A1")
		{
			num = 9;
		}
		else if (notename == "G1")
		{
			num = 10;
		}
		else if (notename == "F1")
		{
			num = 11;
		}
		else if (notename == "E1")
		{
			num = 12;
		}
		else if (notename == "D1")
		{
			num = 13;
		}
		else if (notename == "C1")
		{
			num = 14;
		}
		return this.notelinepos[num];
	}

	// Token: 0x040001E7 RID: 487
	private float latency_offset;

	// Token: 0x040001E8 RID: 488
	public AudioMixer audmix;

	// Token: 0x040001E9 RID: 489
	public AudioMixerGroup audmix_bgmus;

	// Token: 0x040001EA RID: 490
	public GameObject[] playermodels;

	// Token: 0x040001EB RID: 491
	private int puppetnum;

	// Token: 0x040001EC RID: 492
	private int textureindex = 3;

	// Token: 0x040001ED RID: 493
	public int soundset;

	// Token: 0x040001EE RID: 494
	private bool level_finshed;

	// Token: 0x040001EF RID: 495
	private bool readytoplay;

	// Token: 0x040001F0 RID: 496
	private Gyroscope m_Gyro;

	// Token: 0x040001F1 RID: 497
	public int levelnum;

	// Token: 0x040001F2 RID: 498
	public bool controllermode = true;

	// Token: 0x040001F3 RID: 499
	public bool freeplay;

	// Token: 0x040001F4 RID: 500
	public GameObject backbtn;

	// Token: 0x040001F5 RID: 501
	public GameObject editorcanvas;

	// Token: 0x040001F6 RID: 502
	public bool leveleditor;

	// Token: 0x040001F7 RID: 503
	private bool playingineditor;

	// Token: 0x040001F8 RID: 504
	private int editornoteindex;

	// Token: 0x040001F9 RID: 505
	private float editorposition;

	// Token: 0x040001FA RID: 506
	private float editorlength = 0.25f;

	// Token: 0x040001FB RID: 507
	private float editorcopySTART;

	// Token: 0x040001FC RID: 508
	private float editorcopyEND;

	// Token: 0x040001FD RID: 509
	private float editorcopyPASTE;

	// Token: 0x040001FE RID: 510
	private float noteoffset = 0.05f;

	// Token: 0x040001FF RID: 511
	private float levelendpoint;

	// Token: 0x04000200 RID: 512
	private float levelendtime;

	// Token: 0x04000201 RID: 513
	private int lengthindex = 3;

	// Token: 0x04000202 RID: 514
	public Text editorpostext;

	// Token: 0x04000203 RID: 515
	public Text editorlengthtext;

	// Token: 0x04000204 RID: 516
	public Text editorspacingtext;

	// Token: 0x04000205 RID: 517
	public Text editortempotext;

	// Token: 0x04000206 RID: 518
	public Text editortsigtext;

	// Token: 0x04000207 RID: 519
	public Text editorendpostext;

	// Token: 0x04000208 RID: 520
	public float[] startvals = new float[2];

	// Token: 0x04000209 RID: 521
	public float[] endvals = new float[2];

	// Token: 0x0400020A RID: 522
	public int enterindex;

	// Token: 0x0400020B RID: 523
	public InputField levelnamefield;

	// Token: 0x0400020C RID: 524
	public InputField col_r_1;

	// Token: 0x0400020D RID: 525
	public InputField col_g_1;

	// Token: 0x0400020E RID: 526
	public InputField col_b_1;

	// Token: 0x0400020F RID: 527
	public InputField col_r_2;

	// Token: 0x04000210 RID: 528
	public InputField col_g_2;

	// Token: 0x04000211 RID: 529
	public InputField col_b_2;

	// Token: 0x04000212 RID: 530
	private bool autoplay;

	// Token: 0x04000213 RID: 531
	public Text autoplaytxt;

	// Token: 0x04000214 RID: 532
	private bool paused;

	// Token: 0x04000215 RID: 533
	public bool quitting;

	// Token: 0x04000216 RID: 534
	public bool retrying;

	// Token: 0x04000217 RID: 535
	public GameObject pausecanvas;

	// Token: 0x04000218 RID: 536
	//public PauseCanvasController pausecontroller;

	// Token: 0x04000219 RID: 537
	public GameObject[] alltracks = new GameObject[2];

	// Token: 0x0400021A RID: 538
	public GameObject curtains;

	// Token: 0x0400021B RID: 539
	//private CurtainController curtainc;

	// Token: 0x0400021C RID: 540
	public GameObject pointer;

	// Token: 0x0400021D RID: 541
	public Image pointerimg;

	// Token: 0x0400021E RID: 542
	public CanvasGroup pointerglow;

	// Token: 0x0400021F RID: 543
	public int pointercolorindex;

	// Token: 0x04000220 RID: 544
	private bool flipscheme;

	// Token: 0x04000221 RID: 545
	public GameObject topbreath;

	// Token: 0x04000222 RID: 546
	public GameObject bottombreath;

	// Token: 0x04000223 RID: 547
	private RectTransform topbreathr;

	// Token: 0x04000224 RID: 548
	private RectTransform bottombreathr;

	// Token: 0x04000225 RID: 549
	public RectTransform breathglow;

	// Token: 0x04000226 RID: 550
	private float breathscale = 1f;

	// Token: 0x04000227 RID: 551
	private float breathcounter;

	// Token: 0x04000228 RID: 552
	private bool outofbreath;

	// Token: 0x04000229 RID: 553
	public GameObject leftbounds;

	// Token: 0x0400022A RID: 554
	public GameObject leftboundsglow;

	// Token: 0x0400022B RID: 555
	private RectTransform pointerrect;

	// Token: 0x0400022C RID: 556
	public GameObject soundSets;

	// Token: 0x0400022D RID: 557
	//private AudioRefs audiorefs;

	// Token: 0x0400022E RID: 558
	//public SFXRefs sfxrefs;

	// Token: 0x0400022F RID: 559
	public Text songtitle;

	// Token: 0x04000230 RID: 560
	public Text songtitleshadow;

	// Token: 0x04000231 RID: 561
	public GameObject notelinesholder;

	// Token: 0x04000232 RID: 562
	private GameObject[] notelines = new GameObject[15];

	// Token: 0x04000233 RID: 563
	private float[] notelinepos = new float[15];

	// Token: 0x04000234 RID: 564
	public GameObject noteparticles;

	// Token: 0x04000235 RID: 565
	private RectTransform noteparticlesrect;

	// Token: 0x04000236 RID: 566
	private int noteparticles_index;

	// Token: 0x04000237 RID: 567
	public Sprite[] noteparticle_images = new Sprite[2];

	// Token: 0x04000238 RID: 568
	private int defaultnotelength = 240;

	// Token: 0x04000239 RID: 569
	private int numbeatlines = 6;

	// Token: 0x0400023A RID: 570
	private int beatstoshow = 16;

	// Token: 0x0400023B RID: 571
	public GameObject beatline;

	// Token: 0x0400023C RID: 572
	private int beatlineindex;

	// Token: 0x0400023D RID: 573
	private float maxbeatlinex;

	// Token: 0x0400023E RID: 574
	private RectTransform[] allbeatlines = new RectTransform[12];

	// Token: 0x0400023F RID: 575
	private RectTransform[] alleditorbeatlines = new RectTransform[500];

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

	// Token: 0x04000242 RID: 578
	public GameObject noteholder;

	// Token: 0x04000243 RID: 579
	private RectTransform noteholderr;

	// Token: 0x04000244 RID: 580
	public GameObject lyricsholder;

	// Token: 0x04000245 RID: 581
	private RectTransform lyricsholderr;

	// Token: 0x04000246 RID: 582
	public GameObject singlenote;

	// Token: 0x04000247 RID: 583
	public GameObject singlelyric;

	// Token: 0x04000248 RID: 584
	public GameObject singlebg;

	// Token: 0x04000249 RID: 585
	private List<float[]> leveldata = new List<float[]>();

	// Token: 0x0400024A RID: 586
	private List<GameObject> alllyrics = new List<GameObject>();

	// Token: 0x0400024B RID: 587
	private List<float[]> lyricdata_pos = new List<float[]>();

	// Token: 0x0400024C RID: 588
	private List<string> lyricdata_txt = new List<string>();

	// Token: 0x0400024D RID: 589
	private string editorlyrictxt = "";

	// Token: 0x0400024E RID: 590
	private bool enteringlyrics;

	// Token: 0x0400024F RID: 591
	private List<float[]> bgdata = new List<float[]>();

	// Token: 0x04000250 RID: 592
	private List<GameObject> bgobjs = new List<GameObject>();

	// Token: 0x04000251 RID: 593
	private int bgindex;

	// Token: 0x04000252 RID: 594
	public GameObject bgholder;

	// Token: 0x04000253 RID: 595
	//public BGController bgcontroller;

	// Token: 0x04000254 RID: 596
	private float tempo = 40f;

	// Token: 0x04000255 RID: 597
	private float tempotimer;

	// Token: 0x04000256 RID: 598
	private float tempotimerdot;

	// Token: 0x04000257 RID: 599
	private int beatnum = 1;

	// Token: 0x04000258 RID: 600
	private int beatnumdot = 1;

	// Token: 0x04000259 RID: 601
	private int beatspermeasure = 2;

	// Token: 0x0400025A RID: 602
	private int timesigcount = 1;

	// Token: 0x0400025B RID: 603
	private float trackmovemult = 1f;

	// Token: 0x0400025C RID: 604
	public GameObject musicref;

	// Token: 0x0400025D RID: 605
	public AudioSource musictrack;

	// Token: 0x0400025E RID: 606
	private int dotsize = 60;

	// Token: 0x0400025F RID: 607
	private int levelnotesize = 20;

	// Token: 0x04000260 RID: 608
	private List<GameObject> allnotes = new List<GameObject>();

	// Token: 0x04000261 RID: 609
	private List<float[]> allnotevals = new List<float[]>();

	// Token: 0x04000262 RID: 610
	private int currentnoteindex;

	// Token: 0x04000263 RID: 611
	private float currentnotestart;

	// Token: 0x04000264 RID: 612
	private float currentnoteend;

	// Token: 0x04000265 RID: 613
	private float currentnotestarty;

	// Token: 0x04000266 RID: 614
	private float currentnoteendy;

	// Token: 0x04000267 RID: 615
	private float currentnotepshift;

	// Token: 0x04000268 RID: 616
	private bool noteactive;

	// Token: 0x04000269 RID: 617
	private float notescoreaverage;

	// Token: 0x0400026A RID: 618
	private float notescoresamples = 1f;

	// Token: 0x0400026B RID: 619
	private float note_end_timer;

	// Token: 0x0400026C RID: 620
	private float max_note_end_timer = 0.25f;

	// Token: 0x0400026D RID: 621
	private float notestartpos;

	// Token: 0x0400026E RID: 622
	public AudioSource currentnotesound;

	// Token: 0x0400026F RID: 623
	private AudioClipsTromb trombclips;

	// Token: 0x04000270 RID: 624
	private float pitchamount = 0.00501f;

	// Token: 0x04000271 RID: 625
	private float mousemult = 1.35f;

	// Token: 0x04000272 RID: 626
	private float movesmoothing = 35f;

	// Token: 0x04000273 RID: 627
	private float vbounds = 165f;

	// Token: 0x04000274 RID: 628
	private float outerbuffer = 15f;

	// Token: 0x04000275 RID: 629
	private float zeroxpos = 60f;

	// Token: 0x04000276 RID: 630
	private float vsensitivity = -350f;

	// Token: 0x04000277 RID: 631
	private float startmouseX;

	// Token: 0x04000278 RID: 632
	private float vibratoamt;

	// Token: 0x04000279 RID: 633
	public GameObject cameramodelparent;

	// Token: 0x0400027A RID: 634
	public GameObject modelparent;

	// Token: 0x0400027B RID: 635
	private GameObject puppet_human;

	// Token: 0x0400027C RID: 636
	//private HumanPuppetController puppet_humanc;

	// Token: 0x0400027D RID: 637
	public GameObject puppet_mk22;

	// Token: 0x0400027E RID: 638
	//private puppetcontroller_mk22 p3dc_r;

	// Token: 0x0400027F RID: 639
	public GameObject puppet_tromb;

	// Token: 0x04000280 RID: 640
	//private TrombonePuppetController p3dc_t;

	// Token: 0x04000281 RID: 641
	private bool noteplaying;

	// Token: 0x04000282 RID: 642
	private bool released_button_between_notes;

	// Token: 0x04000283 RID: 643
	//public PostProcessingProfile gameplayppp;

	// Token: 0x04000284 RID: 644
	//public RainbowEffect rainbowcontroller;

	// Token: 0x04000285 RID: 645
	public Button ui_savebtn;

	// Token: 0x04000286 RID: 646
	public Button ui_loadbtn;

	// Token: 0x04000287 RID: 647
	public Text ui_score;

	// Token: 0x04000288 RID: 648
	public Text ui_score_shadow;

	// Token: 0x04000289 RID: 649
	private int multiplier;

	// Token: 0x0400028A RID: 650
	private int max_multiplier = 10;

	// Token: 0x0400028B RID: 651
	public Text highestcombo;

	// Token: 0x0400028C RID: 652
	public Text highestcomboshad;

	// Token: 0x0400028D RID: 653
	public GameObject highestcomboobj;

	// Token: 0x0400028E RID: 654
	private int highestcombo_level;

	// Token: 0x0400028F RID: 655
	private int highestcombocounter;

	// Token: 0x04000290 RID: 656
	private int currentscore;

	// Token: 0x04000291 RID: 657
	private int totalscore;

	// Token: 0x04000292 RID: 658
	private int maxlevelscore;

	// Token: 0x04000293 RID: 659
	private float scorecounter;

	// Token: 0x04000294 RID: 660
	public GameObject popuptextobj;

	// Token: 0x04000295 RID: 661
	public GameObject multtextobj;

	// Token: 0x04000296 RID: 662
	public Text popuptext;

	// Token: 0x04000297 RID: 663
	public Text multtext;

	// Token: 0x04000298 RID: 664
	public Text popuptextshadow;

	// Token: 0x04000299 RID: 665
	public Text multtextshadow;

	// Token: 0x0400029A RID: 666
	private float multtexthide;

	// Token: 0x0400029B RID: 667
	private float currenthealth;

	// Token: 0x0400029C RID: 668
	public GameObject healthobj;

	// Token: 0x0400029D RID: 669
	public GameObject healthmask;

	// Token: 0x0400029E RID: 670
	public GameObject healthfill;

	// Token: 0x0400029F RID: 671
	private float healthposy = -2f;

	// Token: 0x040002A0 RID: 672
	private float healthzerotimer;

	// Token: 0x040002A1 RID: 673
	//public ChampGUIController champcontroller;

	// Token: 0x040002A2 RID: 674
	private int scores_A;

	// Token: 0x040002A3 RID: 675
	private int scores_B;

	// Token: 0x040002A4 RID: 676
	private int scores_C;

	// Token: 0x040002A5 RID: 677
	private int scores_D;

	// Token: 0x040002A6 RID: 678
	private int scores_F;

	// Token: 0x040002A7 RID: 679
	public Text debugtext;

	// Token: 0x040002A8 RID: 680
	private AssetBundle myLoadedAssetBundle;

	// Token: 0x040002A9 RID: 681
	private AssetBundle mySoundAssetBundle;
}
