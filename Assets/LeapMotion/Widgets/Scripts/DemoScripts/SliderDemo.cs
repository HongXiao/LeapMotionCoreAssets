﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LMWidgets;

public class SliderDemo : SliderBase 
{
  public GameObject activeBar = null;
  public SliderDemoGraphics topLayer = null;
  public SliderDemoGraphics midLayer = null;
  public SliderDemoGraphics botLayer = null;
  public GameObject dot = null;
  public int numberOfDots = 0;

  private List<GameObject> dots = new List<GameObject>();

  public override void SliderPressed()
  {
    PressedGraphics();
  }

  public override void SliderReleased()
  {
    ReleasedGraphics();
  }

  private void PressedGraphics()
  {
    topLayer.SetBloomGain(5.0f);
    botLayer.SetBloomGain(4.0f);
    botLayer.SetColor(new Color(0.0f, 1.0f, 1.0f, 1.0f));
  }

  private void ReleasedGraphics()
  {
    topLayer.SetBloomGain(2.0f);
    botLayer.SetBloomGain(2.0f);
    botLayer.SetColor(new Color(0.0f, 0.25f, 0.25f, 0.5f));
  }

  // Updates the slider handle graphics
  private void UpdateGraphics()
  {
    float handleFraction = GetHandleFraction();
    Vector3 topPosition = transform.localPosition;
    topPosition.z -= (1.0f - handleFraction) * 0.25f;
    topPosition.z = Mathf.Min(topPosition.z, -0.003f); // -0.003 is so midLayer will never intercept with top or bot layer
    topLayer.transform.localPosition = topPosition;

    Vector3 botPosition = transform.localPosition;
    botPosition.z = -0.001f;
    botLayer.transform.localPosition = botPosition;

    midLayer.transform.localPosition = (topPosition + botPosition) / 2.0f;

    if (activeBar)
    {
      UpdateActiveBar();
    }
    if (numberOfDots > 0)
    {
      UpdateDots();
    }
  }

  // Updates the active bar behind the handle
  private void UpdateActiveBar()
  {
    Vector3 activeBarPosition = activeBar.transform.localPosition;
    activeBarPosition.x = (transform.localPosition.x + lowerLimit.transform.localPosition.x) / 2.0f;
    activeBar.transform.localPosition = activeBarPosition;
    Vector3 activeBarScale = activeBar.transform.localScale;
    activeBarScale.x = Mathf.Abs(transform.localPosition.x - lowerLimit.transform.localPosition.x);
    activeBar.transform.localScale = activeBarScale;
    Renderer[] renderers = activeBar.GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      renderer.material.SetFloat("_Gain", 3.0f);
    }

    if (GetSliderFraction() > 99.0f)
    {
      Renderer[] upper_limit_renderers = upperLimit.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in upper_limit_renderers)
      {
        renderer.enabled = true;
      }
    }
    else
    {
      Renderer[] upper_limit_renderers = upperLimit.GetComponentsInChildren<Renderer>();
      foreach (Renderer renderer in upper_limit_renderers)
      {
        renderer.enabled = false;
      }
    }
  }

  // Updates the dots above the slider
  private void UpdateDots()
  {
    for (int i = 0; i < dots.Count; ++i)
    {
      if (dots[i].transform.localPosition.x < transform.localPosition.x)
      {
        Renderer[] renderers = dots[i].GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
          renderer.material.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
          renderer.material.SetFloat("_Gain", 3.0f);
        }
      }
      else
      {
        Renderer[] renderers = dots[i].GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
          renderer.material.color = new Color(0.0f, 0.25f, 0.25f, 0.5f);
          renderer.material.SetFloat("_Gain", 1.0f);
        }
      }
    }
  }

  protected override void Awake()
  {
    base.Awake();
    // Initiate the graphics for the handle
    ReleasedGraphics();

    // Initiate the dots
    if (numberOfDots > 0)
    {
      float lower_limit = lowerLimit.transform.localPosition.x;
      float upper_limit = upperLimit.transform.localPosition.x;
      float length = upper_limit - lower_limit;
      float increments = length / numberOfDots;

      for (float x = lower_limit + increments / 2.0f; x < upper_limit; x += increments)
      {
        GameObject new_dot = Instantiate(dot) as GameObject;
        new_dot.transform.parent = transform;
        new_dot.transform.localPosition = new Vector3(x, 1.0f, m_localTriggerDistance);
        new_dot.transform.localRotation = dot.transform.localRotation;
        new_dot.transform.localScale = Vector3.one;
        new_dot.transform.parent = transform.parent;
        dots.Add(new_dot);
      }
      Destroy(dot);
      UpdateDots();
    }

    // Initiate the graphics for the active bar
    if (activeBar)
    {
      UpdateActiveBar();
    }
  }

  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    UpdateGraphics();
  }
}
